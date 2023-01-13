using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMeleeEnemy : Enemy
{
    Vector3 dir;
    PlayerDirectionX playerDirectionX;
    Rigidbody2D playerRigid;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyInfo = EnemyData.EnemyTable[3];

        stat = new Status(enemyInfo.monster_Hp, 0, enemyInfo.monster_Damage);

        playerRigid = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    public override IEnumerator Attack()
    {
        isAttackActivation = true;

        enemyController.SetAnimation("isRush");

        playerDirectionX = (PlayerDirectionX)enemyController.GetPlayerDirectionX();

        // enemyAttackWarningArea.SetActive(true);
        enemyAttackTimingBox.SetActive(true);

        enemyController.SetIsRushAttack(true);

        yield return new WaitForSeconds(enemyInfo.monster_AttackDelay * 1.8f);

        enemyAttackWarningArea.SetActive(false);
        enemyAttackTimingBox.SetActive(false);

        enemyAttackCollider.SetActive(true);

        enemyController.SetAnimation("isRush2");

        if (playerDirectionX == PlayerDirectionX.LEFT)
        {
            dir = new Vector3(transform.position.x - 10, transform.position.y, 0);
        }
        else
        {
            dir = new Vector3(transform.position.x + 10, transform.position.y, 0);
        }

        dir.x = Mathf.Clamp(dir.x, gm.CurRoomMgr.MapSizeMin.x, gm.CurRoomMgr.MapSizeMax.x);
        dir.y = Mathf.Clamp(dir.y, gm.CurRoomMgr.MapSizeMin.y, gm.CurRoomMgr.MapSizeMax.y);
        // Á¶Á¤
        for (var f = 0f; f <= 1f; f += Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, dir, 10 * Time.deltaTime);
            if (!isPlayerHit)
            {
                CheckCollider();
                if (isPlayerHit)
                {
                    StartCoroutine(KnockBackPlayer());
                }
            }
            if (transform.position == dir)
            {
                enemyController.SetAnimation("isIdle");
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        enemyController.SetIsRushAttack(false);

        if (isPlayerHit)
        {
            isPlayerHit = false;
        }

        enemyAttackCollider.SetActive(false);

        isAttackActivation = false;
    }
    IEnumerator KnockBackPlayer()
    {
        Vector3 dir;

        if(enemyController.GetTargetTransformPosition().x > transform.position.x)
        {
            dir = enemyController.GetTargetTransformPosition() + new Vector3(3, 0, 0);
        }
        else
        {
            dir = enemyController.GetTargetTransformPosition() + new Vector3(-3, 0, 0);
        }
        dir.y = Mathf.Clamp(dir.y, gm.CurRoomMgr.MapSizeMin.y, gm.CurRoomMgr.MapSizeMax.y);

        float curTime = 0;

        while (curTime < 0.5f)
        {
            playerRigid.MovePosition(Vector3.Lerp(playerRigid.position, dir, Time.deltaTime * 20));
            curTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
