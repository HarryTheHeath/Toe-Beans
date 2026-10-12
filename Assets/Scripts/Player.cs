using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    private CharacterController _characterController;
    private Camera _camera;
    private Vector3 _velocity;
    private float _xRotation;
    private bool _isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();

        if (_characterController == null)
        {
            _characterController = gameObject.AddComponent<CharacterController>();
            _characterController.height = playerHeight;
        }

        if (_camera == null)
        {
            GameObject cameraObj = new GameObject("Camera");
            cameraObj.transform.SetParent(transform);
            cameraObj.transform.localPosition = new Vector3(0, playerHeight / 2, 0);
            _camera = cameraObj.AddComponent<Camera>();
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleMovement();
        HandleLook();
    }

    private void HandleInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * moveZ + transform.right * moveX;
        moveDirection = moveDirection.normalized;

        _characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _velocity.y = jumpForce;
        }
    }

    private void HandleGroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0;
        }
    }

    private void HandleMovement()
    {
        _velocity.y -= 9.81f * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);

        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}