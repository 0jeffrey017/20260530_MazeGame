using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace Network
{
    public class NetworkObjectSpawner : MonoBehaviour
    {   
        private IObjectResolver _container;

        [Inject]
        public void Construct(IObjectResolver container)
        {
            _container = container;
        }

        public GameObject SpawnNetworkObject(GameObject obj,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
        {
            
            GameObject instance = parent == null ? Instantiate(obj) : Instantiate(obj, parent);
            instance.transform.position = parent == null ? position : parent.position + position;
            instance.transform.rotation = rotation == default ? Quaternion.identity : rotation;
            instance.transform.localScale = scale == default ? Vector3.one : scale;
            
            if(instance.TryGetComponent<Key>(out var key))
            {
                 _container.Inject(key);
            }
            if(instance.TryGetComponent<Door>(out var door))
            {
                _container.Inject(door);
            }
           

            var netObj = instance.GetComponent<NetworkObject>();
            if(netObj != null)
            {
                netObj.Spawn(true); 
            }

            return instance;
        }
        
        public GameObject SpawnNetworkObjectWithOwnership(GameObject obj,
            ulong ownerId,
            Transform parent = null,
            Vector3 position = default,
            Quaternion rotation = default,
            Vector3 scale = default)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) 
                return null;
            
            GameObject instance = parent == null ? Instantiate(obj) : Instantiate(obj, parent);
            instance.transform.position = parent == null ? position : parent.position + position;
            instance.transform.rotation = rotation == default ? Quaternion.identity : rotation;
            instance.transform.localScale = scale == default ? Vector3.one : scale;
            
            _container.Inject(instance);
            
            var netObj = instance.GetComponent<NetworkObject>();
            if(netObj != null)
            {
                netObj.SpawnWithOwnership(ownerId,true); 
            }

            return instance;
        }
        
        public void DespawnNetworkObject(GameObject obj)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (obj == null) return;
            
            var netObj = obj.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(destroy: true);
            }
        }
    }
}
