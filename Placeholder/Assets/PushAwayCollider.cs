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

    class FixedQueue<T>
    {
        private int m_capacity;
        private int m_size;
        private int m_tailIndex;
        private T[] m_data;
        public FixedQueue(int maxSize)
        {
            m_capacity = maxSize;
            m_data = new T[m_capacity];
            m_size = 0;
            m_tailIndex = 0;
        }

        public void Add(T value)
        {
            m_data[m_tailIndex] = value;
            m_tailIndex = (m_tailIndex + 1) % m_capacity;
            if (m_size < m_capacity)
                m_size++;
        }

        public int Count { get { return m_size; } }

        private int modifyIndex(int i)
        {
            return (m_tailIndex + i) % m_size;
        }

        public T this[int i]
        {
            get { return m_data[modifyIndex(i)]; }
            set { m_data[modifyIndex(i)] = value; }
        }
    }


    FixedQueue<Vector3> m_knownPoints = new FixedQueue<Vector3>(30);
    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, RandomOrientation() * Vector3.forward);
        if(Physics.Raycast(ray, out hit, radius, ~0))
        {
            m_knownPoints.Add(hit.point);
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
