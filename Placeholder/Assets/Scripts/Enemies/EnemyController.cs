using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IPooledObject
{
    #region IPooledObject
    public void OnSpawned()
    {
    }
    public void OnDestroy()
    {
    }
    public ESpawnItemType GetObjectType() { return type; }
	#endregion

	public ESpawnItemType type;
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
    public ESpawnItemType m_attackProjectileType;
	[SerializeField]
	Transform m_attackProjectileInitialTransform = null;

	public enum eState
	{
		IDLE,
		MOVE,
		ATTACK,
		DEAD
	}

	eState m_state;
	eState m_previousState;
	public eState State
	{
		get { return m_state; }
		set
		{
			if(m_state != value)
			{
				m_previousState = m_state;
				m_state = value;
				SendMessage("OnStateChanged", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	public eState PreviousState { get { return m_previousState; } }
	public bool CanMoveWhileAttack { get { return m_canMoveWhileAttack; } }

	bool CanAttack { get; set; }

	float m_attackCooldownTime = 0.0f;

	Rigidbody m_rb = null;
	Vector3 m_moveTarget = Vector3.zero;
	GameObject m_attackTargetGameObject = null;
	Vector3 m_attackTarget = Vector3.zero;
	NavMeshAgent m_navMeshAgent = null;

	bool UsingNavMesh { get { return m_navMeshAgent && m_navMeshAgent.isOnNavMesh; } }

	void Start()
    {
		m_rb = GetComponent<Rigidbody>();
		m_navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void OnDisable()
	{
		if(m_navMeshAgent)
		{
			m_navMeshAgent.enabled = false;
		}
	}

	void Update()
    {
		if (m_state == eState.DEAD)
		{
			return;
		}

		if (m_navMeshAgent && !m_navMeshAgent.enabled)
		{
			m_navMeshAgent.enabled = true;
		}

		m_attackCooldownTime -= Time.deltaTime;

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

	bool NeedMoveCloser()
	{
		if (m_attackTarget == Vector3.zero)
		{
			return false;
		}

		if ((transform.position - m_attackTarget).magnitude > m_attackDistance)
		{
			return true;
		}

		return false;
	}

	bool UpdateAttackPossibility()
	{
		if(NeedMoveCloser())
		{
			return false;
		}

		//TODO: Add checks for barriers and etc

		return true;
	}

	protected virtual void UpdateTargets()
	{
		PlayerController player = GameManager.GetInstance().Player;
		if(!player)
		{
			m_moveTarget = Vector3.zero;
			m_attackTarget = Vector3.zero;
			State = eState.IDLE;
			return;
		}

		m_moveTarget = player.transform.position;
		m_attackTarget = player.transform.position;

		if(!NeedMoveCloser())
		{
			if(m_attackTarget != Vector3.zero)
			{
				State = eState.ATTACK;
			}
			else
			{
				State = eState.IDLE;
			}

			m_moveTarget = Vector3.zero;
			if(UsingNavMesh)
			{
				m_navMeshAgent.destination = transform.position;
			}
			else
			{
				m_rb.velocity = Vector3.zero;
			}
		}

		if(m_attackTarget != Vector3.zero)
		{
			if(UsingNavMesh)
			{
				transform.LookAt(new Vector3(m_navMeshAgent.steeringTarget.x, transform.position.y, m_navMeshAgent.steeringTarget.z));
			}
			else
			{
				transform.LookAt(new Vector3(m_attackTarget.x, transform.position.y, m_attackTarget.z));
			}
		}
	}

	protected virtual void UpdateIdle()
	{
		if (m_moveTarget != Vector3.zero)
		{
			State = eState.MOVE;
		}
	}

	protected virtual void UpdateMove()
	{
		if(UsingNavMesh)
		{
			m_navMeshAgent.destination = m_moveTarget;
			UnityEngine.Debug.DrawLine(transform.position, m_navMeshAgent.steeringTarget, Color.red);
		}
		else
		{
			Vector3 toTarget = m_moveTarget - transform.position;
			Vector3 direction = toTarget.normalized;
			//direction = new Vector3(direction.x, 0.0f, direction.z);

			m_rb.velocity = direction * m_moveSpeed * Time.deltaTime * 10.0f;
			UnityEngine.Debug.DrawLine(transform.position, m_moveTarget, Color.red);
		}
	}

	protected virtual void UpdateAttack()
	{
		if(CanAttack)
		{
			if (m_attackCooldownTime <= 0.0f)
			{
				m_attackCooldownTime = m_attackCooldown;
				
				SendMessage("OnAttack");
				GameObject projectile = GameManager.GetInstance().GetNewObject(m_attackProjectileType);
				projectile.transform.position = m_attackProjectileInitialTransform.position;
				AttackProjectile attackProjectile = projectile.GetComponent<AttackProjectile>();
				attackProjectile.AttackTarget = m_attackTarget;
				projectile.SetActive(true);
				attackProjectile.Setup();
			}
		}
		else if(NeedMoveCloser())
		{
			State = eState.MOVE;
		}
	}

	protected virtual void UpdateDead()
	{
		//TODO: lisen for animation event
		
	}

	public void TakeDamage(int damage)
	{
		if(State == eState.DEAD)
		{
			return;
		}	

		m_healt -= damage;
		if(m_healt <= 0)
		{
			StartCoroutine(DestroyObject());
		}
	}

	IEnumerator DestroyObject()
	{
		State = eState.DEAD;
		//GetComponent<CapsuleCollider>().enabled = false;
		m_rb.velocity = Vector3.zero;
		if(m_navMeshAgent)
		{
			m_navMeshAgent.Stop();
		}

		yield return new WaitForSeconds(3.0f);
		GameObject.Destroy(this.gameObject);
	}
}
