using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator = null;
    EnemyController enemy = null;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyController>();
    }

    void Update()
    {
        animator.SetBool("NeedMoveCloser", enemy.NeedMoveCloser);
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
