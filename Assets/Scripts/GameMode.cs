using Unity.Netcode;
using UnityEngine;

public class GameMode : NetworkBehaviour
{
    // 使用 NetworkVariable 同步遊戲狀態（Server 可寫，所有人可讀）
    public readonly NetworkVariable<bool> HaveKey = new NetworkVariable<bool>(
        false, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );
    public override void OnNetworkSpawn()
    {
        HaveKey.OnValueChanged += OnHaveKeyChanged;
    }

    private void OnHaveKeyChanged(bool previousValue, bool newValue)
    {
        Debug.Log($"[GameMode] 鑰匙狀態更新！當前是否有鑰匙: {newValue}");
    }
    
    public void SetHaveKey(bool state)
    {
        if (!IsServer) return; 
        HaveKey.Value = state;
    }

    public override void OnNetworkDespawn()
    {
        HaveKey.OnValueChanged -= OnHaveKeyChanged;
    }
}