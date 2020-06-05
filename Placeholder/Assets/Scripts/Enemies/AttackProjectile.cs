using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour
{
	[SerializeField]
	int m_damage = 10;
	[SerializeField]
	float m_speed = 100.0f;
	[SerializeField]
	float m_maxDistance = 3.0f;
	[SerializeField]
	float m_gravityModifier = 0.0f;
	[SerializeField]
	GameObject m_vfx = null;
	[SerializeField]
	GameObject m_destroyVfx = null;

	public Vector3 AttackTarget { set; get; }

	Vector3 m_startPosition = Vector3.zero;

    void Start()
    {
		m_startPosition = transform.position;
		if(m_vfx)
		{
			GameObject.Instantiate(m_vfx, transform);
		}

		Rigidbody rb = GetComponent<Rigidbody>();
		rb.useGravity = !m_gravityModifier.Equals(0.0f);

		//TODO: apply gravity for direction
		Vector3 direction = AttackTarget - m_startPosition;
		rb.velocity = direction.normalized * m_speed;
	}

    void Update()
    {
        if((m_startPosition - transform.position).magnitude >= m_maxDistance)
		{
			if(m_destroyVfx)
			{
				GameObject.Instantiate(m_destroyVfx, transform.position, Quaternion.identity);
			}

			Destroy(this.gameObject);
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			other.gameObject.SendMessage("OnTakeDamage", m_damage);
		}
	}
}
