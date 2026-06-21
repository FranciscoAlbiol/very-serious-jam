using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 0.1f;

    private CharacterController controller;
    private Transform cameraTransform;
    private float pitch = 0f;
    private float yaw = 0f;
    private float yVelocity = 0f;

    private Vector2 moveInput;
    private Vector2 lookInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        lookInput = Vector2.zero;

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = right * moveInput.x + forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);

        if (controller.isGrounded)
            yVelocity = -0.5f;
        else
            yVelocity += Physics.gravity.y * Time.deltaTime;

        move.y = yVelocity;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}