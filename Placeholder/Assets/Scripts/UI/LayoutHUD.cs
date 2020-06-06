using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LayoutHUD : MonoBehaviour, ILayoutItem
{
    #region ILayuoutItem
    public void OnLoad() { }
    public void OnBackgroundDeactivation() { }
    public void OnReactivationFromBackground() { }
    public void OnUnload() { }
    #endregion

    public TextMeshProUGUI m_timer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_timer.text = GameManager.GetInstance().GetGameTimeInString();
    }
}
