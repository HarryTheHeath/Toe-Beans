using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerController : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Look")] [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    InputAction interactAction;
    private IInteractable currentInteractable;

    [Header("UI")]
    public CanvasGroup promptObject;
    public TextMeshProUGUI promptTagTMP;
    public TextMeshProUGUI promptActionTMP;


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
        interactAction = playerMap.FindAction("Interact");

        promptTagTMP.text = string.Empty;
        promptActionTMP.text = string.Empty;

        promptObject.alpha = 0f;
    }


    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleInteract();
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

    private void HandleInteract()
    {
        if (interactAction.triggered && controller.isGrounded)
        {
            currentInteractable?.Interact();
            promptObject.alpha = 0;
        }
    }
    
    // Keeping track of the current interactable //
    // Beware: If there are several interactable objects nearby, the most recently collided will be current
    
    // On collision with an object, make it the current interactable
    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            if (!currentInteractable.hasTriggered())
            {
                promptTagTMP.text = currentInteractable.promptTag();
                promptActionTMP.text = currentInteractable.promptAction();
                promptObject.alpha = 1;
            }
        }
    }

    // When leaving the current interactable, set current interactable to null 
    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
            promptTagTMP.text = "";
            promptActionTMP.text = "";
            promptObject.alpha = 0;
        }
    }
}