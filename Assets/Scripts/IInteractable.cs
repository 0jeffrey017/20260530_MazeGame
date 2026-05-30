using R3;

public interface IInteractable
{
    void Interact();
}

public static class GlobalFlag
{
    public static ReadOnlyReactiveProperty<bool> CanDoorOpen => HaveKey;
    public static ReactiveProperty<bool> HaveKey = new ReactiveProperty<bool>(false);
}