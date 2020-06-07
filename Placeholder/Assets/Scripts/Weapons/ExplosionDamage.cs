using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField]
    float m_explosionRadius = 1.0f;
    [SerializeField]
    int m_explosionDamage = 5;

    void Start()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_explosionRadius);   
        foreach(var collider in hitColliders)
		{
            if(collider.gameObject.tag.Equals("Enemy"))
			{
                collider.gameObject.SendMessage("TakeDamage", m_explosionDamage);
			}
		}
    }
}
