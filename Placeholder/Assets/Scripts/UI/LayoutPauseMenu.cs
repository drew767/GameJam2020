using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutPauseMenu : MonoBehaviour, ILayoutItem
{
    #region ILayuoutItem
    public void OnLoad()
    {
        // TODO PAUSE GAME
    }
    public void OnBackgroundDeactivation() { }
    public void OnReactivationFromBackground() { }
    public void OnUnload() { }
    #endregion

    public void ExitGame()
    {
        LayoutManager.GetInstance().ClearLayoutStack();
        LayoutManager.GetInstance().PushLayout(ELayoutId.MainMenu);
    }
    public void ContinueGame()
    {
        // TODO UNPAUSE GAME
        LayoutManager.GetInstance().PopLayout();
    }
}
