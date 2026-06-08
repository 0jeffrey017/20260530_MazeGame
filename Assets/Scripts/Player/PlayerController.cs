using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float playerSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 100.0f;
        [SerializeField] private float rotationDeadzone = 0.2f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravityValue = -9.81f;

        public CharacterController controller;
        private Vector3 _playerVelocity;
        private bool _groundedPlayer;

        [Header("Input Actions")] 
        public InputActionReference moveAction;
        public InputActionReference jumpAction;
        

        private void Awake()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            controller.enabled = false;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                controller.enabled = true;
        
                moveAction.action.Enable();
                jumpAction.action.Enable();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && moveAction != null && jumpAction != null)
            {
                moveAction.action.Disable();
                jumpAction.action.Disable();
            }
        }

        private void Update()
        {   
            if (!IsOwner) return;
            
            if (!controller.enabled) return;
            
            _groundedPlayer = controller.isGrounded;

            if (_groundedPlayer && _playerVelocity.y < 0)
            {
                _playerVelocity.y = -2f;
            }

            // 1. Read input
            Vector2 input = moveAction.action.ReadValue<Vector2>();

            // 2. Handle Rotation
            if (Mathf.Abs(input.x) > rotationDeadzone)
            {
                float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0f, rotationAmount, 0f);
            }

            // 3. Handle Movement Direction
            Vector3 localMove = new Vector3(0, 0, input.y);
            localMove = Vector3.ClampMagnitude(localMove, 1f);
            Vector3 worldMove = transform.TransformDirection(localMove);

            // 4. Jump Logic
            if (_groundedPlayer && jumpAction.action.WasPressedThisFrame())
            {
                _playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            }

            // 5. Apply gravity
            _playerVelocity.y += gravityValue * Time.deltaTime;

            // 6. Combine movement speed and gravity
            Vector3 horizontalMove = worldMove * playerSpeed;
            Vector3 finalFrameMovement = horizontalMove + Vector3.up * _playerVelocity.y;
            
            controller.Move(finalFrameMovement * Time.deltaTime);
        }
    }
}