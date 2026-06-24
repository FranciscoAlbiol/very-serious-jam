using UnityEngine;
using UnityEngine.InputSystem;

public class HandScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 5f;
    public float maxScrollOffset = 10f; // How far left/right it can slide

    [Header("Mouse Trigger Zones")]
    [Range(0f, 0.5f)] public float edgeThreshold = 0.15f;

    public int minimum_cards; //number of cards that have be on screen before scrolling is allowed

    void Update()
    {
        if (transform.childCount >= minimum_cards) {
            if (Mouse.current == null) return;

            float mouseX = Mouse.current.position.ReadValue().x;
            float mouseXRatio = mouseX / Screen.width;

            float moveDirection = 0f;

            // Mouse is near the right edge -> Move cards LEFT
            if (mouseXRatio > (1f - edgeThreshold))
            {
                moveDirection = -0.1f;
            }

            // Mouse is near the left edge -> Move cards RIGHT
            else if (mouseXRatio < edgeThreshold)
            {
                moveDirection = 0.1f;
            }

            if (moveDirection != 0f)
            {
                float newX = transform.localPosition.y + (moveDirection * scrollSpeed * Time.deltaTime);
                newX = Mathf.Clamp(-maxScrollOffset, newX, maxScrollOffset);

                transform.localPosition = new Vector3(transform.localPosition.x, newX, transform.localPosition.z);
            }
        
        }
    }
}