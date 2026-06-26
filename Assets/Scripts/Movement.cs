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
    private bool inputEnabled = true;

    [Header("Audio Clip")]
    public AudioSource walkingAudioSource;
    public AudioClip walk_soundClip;

    void Start()
    {
        if (walkingAudioSource == null) {
            walkingAudioSource = gameObject.AddComponent<AudioSource>();
        }

        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        if (!inputEnabled) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (!inputEnabled) return;
        lookInput = value.Get<Vector2>();
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
        }
    }

    void Update()
    {
        if (inputEnabled)
        {
            yaw += lookInput.x * mouseSensitivity;
            pitch -= lookInput.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            lookInput = Vector2.zero;

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

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

        if (controller.isGrounded && moveInput.sqrMagnitude > 0.01f)
        {
            if (!walkingAudioSource.isPlaying)
            {
                walkingAudioSource.clip = walk_soundClip;
                walkingAudioSource.loop = true;
                walkingAudioSource.Play();
            }
        }
        else
        {
            if (walkingAudioSource.isPlaying)
            {
                walkingAudioSource.Stop();
            }
        }
    }
}