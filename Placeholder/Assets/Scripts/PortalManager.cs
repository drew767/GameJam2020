using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    List<Transform> m_deactivatedSpawns;
    List<Transform> m_activatedSpawns;

    public GameObject m_portalPrefab;

    // Start is called before the first frame update
    void Start()
    {
        m_deactivatedSpawns = new List<Transform>();
        m_activatedSpawns = new List<Transform>();

        // GET ALL PORTAL SPAWN POINTS
        for (int i = 0; i < transform.childCount; i++)
        {
            m_deactivatedSpawns.Add(transform.GetChild(i));
            Debug.Log(transform.GetChild(i).localPosition + " " + transform.GetChild(i).position);

        }

        EventSystem.RegisterListener<OnPortalDesstroyedEvent>(OnPortalDestroyed);
    }

    void Update()
    {
        SpawnPortal();
    }

    void SpawnPortal()
    {
        if (m_deactivatedSpawns.Count > 0)
        {
            int randomIndex = Random.Range(0, m_deactivatedSpawns.Count);
            Transform currentSpawn = m_deactivatedSpawns[randomIndex];
            m_activatedSpawns.Add(currentSpawn);
            m_deactivatedSpawns.Remove(currentSpawn);

            Instantiate(m_portalPrefab, currentSpawn.position, Quaternion.identity);
        }
    }

    private void OnPortalDestroyed(object incomingEvent) 
    {
        OnPortalDesstroyedEvent castedEvent = (OnPortalDesstroyedEvent)incomingEvent;

        int nearestIndex = 0;
        float minimalDistance = 100000000.0f;

        for (int i = 0; i < m_activatedSpawns.Count; i++)
        {
            float distance = Vector3.Distance(castedEvent.position, m_activatedSpawns[i].position);

            if (minimalDistance > distance)
            {
                minimalDistance = distance;
                nearestIndex = i;
            }
        }

        Transform destroyedSpawn = m_deactivatedSpawns[nearestIndex];
        m_deactivatedSpawns.Add(destroyedSpawn);
        m_activatedSpawns.Remove(destroyedSpawn);
    }

}
