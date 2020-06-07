using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;

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
        UpdateWeaponStats();
    }

    PortalTrigger m_lastTriggeredPortal;

    private void OnPortalDesstroyedEvent(object incomingEvent)
    {
        m_lastTriggeredPortal = null;
        // show message that portal closed
        HidePortalProgress();
        UpdatePlayerHealth();
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

    public GameObject progressBarPortal;
    private void HidePortalProgress()
    {
        progressBarPortal.SetActive(false);
    }

    private void ShowPortalProgress()
    {
        progressBarPortal.SetActive(true);
    }

    private void UpdatePortalProgress()
    {
        if (m_lastTriggeredPortal)
        {
            // update it
            float progress = m_lastTriggeredPortal.GetProgress();
        }
    }

    void UpdatePlayerHealth()
    {
        int health = GameManager.GetInstance().Player.GetComponent<PlayerController>().health;
    }

    void UpdateWeaponStats()
    {
        WeaponInventory wi = GameManager.GetInstance().Player.gameObject.GetComponent<WeaponInventory>();
        //wi.weaponBulletsIHave;
        //wi.currentGunScript.GetBylletsInTheGun();
        //wi.currentGunScript.amountOfBulletsPerLoad;
        //wi.currentGunScript.endlessAmmo;
    }
}
