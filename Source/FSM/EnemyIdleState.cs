using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
	float randomValue = 0f;
	public override void Begin(EnemyController ctrl)
	{
		randomValue = Random.Range(1f, 100f);
	}
	public override void Update(EnemyController ctrl)
	{
		if (ctrl.CheckTargetInBush())
			return;
		if (ctrl.GetIsChangeState())
			return;
		if (ctrl.CheckInTraceRange())
		{
			if (randomValue <= ctrl.GetEnemyStateProbability().idleToEscapeProbability)
			{
				ctrl.ChangeState(ctrl.IdleState);
				return;
			}
			ctrl.ChangeState(ctrl.TraceState);
			return;
		}
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
