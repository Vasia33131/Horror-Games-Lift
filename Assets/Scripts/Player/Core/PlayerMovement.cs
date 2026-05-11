using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private InputManager input;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;

    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;

    [Header("Gravity")]
    [SerializeField] private float gravity = -15f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => controller?.velocity ?? Vector3.zero;

    /// <summary>Скорость по горизонтали — для анимаций камеры (head bob).</summary>
    public float HorizontalSpeed
    {
        get
        {
            if (controller == null)
                return 0f;
            Vector3 v = controller.velocity;
            v.y = 0f;
            return v.magnitude;
        }
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (input == null && player != null)
            input = player.Input;

        if (input == null)
            input = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (controller == null || input == null)
            return;

        isGrounded = CheckGrounded();

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector2 move = input.MoveAxis;
        Vector3 moveDir = right * move.x + forward * move.y;
        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        float speed = input.SprintHeld ? sprintSpeed : walkSpeed;
        controller.Move(moveDir * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool CheckGrounded()
    {
        if (groundCheck != null &&
            Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore))
            return true;

        return controller.isGrounded;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}