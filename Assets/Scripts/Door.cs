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
    public void Interact()
    {
        if (_gameMode.HaveKey.Value)
        {
            Debug.Log("Door Interact");
        }
    }
}