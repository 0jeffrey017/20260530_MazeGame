using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerColor : NetworkBehaviour
{   
    private Material _material;
    
    private readonly NetworkVariable<Color> _playerColor = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public override void OnNetworkSpawn()
    {   
        _playerColor.OnValueChanged += OnColorChanged;
        
        UpdateMaterialColor(_playerColor.Value);
        
        if (IsServer)
        {
            ulong ownerId = OwnerClientId; 
            _playerColor.Value = GetColorFromPlayerId(ownerId);
        }
    }
    
    private void OnColorChanged(Color previousValue, Color newValue)
    {
        UpdateMaterialColor(newValue);
    }

    private void UpdateMaterialColor(Color color)
    {
        if (_material != null)
        {
            _material.color = color;
        }
    }

    private Color GetColorFromPlayerId(ulong playerId)
    {
        return playerId switch
        {   
            0 => Color.red,
            1 => Color.blue,
            2 => Color.green,
            3 => Color.yellow,
            _ => Color.white
        };
    }
    
    public override void OnNetworkDespawn()
    {
        _playerColor.OnValueChanged -= OnColorChanged;
    }
}