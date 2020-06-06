using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutMainMenu : MonoBehaviour, ILayoutItem
{
    #region ILayuoutItem
    public void OnLoad() { }
    public void OnBackgroundDeactivation() { }
    public void OnReactivationFromBackground() { }
    public void OnUnload() { }
    #endregion

    public void ExitGame()
    {
        Application.Quit();
    }
    public void PlayGame()
    {
        LayoutManager.GetInstance().ClearLayoutStack();
        LayoutManager.GetInstance().PushLayout(ELayoutId.Hud);
        GameManager.GetInstance().StartPlayingGame();
    }
}
