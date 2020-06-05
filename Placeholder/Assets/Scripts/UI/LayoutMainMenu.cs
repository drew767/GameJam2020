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
        // for now we are just POPing current screen. but later we need to request GAME MANAGER START GAME
        LayoutManager.GetInstance().ClearLayoutStack();
        LayoutManager.GetInstance().PushLayout(ELayoutId.Hud);
    }
}
