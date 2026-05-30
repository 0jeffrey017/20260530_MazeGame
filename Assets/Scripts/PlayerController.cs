using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 100.0f; // Added for controlled turning
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;

    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Input Actions")] 
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            // Keeps the player snapped to the ground without accumulating massive downward velocity
            playerVelocity.y = -2f;
        }

        // 1. Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 2. Handle Rotation (Turn Left/Right based on X input)
        if (input.x != 0)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }

        // 3. Handle Movement Direction (Forward/Backward based on Y input)
        // We create a local forward vector and convert it to world space
        Vector3 localMove = new Vector3(0, 0, input.y);
        localMove = Vector3.ClampMagnitude(localMove, 1f);
        Vector3 worldMove = transform.TransformDirection(localMove);

        // 4. Jump Logic
        if (groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // 5. Apply gravity over time
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        // 6. Combine movement speed and gravity, then move the controller
        Vector3 finalFrameMovement = (worldMove * playerSpeed) + (Vector3.up * playerVelocity.y);
        controller.Move(finalFrameMovement * Time.deltaTime);
    }
}