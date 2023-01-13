using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RangedEnemy : Enemy
{
    public GameObject summonCreature;
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = EnemyData.EnemyTable[2];

        stat = new Status(enemyInfo.monster_Hp, 0, enemyInfo.monster_Damage);
    }
    public override IEnumerator Attack()
    {
        enemyAttackWarningArea.transform.position = enemyController.GetTargetTransformPosition();

        isAttackActivation = true;

        yield return new WaitForSeconds(enemyInfo.monster_AttackDelay);

        var summonObject = Instantiate(summonCreature, transform.position + new Vector3(0, 2.5f, 0), Quaternion.identity);
        summonObject.GetComponent<SummonScript>().targetPosition = enemyAttackWarningArea.transform.position;

        // rangedEnemyAttackSkeletonAnime.AnimationState.SetAnimation(0, "animation", true).MixDuration = 0f;

        enemyAttackCollider.SetActive(true);

        //Attack
        CheckCollider();

        yield return new WaitForSeconds(enemyInfo.monster_AttackSpeed);

        enemyAttackCollider.SetActive(false);

        isAttackActivation = false;
    }
}
