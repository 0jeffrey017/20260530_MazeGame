using System;
using Unity.Netcode;
using UnityEngine;

public class playerHitBox : NetworkBehaviour
{   
    public readonly NetworkVariable<ulong> OwnedClientId = new(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            OwnedClientId.Value = OwnerClientId; 
            Debug.Log($"[Server] {NetworkObjectId} assign Client: {OwnerClientId}");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact(OwnedClientId.Value);
        }
    }
}
