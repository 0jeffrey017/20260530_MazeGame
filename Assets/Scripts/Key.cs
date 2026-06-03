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
        Debug.Log($"[Key] 透過 VContainer 成功注入！");
    }
    
    public override void OnNetworkSpawn()
    {
        _rotationAnimation = LMotion.Create(0.0f, 180.0f, 2.0f)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(v => transform.rotation = Quaternion.Euler(0, v, 0));
    }

    public void Interact()
    {
        if (IsServer)
        {
            ProcessPickup();
        }
        else
        {
            RequestPickupServerRpc();
        }
    }
    
    [Rpc(SendTo.Server)]
    private void RequestPickupServerRpc()
    {
        ProcessPickup();
    }

    // 伺服器核心處理邏輯
    private void ProcessPickup()
    {   
        if (_gameMode == null)
        {
            Debug.LogError($"[Key Error] Server端的 _gameMode 是 null！物件名稱: {gameObject.name}", this);
            return;
        }

        if (_networkObjectSpawner == null)
        {
            Debug.LogError($"[Key Error] Server端的 _networkObjectSpawner 是 null！物件名稱: {gameObject.name}", this);
            return;
        }
        if (_gameMode != null)
        {
            _gameMode.SetHaveKey(true);
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