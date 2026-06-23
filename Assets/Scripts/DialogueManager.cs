using System.Collections;
using System.Collections.Generic;
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

    public Button[] choiceButtons;

    public float letterDelay = 0.05f;

    private AudioSource audioSource;
    private DialogueLine[] currentLines;
    private int currentIndex = 0;
    private bool dialogueActive = false;
    private DialogueTrigger activeTrigger;
    private int activeDialogueIndex;

    private string targetText = "";
    private int visibleCount = 0;
    private float timer = 0f;
    private bool isTyping = false;
    private bool buttonReady = false;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        dialogueBox.SetActive(false);
        nextButton.onClick.AddListener(OnNextPressed);

        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (isTyping)
        {
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
                StartCoroutine(EnableButtonNextFrame());
            }
        }
    }

    IEnumerator EnableButtonNextFrame()
    {
        yield return null;
        buttonReady = true;
        Debug.Log($"[Dialogue] buttonReady=true, currentIndex={currentIndex}");

        DialogueLine line = currentLines[currentIndex];
        if (line.isChoice && line.choices != null && line.choices.Length > 0)
        {
            Debug.Log($"[Dialogue] Showing {line.choices.Length} choice buttons");
            ShowChoiceButtons(line);
        }
        else
            nextButton.gameObject.SetActive(true);
    }


    void ShowChoiceButtons(DialogueLine line)
    {
        nextButton.gameObject.SetActive(false);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < line.choices.Length)
            {
                DialogueChoice capturedChoice = line.choices[i];

                choiceButtons[i].gameObject.SetActive(true);

                TextMeshProUGUI label = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>(true);
                if (label != null) label.text = capturedChoice.label ?? "";

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoicePressed(capturedChoice));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HideChoiceButtons()
    {
        foreach (Button btn in choiceButtons)
            btn.gameObject.SetActive(false);
    }

    void OnChoicePressed(DialogueChoice choice)
    {
        Debug.Log($"[Dialogue] OnChoicePressed called. dialogueActive={dialogueActive} buttonReady={buttonReady} choice={choice?.label}");
        if (!dialogueActive || !buttonReady)
        {
            Debug.LogWarning($"[Dialogue] Choice blocked. dialogueActive={dialogueActive} buttonReady={buttonReady}");
            return;
        }

        HideChoiceButtons();
        buttonReady = false;

        choice.onChosen?.Invoke();

        if (choice.nextLineIndex < 0 || choice.nextLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        currentIndex = choice.nextLineIndex;
        ShowLine(currentIndex);
    }


    public void StartDialogue(DialogueLine[] allLines, int dialogueIndex, DialogueTrigger trigger = null)
    {
        List<DialogueLine> filtered = new List<DialogueLine>();
        foreach (DialogueLine line in allLines)
        {
            if (line.dialogueIndex == dialogueIndex)
                filtered.Add(line);
        }

        if (filtered.Count == 0)
        {
            Debug.LogWarning($"DialogueManager: No lines found for index {dialogueIndex}");
            return;
        }

        activeTrigger = trigger;
        activeDialogueIndex = dialogueIndex;
        currentLines = filtered.ToArray();
        currentIndex = 0;
        dialogueActive = true;
        dialogueBox.SetActive(true);
        nextButton.gameObject.SetActive(false);
        HideChoiceButtons();
        buttonReady = false;
        ShowLine(0);
    }

    void ShowLine(int index)
    {
        DialogueLine line = currentLines[index];

        if (nameText != null)
        {
            nameText.text = string.IsNullOrEmpty(line.characterName) ? "" : line.characterName;
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(line.characterName));
        }

        if (line.characterRenderer != null)
            line.characterRenderer.sprite = line.characterSprite;

        if (line.voiceLine != null)
        {
            audioSource.Stop();
            audioSource.clip = line.voiceLine;
            audioSource.Play();
        }

        targetText = line.text;
        visibleCount = 0;
        timer = 0f;
        isTyping = true;
        buttonReady = false;
        dialogueText.text = "";
        nextButton.gameObject.SetActive(false);
        HideChoiceButtons();

        line.onLineShown?.Invoke();
    }

    public void OnNextPressed()
    {
        if (!dialogueActive || currentLines == null || !buttonReady) return;

        if (isTyping)
        {
            isTyping = false;
            visibleCount = targetText.Length;
            dialogueText.text = targetText;
            buttonReady = false;
            StartCoroutine(EnableButtonNextFrame());
            return;
        }

        buttonReady = false;
        nextButton.gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine(currentIndex);
    }

    public void EndDialogue()
    {
        if (activeTrigger != null)
            activeTrigger.MarkIndexSeen(activeDialogueIndex);

        bool shouldContinue = activeTrigger != null
            && currentLines != null
            && currentLines.Length > 0
            && currentLines[currentLines.Length - 1].continueToNextIndex;

        dialogueActive = false;
        isTyping = false;
        audioSource.Stop();
        dialogueBox.SetActive(false);
        HideChoiceButtons();
        currentLines = null;

        if (shouldContinue)
        {
            activeTrigger.AdvanceAndRestart();
            return;
        }

        PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
        if (interaction != null)
            interaction.EndInteraction();
    }
}
