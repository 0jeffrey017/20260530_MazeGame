using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace User.Jeffrey.Scripts.Player
{
    public class PlayerCameraFollower : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                BindLocalCamera();
            }
        }

        private void BindLocalCamera()
        {
            var vCam = FindFirstObjectByType<CinemachineCamera>();

            if (vCam != null)
            {
                vCam.Follow = this.transform;
                vCam.LookAt = this.transform;
                
                Debug.Log($"[Camera] 成功將本機相機綁定至 Local Player: {gameObject.name}");
            }
            else
            {
                Debug.LogError("[Camera] 場景中找不到 CinemachineCamera！");
            }
        }
    }
}