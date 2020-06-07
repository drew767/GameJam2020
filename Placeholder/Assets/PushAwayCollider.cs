using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAwayCollider : MonoBehaviour
{
    [SerializeField]
    float radius;

    [SerializeField]
    float force;

    Rigidbody m_rigidbody;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    float RandomAngle()
    {
        return Random.Range(0, 360);
    }

    Quaternion RandomOrientation()
    {
        return Quaternion.Euler(RandomAngle(), RandomAngle(), RandomAngle());
    }

    List<Vector3> m_knownPoints = new List<Vector3>();
    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, RandomOrientation() * Vector3.forward);
        if(Physics.Raycast(ray, out hit, radius, ~0))
        {
            m_knownPoints.Add(hit.point);
            if(m_knownPoints.Count > 50)
               m_knownPoints.RemoveAt(0);
        }

        if (m_knownPoints.Count > 0)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < m_knownPoints.Count; ++i)
            {
                Vector3 dif = transform.position - m_knownPoints[i];
                sum += dif.normalized * (1f / Mathf.Max(dif.sqrMagnitude, 1f));
            }

            sum = sum * (1f / m_knownPoints.Count);

            m_rigidbody.velocity = m_rigidbody.velocity + sum * force;
        }
    }
}
