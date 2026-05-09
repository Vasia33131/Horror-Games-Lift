using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private InputManager input;

    [Header("Sensitivity")]
    [SerializeField] private float horizontalSensitivity = 120f;
    [SerializeField] private float verticalSensitivity = 120f;
    [Tooltip("Включите для инверсии вертикали.")]
    [SerializeField] private bool invertVertical;

    [Header("Pitch Limits")]
    [SerializeField] private float minPitch = -89f;
    [SerializeField] private float maxPitch = 89f;

    [Header("Smoothing")]
    [SerializeField] private bool enableSmoothing;
    [SerializeField] private float smoothing = 12f;

    private Transform playerBody;
    private float pitchRotation;
    private Vector2 smoothedDelta;
    private Vector2 smoothVelocity;

    private void Awake()
    {
        playerBody = GetComponentInParent<Player>()?.transform
                      ?? GetComponentInParent<CharacterController>()?.transform;

        if (input == null && player != null)
            input = player.Input;

        if (input == null)
            input = GetComponentInParent<InputManager>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (playerBody == null || input == null)
            return;

        Vector2 lookDelta = input.LookDelta;

        float vertMultiplier = invertVertical ? -1f : 1f;
        Vector2 scaled = new Vector2(
            lookDelta.x * horizontalSensitivity * Time.deltaTime,
            lookDelta.y * verticalSensitivity * Time.deltaTime * vertMultiplier);

        Vector2 applied = scaled;

        if (enableSmoothing)
        {
            smoothedDelta = Vector2.SmoothDamp(smoothedDelta, scaled, ref smoothVelocity, 1f / Mathf.Max(smoothing, 0.01f));
            applied = smoothedDelta;
        }

        playerBody.Rotate(Vector3.up * applied.x);

        pitchRotation -= applied.y;
        pitchRotation = Mathf.Clamp(pitchRotation, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitchRotation, 0f, 0f);
    }
}
