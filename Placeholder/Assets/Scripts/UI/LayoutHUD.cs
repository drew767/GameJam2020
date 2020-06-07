using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UI;

public class LayoutHUD : MonoBehaviour, ILayoutItem
{
    #region ILayuoutItem
    public void OnLoad() { }
    public void OnBackgroundDeactivation() { }
    public void OnReactivationFromBackground() { }
    public void OnUnload() { }
    #endregion

    public TextMeshProUGUI m_timer;
    public TextMeshProUGUI m_bullets;
    public TextMeshProUGUI m_kills;
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.RegisterListener<OnPortalEndTrigger>(OnEndTriggerPortal);
        EventSystem.RegisterListener<OnPortalBeginTrigger>(OnBeginTriggerPortal);
        EventSystem.RegisterListener<OnPortalDesstroyedEvent>(OnPortalDesstroyedEvent);
    }

    // Update is called once per frame
    void Update()
    {
        m_timer.text = GameManager.GetInstance().GetGameTimeInString();
        UpdatePortalProgress();
        UpdateWeaponStats();
        UpdatePlayerHealth();
        UpdateKills();
    }

    public Slider portalDestruction;
    public Slider health;

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

    public GameObject progressBarPortal;
    private void HidePortalProgress()
    {
        if (progressBarPortal == null)
        {
            return;
        }

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
            portalDestruction.value = m_lastTriggeredPortal.GetProgress();
        }
    }

    void UpdatePlayerHealth()
    {
        health.value = GameManager.GetInstance().Player.GetComponent<PlayerController>().health / 100.0f;
    }

    void UpdateWeaponStats()
    {
        WeaponInventory wi = GameManager.GetInstance().Player.gameObject.GetComponent<WeaponInventory>();
        int total = 0;
        int inTheGun = 0;

        if (wi.currentGunScript == null)
        {
            return;
        }

        if (wi.currentGunScript.endlessAmmo)
        {
            total = 9999;
            inTheGun = 11;
        }
        else
        {
            total = (int)wi.weaponBulletsIHave;
            inTheGun = (int)wi.currentGunScript.GetBylletsInTheGun2();
        }
        m_bullets.text = inTheGun.ToString() + " / " + total.ToString();
    }

    private void UpdateKills()
    {
        m_kills.text = GameManager.GetInstance().kills.ToString();
    }

}
