using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTraceState : EnemyBaseState
{
    float timer = 0f;
    float traceTime = 1f;
    float randomValue = 0f;
    public override void Begin(EnemyController ctrl)
    {
        randomValue = Random.Range(1f, 100f);
    }
    public override void Update(EnemyController ctrl)
    {
        timer += Time.deltaTime;

        if (!ctrl.IsAliveTarget())
        {
            ctrl.ChangeState(ctrl.IdleState);
            return;
        }
        if (ctrl.CheckTargetInBush() && timer > traceTime && !ctrl.CheckEnemyInBush())
        {
            timer = 0;
            ctrl.ChangeState(ctrl.IdleState);
            return;
        }
        if (!ctrl.CheckInTraceRange())
        {
            ctrl.ChangeState(ctrl.IdleState);
            return;
        }
        if(ctrl.CheckInAttackRange())
        {
            if (randomValue <= ctrl.GetEnemyStateProbability().traceToIdleProbability)
            {
                ctrl.ChangeState(ctrl.IdleState);
                return;
            }
            else if (randomValue <= ctrl.GetEnemyStateProbability().traceToIdleProbability + ctrl.GetEnemyStateProbability().traceToEscapeProbability)
            {
                ctrl.ChangeState(ctrl.EscapeState);
                return;
            }
            ctrl.ChangeState(ctrl.AttackState);
            return;
        }
        ctrl.TraceTarget();
    }
    public override void OnCollisionEnter(EnemyController ctrl)
    {
        if (!ctrl.IsAlive())
        {
            ctrl.ChangeState(ctrl.DeadState);
            return;
        }
        ctrl.ChangeState(ctrl.HitState);
    }
    public override void End(EnemyController ctrl)
    {
        
    }
}
