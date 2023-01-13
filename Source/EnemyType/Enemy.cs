using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyData;
using static GameMgr;

public class Enemy : MonoBehaviour
{
    public Status stat;
    public GameObject hitEffect;
    public GameObject hitEffectBlood;

    protected GameMgr gm;

    protected EnemyController enemyController;
    protected EnemyInfo enemyInfo;

    [SerializeField]
    protected GameObject enemyAttackCollider;
    [SerializeField]
    protected GameObject enemyAttackWarningArea;
    [SerializeField]
    protected GameObject enemyAttackTimingBox;
    [SerializeField]
    protected GameObject enemyHitCollierBox;

    private PlayerHit playerHit;

    protected bool isAttackActivation = false;

    protected bool isPlayerHit = false;

    [SerializeField]
    protected BoxCollider2D enemyAttackBoxColl;

    private void Awake()
    {
        gm = GM;
        ReadAttackData();
        enemyController = GetComponent<EnemyController>();
        playerHit = gm.Player.GetComponent<PlayerHit>();
    }
    public void AttCollsSetActiveFalse()
    {
        enemyAttackCollider.SetActive(false);
        enemyAttackTimingBox.SetActive(false);
        enemyAttackWarningArea.SetActive(false);
        enemyHitCollierBox.SetActive(false);
        
    }
    public float GetEnemyDamage()
    {
        return enemyInfo.monster_Damage;
    }
    public float GetEnemySpeed()
    {
        return enemyInfo.monster_Speed;
    }
    public bool GetIsAttackActivation()
    {
        return isAttackActivation;
    }
    public void SetChaseRange(float value)
    {
        enemyInfo.chase_Range = value;
    }
    public float GetChaseRange()
    {
        return enemyInfo.chase_Range;
    }
    public float GetAttackRange()
    {
        return enemyInfo.attack_Range;
    }
    public float GetMinDistance()
    {
        return enemyInfo.monster_MinDistance;
    }
    public void ChangeEnemyWarningAreaZRotation(Quaternion rotation)
    {
        enemyAttackWarningArea.transform.rotation = rotation;
    }
    public virtual IEnumerator Attack()
    {
        Debug.Log("Enemy Attack");
        yield return 0;
    }
    public IEnumerator PrintHitEffect(Vector3 PlayerPosition)
    {
        var hitEffectObject = Instantiate(hitEffect, transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.identity);
        var hitEffectBloodObject = Instantiate(hitEffectBlood, transform.position + new Vector3(0f, 1.5f, 0f), Quaternion.identity);

        if (PlayerPosition.x - transform.position.x > 0)
        {
            hitEffectBloodObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        yield return null;

        Destroy(hitEffectObject, 0.5f);
        Destroy(hitEffectBloodObject, 0.5f);
    }
    public virtual IEnumerator PrintAttackEffect()
    {
        Debug.Log("AttackEffect");
        yield return null;
    }
    public virtual void StartAttackCoroutine()
    {
        StartCoroutine(Attack());
    }
    public virtual void StopAttackCoroutine()
    {
        StopCoroutine(Attack());
    }

    protected void CheckCollider()
    {
        RaycastHit2D[] hitResults = new RaycastHit2D[100];
        for (int i = 0; i < enemyAttackBoxColl.Cast(Vector2.left, hitResults, 0); i++)
        {
            if (hitResults[i].collider.gameObject.tag == "Player")
            {
                //Debug.Log(hitResults[i].collider.gameObject.name);
                playerHit.Hit(-enemyInfo.monster_Damage);
                isPlayerHit = true;
            }
        }
    }
}
