using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Player
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
                
                Debug.Log($"[Camera] Local Player: {gameObject.name}");
            }
            else
            {
                Debug.LogError("[Camera] Can't Find CinemachineCamera！");
            }
        }
    }
}