using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    public string[] dialogues;
    public AudioClip[] audioClips;
    public TMP_Text subtitleText;
    public GameObject interactPrompt;

    private AudioSource audioSource;
    private bool playerNearby = false;
    private bool isTalking = false;
    private int dialogueIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactPrompt.SetActive(false);
        subtitleText.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !isTalking)
        {
            isTalking = true;
            dialogueIndex = 0;

            interactPrompt.SetActive(false); // Hide "Press E" prompt
            subtitleText.transform.parent.gameObject.SetActive(true);
            PlayDialogueSequence();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (!isTalking)
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            // ❗ STOP everything when player leaves
            CancelInvoke("PlayNextLine");
            audioSource.Stop();

            interactPrompt.SetActive(false);
            subtitleText.transform.parent.gameObject.SetActive(false);

            isTalking = false;
            dialogueIndex = 0;
        }
    }

    void PlayDialogueSequence()
    {
        if (dialogueIndex >= dialogues.Length)
        {
            subtitleText.transform.parent.gameObject.SetActive(false);
            isTalking = false;

            // Show "Press E" prompt again only if player is still nearby
            if (playerNearby)
                interactPrompt.SetActive(true);
            return;
        }

        subtitleText.text = dialogues[dialogueIndex];
        audioSource.clip = audioClips[dialogueIndex];
        audioSource.Play();

        Invoke("PlayNextLine", audioSource.clip.length + 0.5f); // 0.5s pause
    }

    void PlayNextLine()
    {
        dialogueIndex++;
        PlayDialogueSequence();
    }
}
