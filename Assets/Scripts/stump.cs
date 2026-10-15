using UnityEngine;
using UnityEngine.InputSystem;

public class stump : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera stumpCamera;
    [SerializeField] private Camera playerCamera;

    [Header("UI")]
    [SerializeField] private string uiPromptTag;
    [SerializeField] private string uiPromptAction;

    [Header("Input")]
    private PlayerInput playerInput;
    private InputAction lookAction;
    [Header("Look")][SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;
    private float xRotation;
    private Quaternion camRot;

    private bool sitting = false;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)

            playerInput = gameObject.AddComponent<PlayerInput>();
        var playerMap = playerInput.actions.FindActionMap("Player");
        lookAction = playerMap.FindAction("Look");

        camRot = stumpCamera.transform.localRotation;

    }
    public void Interact()
    {

        playerObject.TryGetComponent<FirstPersonPlayerController>(out FirstPersonPlayerController player);
        if (sitting)
        {
            player.Unfreeze();
            sitting = false;
            stumpCamera.enabled = false;
            playerCamera.enabled = true;
            uiPromptTag = "stump";
            uiPromptAction = "sit";
        }
        else
        {
            // freeze player
            player.Freeze(false);
            // activate new camera looking from the stump
            stumpCamera.enabled = true;
            playerCamera.enabled = false;
            sitting = true;
            // change prompt to "get up"
            player.promptTagTMP.text = "";
            player.promptActionTMP.text = "get up";
            player.promptObject.alpha = 1;
        }

    }

    public bool hasTriggered()
    {
        return false;
    }

    public string promptAction()
    {
        return uiPromptAction;
    }

    public string promptTag()
    {
        return uiPromptTag;
    }

    private void LateUpdate()
    {
        if (!sitting) return;

        Vector2 look = lookAction.ReadValue<Vector2>();

        xRotation -= look.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        
        stumpCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //stumpCamera.transform.Rotate(Vector3.up * look.x * mouseSensitivity);
    }
}