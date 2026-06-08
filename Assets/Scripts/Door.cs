using Network;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class Door : NetworkBehaviour, IInteractable
{   
    private NetworkObjectSpawner _networkObjectSpawner;
    private GameMode _gameMode;
    
    [Inject]
    public void Construct(NetworkObjectSpawner spawner,GameMode gameMode)
    {
        _networkObjectSpawner = spawner;
        _gameMode = gameMode;
    }
    public void Interact(ulong id)
    {   
        if (_gameMode == null)//TODO GAMEMODE is NULL
        {
            Debug.LogError($"[Key Error] Server _gameMode is null！{gameObject.name}", this);
            return;
        }
        if (_networkObjectSpawner == null)
        {
            Debug.LogError($"[Key Error] Server _networkObjectSpawner is null！ {gameObject.name}", this);
            return;
        }
        if (_gameMode.CurrentHaveKeyStatus)
        {
            Debug.Log("Door Interact");
        }
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            _networkObjectSpawner.DespawnNetworkObject(gameObject);
        }
    } 
}