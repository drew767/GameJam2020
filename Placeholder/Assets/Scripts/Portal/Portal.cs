using System;
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

    public float m_spawnPickWillBeReachedAfterSecondsFromSpawn = 1000.0f;
    public float m_minMobsInSecond = 0.1f;
    public float m_maxMobsInSecond = 5.0f;
    float m_timeBetweenSpawn;
    float m_timeSinceLastSpawn;
    float m_timeSinceOpened;

    [Serializable]
    public class SpawnOjbectInfo
	{
        public ESpawnItemType m_type = ESpawnItemType.SlowEnemy;
        public float m_weight = 50.0f;
    }

    [SerializeField]
    List<SpawnOjbectInfo> m_spawnObjects = new List<SpawnOjbectInfo>();

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

    void SpawnMob()
    {
        float weight = 0.0f;
        foreach(var spawnObject in m_spawnObjects)
		{
            weight += spawnObject.m_weight;
		}

        float chance = UnityEngine.Random.Range(0.0f, weight);
        weight = 0.0f;
        ESpawnItemType type = ESpawnItemType.SlowEnemy;
        foreach (var spawnObject in m_spawnObjects)
		{
            weight += spawnObject.m_weight;
            if(chance <= weight)
			{
                type = spawnObject.m_type;
                break;
			}
		}

        GameObject mob = GameManager.GetInstance().GetNewObject(type);
        mob.SetActive(true);

        mob.transform.position = m_spawnPoint.position;
        mob.transform.rotation = Quaternion.identity;

        m_timeSinceLastSpawn = 0.0f;
    }
}
