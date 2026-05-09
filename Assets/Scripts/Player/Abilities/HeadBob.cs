using UnityEngine;

/// <summary>
/// Покачивание камеры при ходьбе/беге (локальная позиция относительно точки покоя).
/// Вешается на камеру. После PlayerLook — использует LateUpdate.
/// </summary>
[DefaultExecutionOrder(90)]
public class HeadBob : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Player player;

    [Header("Walk Bob")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobAmountX = 0.05f;
    [SerializeField] private float walkBobAmountY = 0.05f;

    [Header("Sprint Bob (Shift)")]
    [SerializeField] private float sprintBobSpeed = 19f;
    [SerializeField] private float sprintBobAmountX = 0.16f;
    [SerializeField] private float sprintBobAmountY = 0.18f;
    [Tooltip("Дополнительный множитель амплитуды только при беге.")]
    [SerializeField] private float sprintBobIntensity = 1.35f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private float transitionSpeed = 5f;
    [Tooltip("Ускорить переход именно в режим бега (сильнее отличие от ходьбы).")]
    [SerializeField] private float sprintTransitionBoost = 1.65f;

    private Vector3 initialPosition;
    private float bobTimer;
    private float currentSpeed;
    private float currentAmountX;
    private float currentAmountY;
    private float targetSpeed;
    private float targetAmountX;
    private float targetAmountY;

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<Player>();

        if (player != null)
        {
            if (inputManager == null)
                inputManager = player.Input;
            if (movement == null)
                movement = player.Movement;
        }

        if (inputManager == null)
            inputManager = GetComponentInParent<InputManager>();

        if (movement == null)
            movement = GetComponentInParent<PlayerMovement>();
    }

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (movement == null || inputManager == null)
            return;

        bool isMoving = movement.IsGrounded && inputManager.MoveAxis.magnitude > 0.1f;

        if (!isMoving)
        {
            targetSpeed = 0f;
            targetAmountX = 0f;
            targetAmountY = 0f;
        }
        else if (inputManager.SprintHeld)
        {
            targetSpeed = sprintBobSpeed;
            targetAmountX = sprintBobAmountX * sprintBobIntensity;
            targetAmountY = sprintBobAmountY * sprintBobIntensity;
        }
        else
        {
            targetSpeed = walkBobSpeed;
            targetAmountX = walkBobAmountX;
            targetAmountY = walkBobAmountY;
        }

        float tMul = inputManager.SprintHeld && isMoving ? sprintTransitionBoost : 1f;
        float t = Time.deltaTime * transitionSpeed * tMul;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, t);
        currentAmountX = Mathf.Lerp(currentAmountX, targetAmountX, t);
        currentAmountY = Mathf.Lerp(currentAmountY, targetAmountY, t);

        if (currentSpeed > 0.01f)
        {
            bobTimer += Time.deltaTime * currentSpeed;

            float offsetX = Mathf.Cos(bobTimer * 0.5f) * currentAmountX;
            float offsetY = Mathf.Sin(bobTimer) * currentAmountY;

            Vector3 targetPosition = initialPosition + new Vector3(offsetX, offsetY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * smoothSpeed);
        }
    }

    /// <summary>Сброс камеры в начальную позицию (телепорт, кат-сцены).</summary>
    public void ResetPosition()
    {
        bobTimer = 0f;
        currentSpeed = 0f;
        currentAmountX = 0f;
        currentAmountY = 0f;
        transform.localPosition = initialPosition;
    }
}
