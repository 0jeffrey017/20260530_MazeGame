using System;
using Unity.Netcode;
using UnityEngine;

public class GameMode : NetworkBehaviour
{
    public readonly NetworkVariable<bool> GlobalHaveKey = new(false);
    
    public event Action<bool, bool> OnHaveKeyChanged;
    
    public bool CurrentHaveKeyStatus => GlobalHaveKey.Value;

    public override void OnNetworkSpawn()
    {
        GlobalHaveKey.OnValueChanged += HandleKeyChanged;
    }

    public override void OnNetworkDespawn()
    {
        GlobalHaveKey.OnValueChanged -= HandleKeyChanged;
    }

    public void KeyChanged(bool value)
    {
        GlobalHaveKey.Value = value;
    }

    private void HandleKeyChanged(bool previousValue, bool newValue)
    {
        OnHaveKeyChanged?.Invoke(previousValue, newValue);
    }
}