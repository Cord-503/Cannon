using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Vector2 _currentMouseLook;
    private Vector2 _smoothedMouseDelta;
    private Vector2 _initialCameraRotation;
    private Vector2 _initialCharacterRotation;
    private PlayerInputActions _inputActions;

    [Header("Rotation Settings")]
    public Vector2 ClampInDegrees = new Vector2(360f, 180f);
    public Vector2 Sensitivity = new Vector2(0.1f, 0.1f);
    public Vector2 Smoothing = new Vector2(1f, 1f);

    [Header("Cursor Settings")]
    public bool LockCursor = true;
    public GameObject CharacterBody;

    private void OnEnable()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
    }

    private void Start()
    {
        // Initialize the target
        _initialCameraRotation = transform.localRotation.eulerAngles;

        if (CharacterBody)
        {
            _initialCharacterRotation = CharacterBody.transform.localRotation.eulerAngles;
        }

    }

    private void LateUpdate()
    {
        HandleCursorLock();
        Vector2 mouseDelta = _inputActions.pActionMap.MouseLook.ReadValue<Vector2>();
        _currentMouseLook += ScaleAndSmoothMouseDelta(mouseDelta);

        ClampMouseLook();
        AlignCameraWithBody();
    }

    private Vector2 ScaleAndSmoothMouseDelta(Vector2 delta)
    {
        // Apply scaling
        delta = Vector2.Scale(delta, new Vector2(Sensitivity.x * Smoothing.x, Sensitivity.y * Smoothing.y));

        _smoothedMouseDelta.x = Mathf.Lerp(_smoothedMouseDelta.x, delta.x, 1f / Smoothing.x);
        _smoothedMouseDelta.y = Mathf.Lerp(_smoothedMouseDelta.y, delta.y, 1f / Smoothing.y);

        return _smoothedMouseDelta;
    }

    private void ClampMouseLook()
    {
        if (ClampInDegrees.x < 360f)
        {
            _currentMouseLook.x = Mathf.Clamp(_currentMouseLook.x, -ClampInDegrees.x * 0.5f, ClampInDegrees.x * 0.5f);
        }

        if (ClampInDegrees.y < 360f)
        {
            _currentMouseLook.y = Mathf.Clamp(_currentMouseLook.y, -ClampInDegrees.y * 0.5f, ClampInDegrees.y * 0.5f);
        }

        Quaternion targetOrientation = Quaternion.Euler(_initialCameraRotation);
        transform.localRotation = Quaternion.AngleAxis(-_currentMouseLook.y, targetOrientation * Vector3.right) * targetOrientation;
    }

    private void AlignCameraWithBody()
    {
        Quaternion targetCharacterOrientation = Quaternion.Euler(_initialCharacterRotation);
        Quaternion yRotation = Quaternion.identity;

        if (CharacterBody)
        {
            yRotation = Quaternion.AngleAxis(_currentMouseLook.x, Vector3.up);
            CharacterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            yRotation = Quaternion.AngleAxis(_currentMouseLook.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }

    private void HandleCursorLock()
    {
        if (LockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
