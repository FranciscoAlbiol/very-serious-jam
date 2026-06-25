using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Button nextButton;

    public GameObject choiceBox;
    public Button choiceButtonA;
    public Button choiceButtonB;
    public TextMeshProUGUI choiceLabelA;
    public TextMeshProUGUI choiceLabelB;

    public float letterDelay = 0.05f;


    private AudioSource audioSource;

    private DialogueBlock[] allBlocks;
    private DialogueTrigger activeTrigger;

    private DialogueBlock currentBlock;
    private int lineIndex = 0;

    private bool dialogueActive = false;
    private bool buttonReady = false;
    private bool isTyping = false;
    private string targetText = "";
    private int visibleCount = 0;
    private float timer = 0f;


    void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        dialogueBox.SetActive(false);
        choiceBox.SetActive(false);
        nextButton.onClick.AddListener(OnNextPressed);
        nextButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive || !isTyping) return;

        timer += Time.deltaTime;
        while (timer >= letterDelay && visibleCount < targetText.Length)
        {
            visibleCount++;
            timer -= letterDelay;
            dialogueText.text = targetText.Substring(0, visibleCount);
        }

        if (visibleCount >= targetText.Length)
        {
            isTyping = false;
            audioSource.loop = false;
            audioSource.Stop();
            StartCoroutine(ReadyNextFrame());
        }
    }

    public void StartBlock(DialogueBlock[] blocks, int index, DialogueTrigger trigger)
    {
        allBlocks = blocks;
        activeTrigger = trigger;

        DialogueBlock block = GetBlock(index);
        if (block == null)
        {
            Debug.LogWarning($"[DialogueManager] No block found for index {index}");
            return;
        }

        dialogueActive = true;
        dialogueBox.SetActive(true);
        choiceBox.SetActive(false);
        nextButton.gameObject.SetActive(false);
        buttonReady = false;

        LoadBlock(block);
    }

    public void EndDialogue()
    {
        if (activeTrigger != null && currentBlock != null)
            activeTrigger.MarkIndexSeen(currentBlock.index);

        dialogueActive = false;
        isTyping = false;
        audioSource.Stop();
        dialogueBox.SetActive(false);
        choiceBox.SetActive(false);
        nextButton.gameObject.SetActive(false);

        PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
        if (interaction != null)
            interaction.EndInteraction();
    }


    void LoadBlock(DialogueBlock block)
    {
        currentBlock = block;
        lineIndex = 0;

        if (block.lines == null || block.lines.Length == 0)
        {
            OnBlockLinesFinished();
            return;
        }

        ShowLine(block.lines[lineIndex]);
    }

    void ShowLine(DialogueLine line)
    {
        choiceBox.SetActive(false);
        nextButton.gameObject.SetActive(false);
        buttonReady = false;

        if (nameText != null)
        {
            bool hasName = !string.IsNullOrEmpty(line.characterName);
            nameText.gameObject.SetActive(hasName);
            nameText.text = hasName ? line.characterName : "";
        }

        if (line.characterRenderer != null && line.characterSprite != null)
            line.characterRenderer.sprite = line.characterSprite;

        if (line.voiceLine != null)
        {
            audioSource.Stop();
            audioSource.clip = line.voiceLine;
            audioSource.loop = true;
            audioSource.Play();
        }

        targetText = line.text;
        visibleCount = 0;
        timer = 0f;
        isTyping = true;
        dialogueText.text = "";
    }

    IEnumerator ReadyNextFrame()
    {
        yield return null;
        buttonReady = true;
        nextButton.gameObject.SetActive(true);
    }

    void OnNextPressed()
    {
        if (!dialogueActive || !buttonReady) return;

        if (isTyping)
        {
            isTyping = false;
            visibleCount = targetText.Length;
            dialogueText.text = targetText;
            audioSource.loop = false;
            audioSource.Stop();
            buttonReady = false;
            StartCoroutine(ReadyNextFrame());
            return;
        }

        DialogueLine finishedLine = currentBlock.lines[lineIndex];
        finishedLine.onLineFinished?.Invoke();

        buttonReady = false;
        nextButton.gameObject.SetActive(false);
        lineIndex++;

        if (lineIndex < currentBlock.lines.Length)
        {
            ShowLine(currentBlock.lines[lineIndex]);
        }
        else
        {
            OnBlockLinesFinished();
        }
    }

    void OnBlockLinesFinished()
    {
        if (currentBlock.hasChoice)
        {
            ShowChoice(currentBlock.choice);
        }
        else if (currentBlock.continueToNextBlock)
        {
            DialogueBlock nextBlock = GetBlock(currentBlock.nextBlockIndex);
            if (nextBlock == null)
            {
                Debug.LogWarning($"[DialogueManager] continueToNextBlock points to index {currentBlock.nextBlockIndex} but no block found.");
                EndDialogue();
                return;
            }
            LoadBlock(nextBlock);
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowChoice(DialogueChoice choice)
    {
        nextButton.gameObject.SetActive(false);
        dialogueText.text = "";
        if (nameText != null) nameText.gameObject.SetActive(false);

        choiceLabelA.text = choice.optionA.label;
        choiceLabelB.text = choice.optionB.label;

        choiceButtonA.onClick.RemoveAllListeners();
        choiceButtonB.onClick.RemoveAllListeners();

        choiceButtonA.onClick.AddListener(() => OnOptionChosen(choice.optionA));
        choiceButtonB.onClick.AddListener(() => OnOptionChosen(choice.optionB));

        choiceBox.SetActive(true);
    }

    void OnOptionChosen(DialogueChoiceOption option)
    {
        choiceBox.SetActive(false);

        option.onChosen?.Invoke();

        if (option.nextIndex < 0)
        {
            EndDialogue();
            return;
        }

        DialogueBlock nextBlock = GetBlock(option.nextIndex);
        if (nextBlock == null)
        {
            Debug.LogWarning($"[DialogueManager] Choice points to index {option.nextIndex} but no block found.");
            EndDialogue();
            return;
        }

        LoadBlock(nextBlock);
    }

    DialogueBlock GetBlock(int index)
    {
        if (allBlocks == null) return null;
        foreach (DialogueBlock block in allBlocks)
            if (block.index == index) return block;
        return null;
    }
}
