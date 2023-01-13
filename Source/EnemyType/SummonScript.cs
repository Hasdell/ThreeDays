using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameMgr;


public class SummonScript : Enemy
{
    float BombTimer = 0f;

    bool isAttack = false;
    bool isArrival = false;

    public GameObject explosionEffect;
    public Vector3 targetPosition;

    Color c;

    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = EnemyData.EnemyTable[5];

        stat = new Status(enemyInfo.monster_Hp, 0, enemyInfo.monster_Damage);

        StartCoroutine(MovePosition(targetPosition));
    }

	private void Update()
    {
        if(enemyInfo.monster_Hp < 0)
            Destroy(gameObject);
    }

	public override IEnumerator Attack()
    {
        // enemyAttackWarningArea.SetActive(true);
        enemyAttackTimingBox.SetActive(true);

        yield return new WaitForSeconds(enemyInfo.monster_AttackDelay);

        isAttack = true;

        enemyAttackTimingBox.SetActive(false);
        enemyAttackWarningArea.SetActive(false);

        yield return new WaitForSeconds(enemyInfo.monster_AttackSpeed);

        enemyAttackCollider.SetActive(true);

        CheckCollider();
        
        var explosionObject = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.1f);

        enemyAttackCollider.SetActive(false);

        Destroy(explosionObject, 0.65f);
        Destroy(gameObject);
    }
    public IEnumerator MovePosition(Vector3 position)
    {
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, position, enemyInfo.monster_Speed * Time.deltaTime);

            if (((position - transform.position).magnitude) < 0.1f)
            {
                break;
            }
            yield return null;
        }
        StartCoroutine(Attack());
    }
    // Update is called once per frame
}
