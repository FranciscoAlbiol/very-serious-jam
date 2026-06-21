using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 5f;
    public Camera playerCamera;
    public float transitionDuration = 1f;

    private Interactable currentHovered;
    private Interactable currentActive;
    private bool isInteracting = false;
    private bool isTransitioning = false;

    public RawImage fadeImage;

    void Start()
    {
        
    }

    void Update()
    {
        if (isTransitioning) return;

        if (isInteracting)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                StartCoroutine(TransitionCamera(currentActive.interactableCamera, playerCamera, false));
            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        bool didHit = Physics.Raycast(ray, out hit, interactRange);

        Interactable hovered = didHit ? hit.collider.GetComponentInParent<Interactable>() : null;

        if (hovered != currentHovered)
        {
            if (currentHovered != null) currentHovered.SetHighlight(false);
            currentHovered = hovered;
            if (currentHovered != null) currentHovered.SetHighlight(true);
        }

        if (currentHovered != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentHovered.interactableCamera == null) return;
            currentActive = currentHovered;
            currentActive.SetHighlight(false);
            currentHovered = null;
            StartCoroutine(TransitionCamera(playerCamera, currentActive.interactableCamera, true));
        }
    }

    IEnumerator TransitionCamera(Camera from, Camera to, bool enteringInteraction)
    {
        isTransitioning = true;
        float half = transitionDuration / 2f;

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t / half);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);

        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);

        if (enteringInteraction)
        {
            isInteracting = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            isInteracting = false;
            currentActive = null;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        for (float t = 0; t < half; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, 1 - t / half);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);

        isTransitioning = false;
    }
}