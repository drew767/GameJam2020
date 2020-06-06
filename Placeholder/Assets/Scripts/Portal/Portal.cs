using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPooledObject
{
    #region IPooledObject
    public void OnSpawned() 
    {
        m_timeBetweenSpawn = 1 / m_minMobsInSecond;
        m_timeSinceLastSpawn = 0.0f;
        m_timeSinceOpened = 0.0f;
    }
    public void OnDestroy() 
    {
        EventSystem.SendEvent(new OnPortalDesstroyedEvent() { position = transform.position });
    }
    public ESpawnItemType GetObjectType() { return ESpawnItemType.Portal; }
    #endregion

    public Transform m_spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_minMobsInSecond < m_maxMobsInSecond, "Spawn rate is incorrect");
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceOpened += Time.deltaTime;
        m_timeSinceLastSpawn += Time.deltaTime;
        float rateFraction = Mathf.Clamp(m_timeSinceOpened, 0.0f, m_spawnPickWillBeReachedAfterSecondsFromSpawn) / m_spawnPickWillBeReachedAfterSecondsFromSpawn;
        float currentMobsRate = m_minMobsInSecond + (rateFraction * (m_maxMobsInSecond - m_minMobsInSecond));
        m_timeBetweenSpawn = 1 / currentMobsRate;

        if (m_timeSinceLastSpawn >= m_timeBetweenSpawn)
        {
            SpawnMob();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_spawnPoint.position, 1);
    }

    public float m_spawnPickWillBeReachedAfterSecondsFromSpawn = 1000.0f;
    public float m_minMobsInSecond = 0.1f;
    public float m_maxMobsInSecond = 5.0f;
    float m_timeBetweenSpawn;
    float m_timeSinceLastSpawn;
    float m_timeSinceOpened;

    void SpawnMob()
    {
        float chance = Random.Range(0.0f, 100.0f);
        ESpawnItemType type = ESpawnItemType.SlowEnemy;
        
        if (chance <= m_mobSlowChance)
        {
            type = ESpawnItemType.SlowEnemy;
        }
        else if (chance <= m_mobSlowChance + m_mobSpeedChance)
        {
            type = ESpawnItemType.SpeedEnemy;
        }

        GameObject mob = GameManager.GetInstance().GetNewObject(type);
        mob.SetActive(true);

        mob.transform.position = m_spawnPoint.position;
        mob.transform.rotation = Quaternion.identity;

        m_timeSinceLastSpawn = 0.0f;
    }

    public float m_mobSlowChance = 50.0f;
    public float m_mobSpeedChance = 50.0f;
}
