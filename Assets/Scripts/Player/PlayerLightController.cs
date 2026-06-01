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
                    Time.deltaTime * 15f // 數值越大跟隨越快
                );
            }
        }

        private void Calculate3DFreeLookRotation()
        {
            if (Camera.main == null) return;

            // 1. 從滑鼠螢幕位置發射一條 3D 射線
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    
            Vector3 targetPoint;

            // 2. 進行 3D 物理射線偵測（投射距離設為 100 公尺）
            // 建議在實際專案中加上 LayerMask，只偵測環境與敵人，避開玩家自己
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // 如果射線打到東西（地板、牆壁、箱子），手電筒就盯著那個點
                targetPoint = hit.point;
            }
            else
            {
                // 如果滑鼠指著外太空（沒打到任何碰撞體），就讓它盯著射線遠方的某個虛擬點
                targetPoint = ray.GetPoint(50f); 
            }

            // 3. 計算 3D 朝向向量（包含 X, Y, Z 三個軸向的完整高度差）
            Vector3 lookDirection = targetPoint - _light.transform.position;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        
                // 本地端搶先旋轉
                _light.transform.rotation = targetRotation;

                // 發送給伺服器同步（使用上一單元的 ServerRpc）
                UpdateLightRotationServerRpc(targetRotation);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void UpdateLightRotationServerRpc(Quaternion rotation)
        {
            // 伺服器收到後改寫數值，Netcode 會自動廣播給全場
            _lightRotation.Value = rotation;
        }

        public override void OnNetworkDespawn()
        {
            _lightRotation.OnValueChanged -= OnRotationChanged;
        }
    }
}