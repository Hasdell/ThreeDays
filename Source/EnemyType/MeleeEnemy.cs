using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public GameObject attackEffect;
    private GameObject attackEffectObject;
    private IEnumerator effectCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = EnemyData.EnemyTable[1];
        attackEffectObject = null;

        stat = new Status(enemyInfo.monster_Hp, 0, enemyInfo.monster_Damage);
    }
    public override IEnumerator Attack()
    {
        isAttackActivation = true;

        enemyAttackTimingBox.SetActive(true);

        yield return new WaitForSeconds(enemyInfo.monster_AttackDelay);

        enemyAttackTimingBox.SetActive(false);

        enemyAttackCollider.SetActive(true);

        //Attack
        CheckCollider();

        effectCoroutine = PrintAttackEffect();
        StartCoroutine(effectCoroutine);

        yield return new WaitForSeconds(enemyInfo.monster_AttackSpeed);

        enemyAttackCollider.SetActive(false);

        isAttackActivation = false;
    }
    public override IEnumerator PrintAttackEffect()
    {
        yield return new WaitForSeconds(0.15f);
        if(attackEffectObject == null)
           attackEffectObject = Instantiate(attackEffect, transform.position + new Vector3(-1, 1, 0), Quaternion.identity);
        if (enemyController.GetPlayerDirectionX() == (int)PlayerDirectionX.LEFT)
        {
            attackEffectObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            attackEffectObject.transform.position = transform.position + new Vector3(1, 1, 0);
        }
        yield return new WaitForSeconds(0.15f);
        Destroy(attackEffectObject);
        attackEffectObject = null;
        effectCoroutine = null;
    }
    public override void StartAttackCoroutine()
    {
        StartCoroutine(Attack());
    }
    public override void StopAttackCoroutine()
    {
        StopCoroutine(Attack());
        if (attackEffectObject != null)
            Destroy(attackEffectObject);
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);
    }
}
