using LitMotion;
using Network;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class Key : NetworkBehaviour, IInteractable
{   
    private NetworkObjectSpawner _networkObjectSpawner;
    private GameMode _gameMode;
    private MotionHandle _rotationAnimation;
    
    [Inject]
    public void Construct(NetworkObjectSpawner spawner,GameMode gameMode)
    {
        _networkObjectSpawner = spawner;
        _gameMode = gameMode;
    }
    
    public override void OnNetworkSpawn()
    {
        _rotationAnimation = LMotion.Create(0.0f, 180.0f, 2.0f)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(v => transform.rotation = Quaternion.Euler(0, v, 0));
    }

    public void Interact(ulong id)
    {   
        if (IsServer)
        {
            ProcessPickup(id);
        }
        else
        {
            RequestPickupServerRpc(id);
        }
    }
    
    [Rpc(SendTo.Server)]
    private void RequestPickupServerRpc(ulong id)
    {
        ProcessPickup(id);
    }
    
    private void ProcessPickup(ulong id)
    {   
        if (_gameMode == null)
        {
            Debug.LogError($"[Key Error] Server _gameMode is null！{gameObject.name}", this);
            return;
        }
        if (_networkObjectSpawner == null)
        {
            Debug.LogError($"[Key Error] Server _networkObjectSpawner is null！ {gameObject.name}", this);
            return;
        }
        if (_gameMode != null)
        {
            _gameMode.KeyChanged(true);
        }
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            _networkObjectSpawner.DespawnNetworkObject(gameObject);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        if (_rotationAnimation.IsActive())
        {
            _rotationAnimation.Cancel();
        }
    }
}