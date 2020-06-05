using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator = null;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void OnStateChanged(EnemyController enemy)
	{
        int state = (int)enemy.State;
        animator.SetInteger("State", state);
        animator.SetBool("CanMoveWhileAttack", enemy.CanMoveWhileAttack);
	}

    void OnAttack()
	{
        animator.SetTrigger("Attack");
	}
}
