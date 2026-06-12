using Network;
using UI;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class Door : NetworkBehaviour, IInteractable
{   
    private NetworkObjectSpawner _networkObjectSpawner;
    private GameMode _gameMode;
    private GameClearView _gameClearView;
    
    [Inject]
    public void Construct(NetworkObjectSpawner spawner, 
        GameMode gameMode,
        GameClearView  clearView)
    {
        _networkObjectSpawner = spawner;
        _gameMode = gameMode;
        _gameClearView = clearView;
    }
    public void Interact(ulong id)
    {   
        if (IsServer)
        {
            ProcessOpenDoor(id);
        }
        else
        {
            RequestOpenDoorServerRpc(id);
        }
    }
    
    [Rpc(SendTo.Server)]
    private void RequestOpenDoorServerRpc(ulong id)
    {
        ProcessOpenDoor(id);
    }
    
    private void ProcessOpenDoor(ulong id)
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
        if (_gameMode.CurrentHaveKeyStatus)
        {
            RequestGameClearRpc(id);
        }
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void RequestGameClearRpc(ulong id)
    {
        if (_gameClearView != null)//TODO VContainer inject fail
        {
            _gameClearView.SetGameClearUI(NetworkManager.LocalClientId == id);
        }
    }
}