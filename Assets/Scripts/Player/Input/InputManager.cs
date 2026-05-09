using UnityEngine;

/// <summary>
/// Единая точка опроса ввода через классический Input Manager (Edit → Project Settings → Input Manager).
/// </summary>
[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    [Header("Movement axes (имена из Input Manager)")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    [Header("Look (мышь)")]
    [SerializeField] private string mouseXAxis = "Mouse X";
    [SerializeField] private string mouseYAxis = "Mouse Y";

    [Header("Клавиши")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    /// <summary>Плоскость движения: x — стрейф, z — вперёд (для CharacterController обычно x/y в Vector2 как x/z).</summary>
    public Vector2 MoveAxis { get; private set; }

    /// <summary>Дельта мыши за кадр (как Input.GetAxis Mouse X/Y).</summary>
    public Vector2 LookDelta { get; private set; }

    public bool SprintHeld { get; private set; }
    public bool InteractPressed { get; private set; }

    private void Update()
    {
        MoveAxis = new Vector2(
            Input.GetAxisRaw(horizontalAxis),
            Input.GetAxisRaw(verticalAxis));

        if (MoveAxis.sqrMagnitude > 1f)
            MoveAxis.Normalize();

        LookDelta = new Vector2(
            Input.GetAxis(mouseXAxis),
            Input.GetAxis(mouseYAxis));

        SprintHeld = Input.GetKey(sprintKey);
        InteractPressed = Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0);
    }
}
