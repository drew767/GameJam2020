using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public Portal portalRef;
    public float m_timeSincePlayerInside;
    public bool m_playerIsInside = false;
    public float m_timeNeedToWaitToDestroy;

    void Update()
    {
        if (m_playerIsInside)
        {
            m_timeSincePlayerInside += Time.deltaTime;
        }
        else if (m_timeSincePlayerInside > 0)
        {
            m_timeSincePlayerInside -= Time.deltaTime;
        }

        if (m_timeSincePlayerInside > m_timeNeedToWaitToDestroy)
        {
            GameManager.GetInstance().DestroyObject(portalRef.gameObject);
        }
    }

    public float GetProgress()
    {
        return m_timeSincePlayerInside / m_timeNeedToWaitToDestroy;
    }

    private void OnTriggerEnter(Collider other)
    {
        EventSystem.SendEvent(new OnPortalBeginTrigger() { portalTrigger = this });
        m_playerIsInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        EventSystem.SendEvent(new OnPortalEndTrigger() { });
        m_playerIsInside = false;
    }
}
