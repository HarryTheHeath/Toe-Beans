using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
            playerInput = gameObject.AddComponent<PlayerInput>();

        var playerMap = playerInput.actions.FindActionMap("Player");
        moveAction = playerMap.FindAction("Move");
        lookAction = playerMap.FindAction("Look");
        jumpAction = playerMap.FindAction("Jump");


    }

    private void Update()
    {

        HandleMovement();
        HandleJump();
    }
    private void LateUpdate()
    {
        HandleLook();
    }


    private void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = transform.forward * input.y + transform.right * input.x;
        move = Vector3.ProjectOnPlane(move, Vector3.up).normalized;

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        xRotation -= look.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * look.x * mouseSensitivity);
    }

    private void HandleJump()
    {
        if (jumpAction.triggered && controller.isGrounded)
            velocity.y = jumpForce;
    }
}