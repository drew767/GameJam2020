using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ESpawnItemType
{
    Enemy1,
    Enemy2,
    Enemy3,
    Enemy4,
    Enemy5,
    Bullet,
}

public class ObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class PoolItem
    {
        public ESpawnItemType m_id;
        public GameObject m_prefab;
        public int m_defaultReserv;
        public int m_currentNumberOfInstances;
        public int m_requestedNumberOfInstances;
    }

    [SerializeField]
    List<PoolItem> m_prefabs;

    void Start()
    {
        m_active = new Dictionary<ESpawnItemType, List<GameObject>>();
        m_deactivated = new Dictionary<ESpawnItemType, List<GameObject>>();

        for (int i = 0; i < m_prefabs.Count; i++)
        {
            m_prefabs[i].m_currentNumberOfInstances = 0;
            m_prefabs[i].m_requestedNumberOfInstances = m_prefabs[i].m_defaultReserv;
        }
    }

    void Update()
    {
        for (int i = 0; i < m_prefabs.Count; i++)
        {
            List<GameObject> listOfTypedObjects = m_deactivated[m_prefabs[i].m_id];

            if (m_prefabs[i].m_currentNumberOfInstances >= m_prefabs[i].m_requestedNumberOfInstances &&
                listOfTypedObjects.Count == 0)
            {
                m_prefabs[i].m_requestedNumberOfInstances *= 2;
            }

            if (m_prefabs[i].m_currentNumberOfInstances < m_prefabs[i].m_requestedNumberOfInstances)
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
            m_active[type].Add(newGO);
        }

        return newGO;
    }

    public void DestroyObject(ESpawnItemType type, GameObject objectToDestroy)
    {
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
                GameObject newGO = Instantiate(m_prefabs[i].m_prefab);
                newGO.SetActive(false);
                m_deactivated[type].Add(newGO);
                m_prefabs[i].m_currentNumberOfInstances++;
                return;
            }
        }
    }

    Dictionary<ESpawnItemType, List<GameObject>> m_active;
    Dictionary<ESpawnItemType, List<GameObject>> m_deactivated;
}
