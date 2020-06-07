using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundController : MonoBehaviour
{
    [SerializeField]
    AudioClip m_step = null;
    [SerializeField]
    AudioClip m_attack = null;
    [SerializeField]
    AudioClip m_dead = null;

	AudioSource m_audioSource = null;

	void Start()
	{
		m_audioSource = GetComponent<AudioSource>();
	}

	void OnStateChanged(EnemyController enemy)
	{
		//if(enemy.State == EnemyController.eState.MOVE)
		//{
		//	m_audioSource.PlayOneShot(m_dead);
		//}

		if (enemy.State == EnemyController.eState.DEAD)
		{
			m_audioSource.PlayOneShot(m_dead);
		}
	}

	void OnAttack()
	{
		m_audioSource.PlayOneShot(m_attack);
	}
}
