using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
	float timer = 0f;
	float delayTime = 0.3f;
    public override void Begin(EnemyController ctrl)
	{
		timer = 0;
	}
	public override void Update(EnemyController ctrl)
	{
		timer += Time.deltaTime;

		if (!ctrl.IsAlive())
		{
			ctrl.ChangeState(ctrl.DeadState);
			return;
		}
		if (timer > delayTime)
        {
            if (ctrl.GetIsChangeState())
                return;
			if(ctrl.CheckInTraceRange())
            {
				ctrl.ChangeState(ctrl.TraceState);
				return;
            }
            if(ctrl.CheckInAttackRange())
            {
                ctrl.ChangeState(ctrl.AttackState);
                return;
            }
		}

	}
    public override void OnCollisionEnter(EnemyController ctrl)
	{

	}
    public override void End(EnemyController ctrl)
	{

	}
}
