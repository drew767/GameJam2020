using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    List<Transform> m_deactivatedSpawns;
    List<Transform> m_activatedSpawns;

    // Start is called before the first frame update
    void Start()
    {
        m_deactivatedSpawns = new List<Transform>();
        m_activatedSpawns = new List<Transform>();

        // GET ALL PORTAL SPAWN POINTS
        for (int i = 0; i < transform.childCount; i++)
        {
            m_deactivatedSpawns.Add(transform.GetChild(i));
        }

        EventSystem.RegisterListener<OnPortalDesstroyedEvent>(OnPortalDestroyed);

        m_timeSinceLastSpawn = m_timeBetweenSpawnPortal / 1.1f;
    }

    void Update()
    {
        if (m_timeSinceLastSpawn >= m_timeBetweenSpawnPortal && GameManager.GetInstance().GetIsGameTicking())
        {
            SpawnPortal();
            m_timeSinceLastSpawn = 0.0f;
        }
        else if (GameManager.GetInstance().GetIsGameTicking())
        {
            m_timeSinceLastSpawn += Time.deltaTime;
        }
    }

    void SpawnPortal()
    {
        if (m_deactivatedSpawns.Count > 0)
        {
            int randomIndex = Random.Range(0, m_deactivatedSpawns.Count);
            Transform currentSpawn = m_deactivatedSpawns[randomIndex];
            m_activatedSpawns.Add(currentSpawn);
            m_deactivatedSpawns.Remove(currentSpawn);

            GameObject portal = GameManager.GetInstance().GetNewObject(ESpawnItemType.Portal);
            portal.SetActive(true);
            portal.transform.position = currentSpawn.position;
            portal.transform.rotation = Quaternion.identity;
        }
    }

    float m_timeSinceLastSpawn;
    public float m_timeBetweenSpawnPortal = 33.0f;

    private void OnPortalDestroyed(object incomingEvent) 
    {
        if (m_activatedSpawns != null && m_activatedSpawns.Count <= 0)
        {
            return;
        }

        OnPortalDesstroyedEvent castedEvent = (OnPortalDesstroyedEvent)incomingEvent;

        int nearestIndex = 0;
        float minimalDistance = 100000000.0f;

        for (int i = 0; i < m_activatedSpawns.Count; i++)
        {
            if (m_activatedSpawns[i] == null)
            {
                //Debug.LogError("Transform should not be NULL but if this end of the game it is Ok. LOL MOTHERFUCKER");
                continue;
            }

            float distance = Vector3.Distance(castedEvent.position, m_activatedSpawns[i].position);

            if (minimalDistance > distance)
            {
                minimalDistance = distance;
                nearestIndex = i;
            }
        }

        Transform destroyedSpawn = m_activatedSpawns[nearestIndex];
        m_deactivatedSpawns.Add(destroyedSpawn);
        m_activatedSpawns.Remove(destroyedSpawn);
    }

}
