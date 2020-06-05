using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILayoutItem
{
    void OnLoad();
    void OnBackgroundDeactivation();
    void OnReactivationFromBackground();
    void OnUnload();
}