using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEscapeState : EnemyBaseState
{
    public override void Begin(EnemyController ctrl)
    {

    }
    public override void Update(EnemyController ctrl)
    {
        if (!ctrl.IsAliveTarget())
        {
            ctrl.ChangeState(ctrl.IdleState);
            return;
        }
        if (ctrl.GetIsChangeState())
            return;
        if (ctrl.EscapeCompleted())
        {
            ctrl.ChangeState(ctrl.IdleState);
            return;
        }
        ctrl.Escape();
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
