using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ESpawnItemType
{
    Portal,
    SlowEnemy,
    SpeedEnemy,
    RangeEnemy,
    Enemy4,
    Enemy5,
    ProjectileBlackHole,
    MeleeAttack
}

public class ObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class PoolItem
    {
        public void SetupPoolitem()
        {
            m_currentNumberOfInstances = 0;
            m_requestedNumberOfInstances = m_defaultReserv;
        }

        public ESpawnItemType m_id;
        public GameObject m_prefab;
        public int m_defaultReserv;
        int m_currentNumberOfInstances;
        public void IncrementCurrentNumber()
        {
            ++m_currentNumberOfInstances;
        }
        int m_requestedNumberOfInstances;
        public void IncreaseRequestedNumberOfInstances()
        {
            m_requestedNumberOfInstances *= 2;
        }

        public int GetRequestedNumber() { return m_requestedNumberOfInstances; }
        public int GetCurrentNumber() { return m_currentNumberOfInstances; }
    }

    [SerializeField]
    List<PoolItem> m_prefabs = new List<PoolItem>();

    void Start()
    {
        m_active = new Dictionary<ESpawnItemType, List<GameObject>>();
        m_deactivated = new Dictionary<ESpawnItemType, List<GameObject>>();

        for (int i = 0; i < m_prefabs.Count; i++)
        {
            m_prefabs[i].SetupPoolitem();
            m_deactivated.Add(m_prefabs[i].m_id, new List<GameObject>());
            m_active.Add(m_prefabs[i].m_id, new List<GameObject>());
        }
    }

    void Update()
    {
        for (int i = 0; i < m_prefabs.Count; i++)
        {
            List<GameObject> listOfTypedObjects = m_deactivated[m_prefabs[i].m_id];

            if (m_prefabs[i].GetCurrentNumber() >= m_prefabs[i].GetRequestedNumber() &&
                listOfTypedObjects.Count == 0)
            {
                m_prefabs[i].IncreaseRequestedNumberOfInstances();
            }

            if (m_prefabs[i].GetCurrentNumber() < m_prefabs[i].GetRequestedNumber())
            {
                SpawnNewObjectInternal(m_prefabs[i].m_id);
            }
        }
    }

    public GameObject GetNewObject(ESpawnItemType type)
    {
        GameObject newGO = null;

        if (m_deactivated.ContainsKey(type) == false)
        {
            throw new Exception("There is no such prefab");
        }
        else
        {
            var instances = m_deactivated[type];
            if (instances.Count == 0)
            {
                SpawnNewObjectInternal(type);
            }

            newGO = instances[0];
            newGO.SetActive(true);
            instances.RemoveAt(0);
            IPooledObject pooledObject = newGO.GetComponent<IPooledObject>();
            pooledObject.OnSpawned();
            m_active[type].Add(newGO);
        }

        return newGO;
    }

    public void DestroyObject(GameObject objectToDestroy)
    {
        IPooledObject pooledObject = objectToDestroy.GetComponent<IPooledObject>();

        if (pooledObject == null)
        {
            throw new Exception("Pool doesn't support this kind of objects");
        }

        ESpawnItemType type = pooledObject.GetObjectType();

        if (m_active.ContainsKey(type) == false)
        {
            throw new Exception("Pool doesn't manage objects of this type");
        }

        var instances = m_active[type];

        int index = -1;
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] == objectToDestroy)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            throw new Exception("Pool doesn't manage this object so it can not be destroyed");
        }
        pooledObject.OnDestroy();
        objectToDestroy.SetActive(false);
        m_active[type].RemoveAt(index);
        m_deactivated[type].Add(objectToDestroy);
    }

    void SpawnNewObjectInternal(ESpawnItemType type)
    {
        for (int i = 0; i < m_prefabs.Count; i++)
        {
            if (m_prefabs[i].m_id == type)
            {
                // GameObject newGO = Instantiate(m_prefabs[i].m_prefab, new Vector3(-100, -100, -100), Quaternion.identity);
                GameObject newGO = Instantiate(m_prefabs[i].m_prefab);
                newGO.SetActive(false);
                m_deactivated[type].Add(newGO);
                m_prefabs[i].IncrementCurrentNumber();


                IPooledObject pooledObject = newGO.GetComponent<IPooledObject>();
                if (pooledObject == null)
                {
                    throw new Exception("Pool doesn't support this kind of objects");
                }

                return;
            }
        }
    }

    public void DeactivateAllPools()
    {
        if (m_active.Count == 0)
        {
            return;
        }

        foreach (var list in m_active)
        {
            while (list.Value.Count > 0)
            {
                DestroyObject(list.Value[0]);
            }
        }
    }

    Dictionary<ESpawnItemType, List<GameObject>> m_active;
    Dictionary<ESpawnItemType, List<GameObject>> m_deactivated;
}
