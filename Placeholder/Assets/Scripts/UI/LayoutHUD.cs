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
        UpdatePortalProgress();
    }

    PortalTrigger m_lastTriggeredPortal;

    private void OnPortalDesstroyedEvent(object incomingEvent)
    {
        m_lastTriggeredPortal = null;
        // show message that portal closed
        HidePortalProgress();
    }
    private void OnBeginTriggerPortal(object incomingEvent) 
    {
        m_lastTriggeredPortal = ((OnPortalBeginTrigger)incomingEvent).portalTrigger;
        ShowPortalProgress();
    }
    private void OnEndTriggerPortal(object incomingEvent) 
    {
        m_lastTriggeredPortal = null;
        HidePortalProgress();
    }

    private void HidePortalProgress()
    {

    }

    private void ShowPortalProgress()
    {

    }

    private void UpdatePortalProgress()
    {
        if (m_lastTriggeredPortal)
        {
            // update it
        }
    }
}
