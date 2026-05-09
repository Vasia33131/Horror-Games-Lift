using UnityEngine;

/// <summary>
/// Корень игрока: ссылки на компоненты. Ввод опрашивается в InputManager.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerLook look;
    [SerializeField] private InputManager inputManager;

    public CharacterController CharacterController => characterController;
    public Camera PlayerCamera => playerCamera;
    public PlayerMovement Movement => movement;
    public PlayerLook Look => look;
    public InputManager Input => inputManager;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        if (look == null)
            look = GetComponentInChildren<PlayerLook>();

        if (inputManager == null)
            inputManager = GetComponent<InputManager>();
    }
}
