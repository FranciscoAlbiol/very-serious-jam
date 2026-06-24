using UnityEngine;
using UnityEngine.InputSystem;

public class HandScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 5f;
    public float maxScrollOffset = 10f; 

    [Header("Mouse Trigger Zones")]
    [Range(0f, 0.5f)] public float edgeThreshold = 0.15f;

    public int minimum_cards; 

    private float currentOffset = 0f;

    void Update()
    {
        if (transform.childCount >= minimum_cards) 
        {
            if (Mouse.current == null) return;

            float mouseX = Mouse.current.position.ReadValue().x;
            float mouseXRatio = mouseX / Screen.width;

            float moveDirection = 0f;

            if (mouseXRatio > (1f - edgeThreshold))
            {
                moveDirection = -1f;
            }
            else if (mouseXRatio < edgeThreshold)
            {
                moveDirection = 1f;
            }

            if (moveDirection != 0f)
            {
                // Calculate how much we want to move this frame
                float movement = moveDirection * scrollSpeed * Time.deltaTime;

                // Check if this movement would exceed our clamps
                float targetOffset = Mathf.Clamp(currentOffset + movement, -maxScrollOffset, maxScrollOffset);
                float actualMovement = targetOffset - currentOffset;

                // Move cleanly along the object's own horizontal right vector in world space to avoid skewing
                transform.position += transform.right * actualMovement;

                // Update tracked offset
                currentOffset = targetOffset;
            }
        }
    }
}