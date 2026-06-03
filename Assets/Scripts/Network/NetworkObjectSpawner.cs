using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Network
{
    public class NetworkObjectSpawner : NetworkBehaviour
    {   
        private IObjectResolver _container;

        [Inject]
        public void Construct(IObjectResolver container)
        {
            _container = container;
        }
        
        public GameObject SpawnNetworkObject(GameObject obj,
            Transform parent,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
        {
            if (!IsServer) return null;
            
            GameObject instance = Instantiate(obj, parent);
            instance.transform.position = parent.position + position;
            instance.transform.rotation = rotation == default ? Quaternion.identity : rotation;
            instance.transform.localScale = scale == default ? Vector3.one : scale;
            
            _container.Inject(instance);
            
            NetworkObject netObj = instance.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn(true); 
            }

            return instance;
        }
        
        public void DespawnNetworkObject(GameObject obj)
        {
            if (!IsServer) return;
            if (obj == null) return;
            
            var netObj = obj.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(destroy: true);
            }
        }
    }
}
