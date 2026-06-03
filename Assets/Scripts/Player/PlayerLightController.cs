using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace User.Jeffrey.Scripts.Player
{
    public class PlayerLightController : NetworkBehaviour
    {   
        private Light _light;
        
        private readonly NetworkVariable<Quaternion> _lightRotation = new NetworkVariable<Quaternion>(
            Quaternion.identity, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Server
        );
        
        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        public override void OnNetworkSpawn()
        {   
            _lightRotation.OnValueChanged += OnRotationChanged;
            
            _light.transform.rotation = _lightRotation.Value;
        }
        
        private void OnRotationChanged(Quaternion previousValue, Quaternion newValue)
        {
            if (!IsOwner)
            {
                _light.transform.rotation = newValue;
            }
        }

        private void Update()
        {
            if (IsOwner)
            {
                // 本地端繼續即時偵測滑鼠，高反應速率
                Calculate3DFreeLookRotation();
            }
            else
            {
                _light.transform.rotation = Quaternion.Slerp(
                    _light.transform.rotation, 
                    _lightRotation.Value, 
                    Time.deltaTime * 15f 
                );
            }
        }

        private void Calculate3DFreeLookRotation()
        {
            if (Camera.main == null) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            var targetPoint = Physics.Raycast(ray, out RaycastHit hit, 100f) ? hit.point : ray.GetPoint(50f);
            
            Vector3 lookDirection = targetPoint - _light.transform.position;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                
                _light.transform.rotation = targetRotation;
                
                UpdateLightRotationServerRpc(targetRotation);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void UpdateLightRotationServerRpc(Quaternion rotation)
        {
            _lightRotation.Value = rotation;
        }

        public override void OnNetworkDespawn()
        {
            _lightRotation.OnValueChanged -= OnRotationChanged;
        }
    }
}