using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutGameEndScreen : MonoBehaviour, ILayoutItem
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
    public void ToMainMenu()
    {
        LayoutManager.GetInstance().ClearLayoutStack();
        LayoutManager.GetInstance().PushLayout(ELayoutId.MainMenu);
    }
    public void Restart()
    {
        GameManager.GetInstance().StartPlayingGame();
        LayoutManager.GetInstance().ClearLayoutStack();
        LayoutManager.GetInstance().PushLayout(ELayoutId.Hud);
    }
}
