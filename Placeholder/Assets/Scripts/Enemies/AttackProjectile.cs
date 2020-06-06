using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackProjectile : MonoBehaviour, IPooledObject
{
    #region IPooledObject
    public void OnSpawned()
    {
	}
    public void OnDestroy()
    {
    }
    public ESpawnItemType GetObjectType() { return m_type; }
	#endregion

	public ESpawnItemType m_type = ESpawnItemType.ProjectileBlackHole;

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
	Rigidbody m_rb = null;

    public void Setup()
    {
		if (m_rb == null)
		{
            m_rb = GetComponent<Rigidbody>();
            m_rb.useGravity = !m_gravityModifier.Equals(0.0f);
        }

		m_startPosition = transform.position;
		m_timeSinceSpawn = 0;
		if (m_vfx)
		{
			GameObject.Instantiate(m_vfx, transform);
		}

		//TODO: apply gravity for direction
        Vector3 direction = AttackTarget - m_startPosition;
        m_rb.velocity = direction.normalized * m_speed;
	}

	public float m_lifeTime = 15.0f;
	float m_timeSinceSpawn = 0;

    void Update()
    {
		m_timeSinceSpawn += Time.deltaTime;

		if (((m_startPosition - transform.position).magnitude >= m_maxDistance) ||
			m_lifeTime < m_timeSinceSpawn)
		{
			DestroyProjectile();
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag.Equals("Player"))
		{
			other.gameObject.SendMessage("OnTakeDamage", m_damage);
			DestroyProjectile();
		}
		else if(m_vfx != null && !other.tag.Equals("Enemy"))
		{
			DestroyProjectile();
		}
	}

	void DestroyProjectile()
	{
		if (m_destroyVfx)
		{
			GameObject.Instantiate(m_destroyVfx, transform.position, Quaternion.identity);
		}

		GameManager.GetInstance().DestroyObject(gameObject);
	}
}
