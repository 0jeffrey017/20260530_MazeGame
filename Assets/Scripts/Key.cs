using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GlobalFlag.HaveKey.Value = true;
    }
}