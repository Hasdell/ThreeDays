using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void Begin(EnemyController ctrl);
    public abstract void Update(EnemyController ctrl);
    public abstract void OnCollisionEnter(EnemyController ctrl);
    public abstract void End(EnemyController ctrl);
}
