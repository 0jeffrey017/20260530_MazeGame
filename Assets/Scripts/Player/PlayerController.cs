using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float rotationDeadzone = 0.2f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private Vector3 StartPosition = Vector3.one * 5;

    public CharacterController controller;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;

    [Header("Input Actions")] 
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            this.transform.position = StartPosition;
            this.transform.position += Vector3.up;
        }
    }
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

    private void Update()
    {   
        if(!IsOwner)return;
        _groundedPlayer = controller.isGrounded;

        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            // Keeps the player snapped to the ground without accumulating massive downward velocity
            _playerVelocity.y = -2f;
        }

        // 1. Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // 2. Handle Rotation (Turn Left/Right based on X input)
        if (Mathf.Abs(input.x) > rotationDeadzone)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }

        // 3. Handle Movement Direction (Forward/Backward based on Y input)
        // We create a local forward vector and convert it to world space
        Vector3 localMove = new Vector3(0, 0, input.y);
        localMove = Vector3.ClampMagnitude(localMove, 1f);
        Vector3 worldMove = transform.TransformDirection(localMove);

        // 4. Jump Logic
        if (_groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            _playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // 5. Apply gravity over time
        _playerVelocity.y += gravityValue * Time.deltaTime;

        // 6. Prevent tunneling through walls: check horizontal movement with a capsule cast
        Vector3 horizontalMove = worldMove * playerSpeed;
        Vector3 horizontalDisplacement = horizontalMove * Time.deltaTime;

        if (horizontalDisplacement.sqrMagnitude > 0.000001f)
        {
            // Compute capsule endpoints in world space (based on the CharacterController)
            Vector3 capsuleCenter = transform.position + controller.center;
            float capsuleHeight = Mathf.Max(2f * controller.radius, controller.height);
            Vector3 point1 = capsuleCenter + Vector3.up * (capsuleHeight / 2f - controller.radius);
            Vector3 point2 = capsuleCenter + Vector3.up * (-capsuleHeight / 2f + controller.radius);
            float checkDistance = horizontalDisplacement.magnitude + 0.01f;

            if (Physics.CapsuleCast(point1, point2, controller.radius, horizontalDisplacement.normalized, out RaycastHit hit, checkDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                // Obstacle detected ahead — cancel horizontal movement to avoid passing through walls.
                horizontalMove = Vector3.zero;
            }
        }

        // 7. Combine movement speed and gravity, then move the controller
        Vector3 finalFrameMovement = horizontalMove + Vector3.up * _playerVelocity.y;
        controller.Move(finalFrameMovement * Time.deltaTime);
    }
}