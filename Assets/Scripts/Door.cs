using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (GlobalFlag.CanDoorOpen.CurrentValue)
        {
            Debug.Log("Door is open");
            Debug.Log("Game Clear");
        }
    }
}