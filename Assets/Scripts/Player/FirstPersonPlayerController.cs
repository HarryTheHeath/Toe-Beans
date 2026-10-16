using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
    private InputAction pauseAction;

    InputAction interactAction;
    private IInteractable currentInteractable;

    [Header("UI")]
    [SerializeField] public CanvasGroup promptObject;
    public TextMeshProUGUI promptTagTMP;
    public TextMeshProUGUI promptActionTMP;
    
    public bool isFrozen = false;
    
    // Equipping pole
    [SerializeField] public Yraiser Yraiser;
    [SerializeField] public GameObject flagPole;
    [SerializeField] public GameObject flagPrefab;

    private GameObject equipped;
    private InputAction equipAction;
    private bool poleEquipped;
    private bool paused;
    private bool canLook = true;
    [SerializeField] public CanvasGroup pause;

    [Header("Footsteps")]
    [SerializeField] private LadyLyla ladyLyla;
    [Header("Footsteps")]
    [SerializeField] private float footstepBaseInterval = 0.45f;
    [SerializeField] private float footstepMoveThreshold = 0.1f;

    private float footstepTimer = 0f;



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
        equipAction = playerMap.FindAction("EquipPole");
        pauseAction = playerMap.FindAction("Pause");

        promptTagTMP.text = string.Empty;
        promptActionTMP.text = string.Empty;

        promptObject.alpha = 0f;
    }


    private void Update()
    {
        if (!isFrozen)
        { 
            HandleMovement();
            HandleJump();
        }
        HandleInteract();
        HandlePause();
        HandleEquipPole();
        if (poleEquipped)
            HandlePlantPole();
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

        float moveMagnitude = new Vector2(input.x, input.y).magnitude; // 0..1
        UpdateFootsteps(moveMagnitude);

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (!canLook && isFrozen) return;

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
            promptObject.alpha = 0;
            currentInteractable?.Interact();
        }
    }

    public void HandlePause()
    {
        if (pauseAction.triggered)
        {
            paused = !paused;
            if (paused)
            {
                Freeze(false);
                pause.alpha = 1;
            }
            else
            {
                Unfreeze();
                pause.alpha = 0;
            }
        }
    }

    public void ClearInteractable(IInteractable interact)
    {
        if (currentInteractable == interact)
            currentInteractable = null;
    }
    
    public void HandleEquipPole() {
        if (equipAction.triggered && controller.isGrounded)
        {
            if (!flagPole.activeSelf)
            {
                flagPole.SetActive(true);
                promptTagTMP.text = Yraiser.getUiPromptTag();
                promptActionTMP.text = Yraiser.getUiPromptActionNotPlanted();
                promptObject.alpha = 1;
                poleEquipped = true;
            }
            else
            {
                deactivatePole();
            }
            
        }
    }

    private void deactivatePole()
    {
        flagPole.SetActive(false);
        promptTagTMP.text = "";
        promptActionTMP.text = "";
        promptObject.alpha = 0;
        poleEquipped = false;
    }

    public void HandlePlantPole()
    {
        if (interactAction.triggered && controller.isGrounded)
        {
            Vector3 pos = transform.position + transform.forward * 2f - Vector3.up * 0.2f;
            GameObject plantedPole = Instantiate(flagPrefab, pos, Quaternion.identity);
            var flag = plantedPole.GetComponent<Yraiser>();
            flag.SetPlayer(this);
            StartCoroutine(WaitToRaiseFlag(flag));
            deactivatePole(); // equipped pole
        }
    }

    private IEnumerator WaitToRaiseFlag(Yraiser flag)
    {
        yield return new WaitForSeconds(0.5f);
        flag.Interact();
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
    
    // Freeze and unfreeze player movement
    
    public void Freeze(bool canLookAround)
    {
        canLook = canLookAround;
        isFrozen = true;
    }
    
    public void Unfreeze()
    {
        canLook = true;
        isFrozen = false;
    }

    public bool TeleportTo(Vector3 worldPosition, Quaternion? worldRotation = null, bool clearVelocity = true)
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        Vector3 target = worldPosition;


        controller.enabled = false;

        transform.position = target;
        if (worldRotation.HasValue) transform.rotation = worldRotation.Value;

        controller.enabled = true;

        if (clearVelocity)
            velocity = Vector3.zero;

        var cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            float angle = cam.transform.localEulerAngles.x;
            if (angle > 180f) angle -= 360f;
            xRotation = Mathf.Clamp(angle, -maxLookAngle, maxLookAngle);
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        Unfreeze();
        return true;
    }

    public bool TeleportTo(Transform targetTransform, bool clearVelocity = true)
    {
        if (targetTransform == null) return false;
        return TeleportTo(targetTransform.position, targetTransform.rotation, clearVelocity);
    }

    private bool IsCapsuleOverlappingAt(Vector3 worldPosition)
    {
        if (controller == null) controller = GetComponent<CharacterController>();

        Vector3 worldCenter = worldPosition + transform.rotation * controller.center;
        float halfHeight = Mathf.Max(0f, (controller.height * 0.5f) - controller.radius);
        Vector3 bottom = worldCenter + Vector3.down * halfHeight;
        Vector3 top = worldCenter + Vector3.up * halfHeight;

        return Physics.CheckCapsule(bottom, top, controller.radius, ~0, QueryTriggerInteraction.Ignore);
    }

    private void UpdateFootsteps(float moveMagnitude)
    {
        if (ladyLyla == null) return;

        // conditions: not frozen, grounded, and moving past threshold
        if (isFrozen || !controller.isGrounded || moveMagnitude <= footstepMoveThreshold)
        {
            // reset timer so first step plays immediately when starting again
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            // speedFactor adjusts cadence by input magnitude
            float speedFactor = Mathf.Clamp(moveMagnitude, 0.5f, 1.5f);

            // Try to play a footstep on LadyLyla's sfx source. If it played, advance timer by interval.
            bool played = ladyLyla.TryPlayFootstep();
            if (played)
            {
                footstepTimer = footstepBaseInterval / speedFactor;
            }
            else
            {
                // sfx source busy — retry shortly
                footstepTimer = 0.05f;
            }
        }
    }
}