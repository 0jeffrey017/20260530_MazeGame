using Network;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerInit : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3[] spawnPoints;
        private NetworkObjectSpawner _spawner;
        
        [Inject]
        public void Construct(NetworkObjectSpawner spawner)
        {
            _spawner = spawner;
        }
        private void Start()
        {   
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnLocalClientConnected;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnLocalClientConnected;
            }
        }

        private void OnLocalClientConnected(ulong clientId)
        {
            Debug.Log($"[Client] 我連線成功了！我的 ID 是 {clientId}，請求伺服器幫我生角色。");
            OnClientConnected(clientId);
        }
        
        private void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            Debug.Log($"[Server 管理器] 偵測到 Client {clientId} 已通車，開始為其規劃出生點...");
            
            Vector3 spawnPosition = Vector3.zero;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)];
            }
            
            _spawner.SpawnNetworkObjectWithOwnership(playerPrefab, clientId, position:spawnPosition);
        }
    }
}
