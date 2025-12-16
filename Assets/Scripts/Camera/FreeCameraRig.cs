using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCameraRig : MonoBehaviour
{
    [Header("Movement Buttons (Arrow keys)")]
    [SerializeField] private InputAction forward; // UpArrow
    [SerializeField] private InputAction back;    // DownArrow
    [SerializeField] private InputAction left;    // LeftArrow
    [SerializeField] private InputAction right;   // RightArrow

    [Header("Mouse Look")]
    [SerializeField] private InputAction look; // <Mouse>/delta (Value/Vector2)
    [SerializeField] private Transform cameraTransform;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float lookSensitivity = 0.08f;
    [SerializeField] private float lookSmoothing = 10f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private InputAction toggleCursor; // Button, e.g. Escape

    private float pitch;
    private Vector2 smoothDelta;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        forward.Enable();
        back.Enable();
        left.Enable();
        right.Enable();
        look.Enable();
        toggleCursor.Enable();
    }

    void OnDisable()
    {
        forward.Disable();
        back.Disable();
        left.Disable();
        right.Disable();
        look.Disable();
        toggleCursor.Disable();
    }

    void Start()
    {
        if (lockCursorOnStart)
            LockCursor();
    }

    void FixedUpdate()
    {
        // Optional: only allow movement when cursor is locked (comment out if you want movement always)
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector3 dir = Vector3.zero;

        // Movement ONLY from arrow key actions
        if (forward.IsPressed()) dir += transform.forward;
        if (back.IsPressed()) dir -= transform.forward;
        if (left.IsPressed()) dir -= transform.right;
        if (right.IsPressed()) dir += transform.right;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        Vector3 nextPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    void Update()
    {
        // Toggle cursor lock (Escape)
        if (toggleCursor.WasPressedThisFrame())
        {
            if (Cursor.lockState == CursorLockMode.Locked) UnlockCursor();
            else LockCursor();
        }

        // Mouse look ONLY when cursor is locked
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (cameraTransform == null)
            return;

        // Rotation ONLY from mouse delta
        Vector2 raw = look.ReadValue<Vector2>();
        smoothDelta = Vector2.Lerp(smoothDelta, raw, Time.deltaTime * lookSmoothing);

        float yawAdd = smoothDelta.x * lookSensitivity;
        float pitchAdd = smoothDelta.y * lookSensitivity;

        // Horizontal rotation on the rig
        transform.Rotate(0f, yawAdd, 0f);

        // Vertical rotation on the camera
        pitch -= pitchAdd;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
