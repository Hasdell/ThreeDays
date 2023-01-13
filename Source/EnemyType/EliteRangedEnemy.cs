using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteRangedEnemy : Enemy
{
    public GameObject summonCreature;
    private float x, y;
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = EnemyData.EnemyTable[4];

        stat = new Status(enemyInfo.monster_Hp, 0, enemyInfo.monster_Damage);
    }

    override public IEnumerator Attack()
    {
        enemyAttackWarningArea.transform.position = enemyController.GetTargetTransformPosition();

        isAttackActivation = true;

        yield return new WaitForSeconds(enemyInfo.monster_AttackDelay);

        for (int i = 0; i < 4; i++)
        {
            x = Random.Range(-2f, 2f);
            y = Random.Range(-1f, 1f);
            var summonObject = Instantiate(summonCreature, transform.position + new Vector3(0, 2.5f, 0f), Quaternion.identity);
            summonObject.GetComponent<SummonScript>().targetPosition = enemyAttackWarningArea.transform.position + new Vector3(x, y, 0);
        }

        yield return new WaitForSeconds(enemyInfo.monster_AttackSpeed);

        isAttackActivation = false;
    }
}
