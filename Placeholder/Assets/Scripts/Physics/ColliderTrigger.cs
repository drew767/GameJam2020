using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public bool IsTriggered { get; private set; }

    List<Collider> m_touchedColliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        IsTriggered = true;
        m_touchedColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        m_touchedColliders.Remove(other);
        m_touchedColliders.Remove(null);
        IsTriggered = m_touchedColliders.Count > 0;
    }

}
