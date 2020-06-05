using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField]
	int m_healt = 100;
	[SerializeField]
	float m_moveSpeed = 50;
	[SerializeField]
	float m_attackDistance = 1.0f;
	[SerializeField]
	float m_attackCooldown = 1.0f;
	[SerializeField]
	bool m_canMoveWhileAttack = false;
	[SerializeField]
	AttackProjectile m_attackProjectile = null;
	[SerializeField]
	Transform m_attackProjectileInitialTransform = null;


	enum eState
	{
		IDLE,
		MOVE,
		ATTACK,
		DEAD
	}

	eState m_state;
	eState State
	{
		get { return m_state; }
		set
		{
			if(m_state != value)
			{
				m_state = value;
				SendMessage("OnStateChanged", m_state, SendMessageOptions.DontRequireReceiver);

				if(m_state == eState.ATTACK)
				{
					m_attackCooldownTime = m_attackCooldown;
				}
			}
		}
	}

	bool CanAttack { get; set; }

	float m_attackCooldownTime = 0.0f;

	Rigidbody m_rb = null;
	Vector3 m_moveTarget = Vector3.zero;
	GameObject m_attackTargetGameObject = null;
	Vector3 m_attackTarget = Vector3.zero;

	void Start()
    {
		m_rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
		CanAttack = UpdateAttackPossibility();
		if (CanAttack)
		{
			State = eState.ATTACK;
		}

		UpdateTargets();

		switch (State)
		{
			case eState.IDLE:
				UpdateIdle();
				break;
			case eState.MOVE:
				if(m_moveTarget != Vector3.zero)
				{
					UpdateMove();
				}
				break;
			case eState.ATTACK:
				if(m_attackTarget != Vector3.zero)
				{
					UpdateAttack();
				}
				if(m_canMoveWhileAttack && m_moveTarget != Vector3.zero)
				{
					UpdateMove();
				}
				break;
			case eState.DEAD:
				UpdateDead();
				break;
			default:
				break;
		}
    }

	bool UpdateAttackPossibility()
	{
		if((transform.position - m_attackTarget).magnitude <= m_attackDistance)
		{
			//TODO: Add checks for barriers and etc
			return true;
		}

		return false;
	}

	protected virtual void UpdateTargets()
	{
		PlayerController player = GameManager.GetInstance().Player;
		if(!player)
		{
			m_moveTarget = Vector3.zero;
			m_attackTarget = Vector3.zero;
			return;
		}

		m_moveTarget = player.transform.position;
		m_attackTarget = player.transform.position;

		//TODO: change it for correct values
		if((m_moveTarget - transform.position).sqrMagnitude <= 1.0f)
		{
			State = eState.IDLE;
			m_moveTarget = Vector3.zero;
			m_rb.velocity = Vector3.zero;
		}

		if(m_attackTarget != Vector3.zero)
		{
			transform.LookAt(new Vector3(m_attackTarget.x, transform.position.y, m_attackTarget.z));
		}
	}

	protected virtual void UpdateIdle()
	{
		if (m_moveTarget != Vector3.zero)
		{
			State = eState.MOVE;
		}
		if (m_attackTarget != Vector3.zero)
		{
			State = eState.ATTACK;
		}
	}

	protected virtual void UpdateMove()
	{
		Vector3 direction = (m_moveTarget - transform.position).normalized;
		direction = new Vector3(direction.x, 0.0f, direction.z);
		m_rb.velocity = direction * m_moveSpeed * Time.deltaTime * 10.0f;
		Debug.DrawLine(transform.position, m_moveTarget, Color.red);
	}

	protected virtual void UpdateAttack()
	{
		m_attackCooldownTime -= Time.deltaTime;
		if(m_attackCooldownTime <= 0.0f)
		{
			m_attackCooldownTime = m_attackCooldown;
			if(CanAttack)
			{
				GameObject projectile = GameObject.Instantiate(m_attackProjectile.gameObject, m_attackProjectileInitialTransform.position, Quaternion.identity);
				AttackProjectile attackProjectile = projectile.GetComponent<AttackProjectile>();
				attackProjectile.AttackTarget = m_attackTarget;
			}
		}
	}

	protected virtual void UpdateDead()
	{
		//TODO: lisen for animation event
		
	}

	public void TakeDamage(int damage)
	{
		m_healt -= damage;
		if(m_healt <= 0)
		{
			State = eState.DEAD;
			StartCoroutine(DestroyObject());
		}
	}

	IEnumerator DestroyObject()
	{
		yield return new WaitForSeconds(3.0f);
		GameObject.Destroy(this.gameObject);
	}
}
