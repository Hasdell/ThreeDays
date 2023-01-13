using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    float timer = 0f;
    float delayTime = 3.0f;
    public override void Begin(EnemyController ctrl)
    {
        Debug.Log("Enemy Dead");
        ctrl.PrintHitEffect();
        ctrl.EnemyDead();
    }
    public override void Update(EnemyController ctrl)
    {
        timer += Time.deltaTime;

        ctrl.CheckAnimation();
        if (timer > delayTime)
        {
            timer = -10f;
            ctrl.DestroyEnemy();
        }
    }
    public override void OnCollisionEnter(EnemyController ctrl)
    {
        
    }
    public override void End(EnemyController ctrl)
    {
        
    }
}
