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
    public Image characterSprite;
    public Button nextButton;

    public float letterDelay = 0.05f;

    private AudioSource audioSource;
    private DialogueLine[] currentLines;
    private int currentIndex = 0;
    private bool dialogueActive = false;

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
        nextButton.gameObject.SetActive(true);
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        currentLines = lines;
        currentIndex = 0;
        dialogueActive = true;
        dialogueBox.SetActive(true);
        nextButton.gameObject.SetActive(false);
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

        if (characterSprite != null)
        {
            characterSprite.sprite = line.characterSprite;
            characterSprite.gameObject.SetActive(line.characterSprite != null);
        }

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
    }

    void OnNextPressed()
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

    void EndDialogue()
    {
        dialogueActive = false;
        isTyping = false;
        audioSource.Stop();
        dialogueBox.SetActive(false);
        currentLines = null;

        PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
        if (interaction != null)
            interaction.EndInteraction();
    }
}