using System;

public class StartUpStatePresenter
{
    public event Action OnReboot = ActionUtility.EmptyAction.Instance;

    public void Reboot()
    {
        OnReboot.Invoke();
    }
}