using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using static GameMgr;
using static EnemyStateProbabilityData;

enum AttackType
{
    MELEE,
    RANGED,
    MELEE_ELITE,
    RANGED_ELITE
}
enum PlayerDirectionX
{
    LEFT,
    RIGHT
}
enum TraceType
{
    TRACE1,
    TRACE2,
    TRACE3
}

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rigid;

    [SerializeField]
    private MainUIMgr mainUIMgr;

    private Enemy enemy;
    private EnemyBaseState currentState;
    private EnemyBaseState prevState;

    public int attTypeValue;
    private AttackType attType;

    private int traceTypeValue;
    private TraceType traceType;
    private Vector2 AddPosition;

    private PlayerDirectionX playerDirectionX;
    private int playerDirectionXValue;

    private Transform targetTransform;
    private Vector2 targetPosition;

    public GameObject hpSlider;

    private SkeletonAnimation skeletonAnimation;
    private Slider slider;

    Coroutine stateCoroutine;

    Animator enemyAnimator;

    EnemyStateProbability enemyStateProbability;

    public AudioClip audioDead;
    public AudioClip audioHit;
    public AudioClip audioTrace;
    private AudioSource audio;

    public bool isInBush = false;
    private bool isChangeState = false;
    private bool isHitCheck = false;
    private bool isEscape = false;
    private bool isRushAttack = false;
    public bool IsRush { get { return isRushAttack; } }

    public bool leftOutside = false;
    public bool rightOutside = false;
    //public bool skill_02_Check = false;

    public EnemyBaseState CurrentState
    {
        get { return currentState; }
    }

    public static int enemyCount = 0;
    public EnemySection enemySection = null;

    public readonly EnemyIdleState IdleState = new EnemyIdleState();
    public readonly EnemyTraceState TraceState = new EnemyTraceState();
    public readonly EnemyAttackState AttackState = new EnemyAttackState();
    public readonly EnemyDeadState DeadState = new EnemyDeadState();
    public readonly EnemyHitState HitState = new EnemyHitState();
    public readonly EnemyEscapeState EscapeState = new EnemyEscapeState();


    private void Awake()
    {
        currentState = IdleState;
        playerDirectionX = PlayerDirectionX.LEFT;

        attType = (AttackType)attTypeValue;
        traceTypeValue = 0;

        player = GameObject.Find("Player");
        mainUIMgr = GameObject.Find("UI").GetComponent<MainUIMgr>();
        targetTransform = player.transform;

        slider = hpSlider.GetComponentInChildren<Slider>();

        audio = GetComponent<AudioSource>();

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        ++enemyCount;
        if (transform.parent)
        {
            if (transform.parent.parent)
            {
                enemySection = transform.parent.parent.GetComponent<EnemySection>();
                ++enemySection.enemyCount_Wave;
                ++enemySection.RealEnemyObjCount;
            }
        }
        mainUIMgr.enemyList.Add(this);
    }
    private void Start()
    {
        ReadProbabilityData();
        enemyAnimator = GetComponent<Animator>();

        if (attType == AttackType.MELEE)
        {
            enemy = GetComponent<MeleeEnemy>();
            enemyStateProbability = EnemyProbabilityTable[1];
        }
        else if (attType == AttackType.RANGED)
        {
            enemy = GetComponent<RangedEnemy>();
            enemyStateProbability = EnemyProbabilityTable[2];
        }
        else if (attType == AttackType.MELEE_ELITE)
        {
            enemy = GetComponent<EliteMeleeEnemy>();
            enemyStateProbability = EnemyProbabilityTable[3];
        }
        else if (attType == AttackType.RANGED_ELITE)
        {
            enemy = GetComponent<EliteRangedEnemy>();
            enemyStateProbability = EnemyProbabilityTable[4];
        }

        currentState = IdleState;
    }
    private void Update()
    {
        currentState.Update(this);
        if (attType == AttackType.RANGED || attType == AttackType.RANGED_ELITE)
        {
            if (!isHitCheck && enemy.GetChaseRange() < 10)
                CheckEnemysHit();
        }
    }
    public void Hit(int damage)
    {
        enemy.stat.curHP -= damage;
        // Debug.Log("Hit! Enemy Hp : " + enemy.stat.curHP);
        currentState.OnCollisionEnter(this);
        GM.SetEnemyHit(true);

        StartCoroutine(KnockBackEnemy());

        if (hpSlider.activeSelf == false)
        {
            hpSlider.SetActive(true);
            StartCoroutine(HideHpBar());
        }
        else
        {
            StopCoroutine(HideHpBar());
            StartCoroutine(HideHpBar());
        }

        slider.value = enemy.stat.curHP / enemy.stat.MaxHP;
    }
    IEnumerator KnockBackEnemy()
    {
        Vector3 dir;
        float curTime = 0f;

        if ((targetTransform.position.x - transform.position.x) > 0)
            dir = transform.position + new Vector3(-0.4f, 0);
        else
            dir = transform.position + new Vector3(0.4f, 0);

        while (curTime < 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, dir, 10 * Time.deltaTime);
            curTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator HideHpBar()
    {
        yield return new WaitForSeconds(4.0f);

        hpSlider.SetActive(false);
    }

    private void CheckEnemysHit()
    {
        if (GM.GetEnemyHit() == true)
        {
            enemy.SetChaseRange(enemy.GetChaseRange() * 3f);
        }
        isHitCheck = true;
        Invoke("HitCheckReset", 0.5f);
    }
    private void HitCheckReset()
    {
        isHitCheck = false;
    }
    public bool GetIsChangeState()
    {
        return isChangeState;
    }
    IEnumerator ChangeStateDelay(EnemyBaseState state)
    {
        isChangeState = true;

        float delayTime = 0.5f;

        if (state != DeadState && state != HitState && currentState != HitState && state != EscapeState)
            yield return new WaitForSeconds(delayTime);

        float randomValue = Random.Range(1f, 100f);

        if (prevState == EscapeState)
        {
            isEscape = false;
        }

        currentState.End(this);
        prevState = currentState;
        currentState = state;
        currentState.Begin(this);

        if (state == TraceState)
        {
            traceTypeValue = Random.Range(0, 3);
            traceType = (TraceType)traceTypeValue;

            if(prevState == HitState)
            {
                Debug.Log("hit to trace");
                traceTypeValue = 0;
                traceType = TraceType.TRACE1;
            }
            targetPosition = targetTransform.position;

            SetAddPosition();
        }
        if (state == EscapeState)
        {
            CheckPlayerDirectionX();

            if (playerDirectionX == PlayerDirectionX.LEFT)
                targetPosition = (Vector2)transform.position + new Vector2(5, 0);
            else
                targetPosition = (Vector2)transform.position + new Vector2(-5, 0);
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, GM.CurRoomMgr.MapSizeMin.x, GM.CurRoomMgr.MapSizeMax.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, GM.CurRoomMgr.MapSizeMin.y, GM.CurRoomMgr.MapSizeMax.y);

        if (state == IdleState)
        {
            if (prevState != IdleState)
                enemyAnimator.SetTrigger("isIdle");
        }
        else if (state == TraceState || state == EscapeState)
        {
            enemyAnimator.SetTrigger("isMove");
        }
        else if (state == HitState)
        {
            PrintHitEffect();

            audio.clip = audioHit;
            audio.Play();

            enemyAnimator.SetTrigger("isHit");
        }
        else if (state == DeadState)
        {
            float random = Random.Range(0, 2);

            if (random == 0)
                enemyAnimator.SetTrigger("isDead");
            if (random == 1)
                enemyAnimator.SetTrigger("isDead2");
            audio.PlayOneShot(audioHit);
            audio.clip = audioDead;
            audio.Play();
        }
        isChangeState = false;
    }
    public void ChangeState(EnemyBaseState state)
    {
        if(isRushAttack)
        {
            if (currentState == AttackState && state == HitState)
            {
                audio.PlayOneShot(audioHit);
                return;
            }
        }
        if (currentState != HitState && currentState != IdleState && state != DeadState && state != HitState && state != EscapeState)
        {
            if (currentState == TraceState)
            {
                traceType = TraceType.TRACE1;
                AddPosition = new Vector2(0, 0);
            }

            currentState.End(this);
            prevState = currentState;
            currentState = IdleState;
            currentState.Begin(this);

            
            enemyAnimator.SetTrigger("isIdle");
        }
        else
        {
            if (stateCoroutine != null)
            {
                StopCoroutine(stateCoroutine);
            }
        }

        stateCoroutine = StartCoroutine(ChangeStateDelay(state));
    }
    public float CalcTargetDistance()
    {
        return (player.transform.position - transform.position).magnitude;
    }
    public bool CheckInTraceRange()
    {
        return (CalcTargetDistance() < enemy.GetChaseRange()) ? true : false;
    }
    public bool CheckInAttackRange()
    {
        return (CalcTargetDistance() < enemy.GetAttackRange() && Mathf.Abs(transform.position.y - player.transform.position.y) < 0.3f) ? true : false;
    }
    public bool CheckTargetInBush()
    {
        if (player.GetComponent<PlayerController>().InBush)
        {
            if (enemy.GetChaseRange() > 10)
                enemy.SetChaseRange(enemy.GetChaseRange() / 3);
            GM.SetEnemyHit(false);
            return true;
        }
        return false;
    }
    public bool CheckEnemyInBush()
    {
        if (CheckInTraceRange())
            return isInBush;
        else
            return false;
    }
    public bool IsAlive()
    {
        return (enemy.stat.curHP > 0) ? true : false;
    }
    public bool IsAliveTarget()
    {
        if (player.transform == null) return false;

        return true;
    }
    public void EnemyDead()
    {
        enemy.StopAllCoroutines();
        mainUIMgr.enemyList.Remove(this);
        enemy.AttCollsSetActiveFalse();
        hpSlider.SetActive(false);
        --enemyCount;
        if (enemySection)
            --enemySection.enemyCount_Wave;
        if (enemyCount == 0)
        {
            GM.canSlow = true;
        }
    }

    IEnumerator Slow(float speed)
    {
        Time.timeScale = speed;
        yield return new WaitForSecondsRealtime(GM.SlowTime);
        Time.timeScale = 1;
    }

    public void DestroyEnemy()
    {
        StartCoroutine(EnemyDisappear());
    }

    public void OnDestroy()
    {
        --enemySection.RealEnemyObjCount;
        mainUIMgr.enemyList.Remove(this);
    }

    IEnumerator EnemyDisappear()
    {
        yield return null;

        float curTIme = 0;
        float duration = 2f;
        float invDuration = 1 / duration;

        Renderer renderer;

        renderer = GetComponentInChildren<Renderer>();
        Color color = renderer.material.GetColor("_Color");

        while (curTIme < duration)
        {
            curTIme += Time.deltaTime;

            color.a = (2 - curTIme) * invDuration;
            renderer.material.SetColor("_Color", color);

            yield return null;
        }
        Destroy(gameObject);
    }
    public int GetPlayerDirectionX()
    {
        playerDirectionXValue = (int)playerDirectionX;
        return playerDirectionXValue;
    }
    public Vector3 GetTargetTransformPosition()
    {
        return targetTransform.position;
    }
    public void CheckPlayerDirectionX()
    {
        if (transform.position.x - targetTransform.position.x > 0)
            playerDirectionX = PlayerDirectionX.LEFT;
        else if (transform.position.x - targetTransform.position.x < 0)
            playerDirectionX = PlayerDirectionX.RIGHT;

        if (traceType == TraceType.TRACE2 || traceType == TraceType.TRACE3)
        {
            if (transform.position.x - (targetPosition.x) > 0)
                playerDirectionX = PlayerDirectionX.LEFT;
            else if (transform.position.x - (targetPosition.x) < 0)
                playerDirectionX = PlayerDirectionX.RIGHT;
        }
    }
    public void CheckPlayerDirectionX(Vector2 target)
    {
        if (transform.position.x - target.x > 0)
            playerDirectionX = PlayerDirectionX.LEFT;
        else if (transform.position.x - target.x < 0)
            playerDirectionX = PlayerDirectionX.RIGHT;
    }
    public void ChangeRotation()
    {
        if (playerDirectionX == PlayerDirectionX.LEFT)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            enemy.ChangeEnemyWarningAreaZRotation(Quaternion.Euler(0f, 0f, 0f));
        }
        else if (playerDirectionX == PlayerDirectionX.RIGHT)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            enemy.ChangeEnemyWarningAreaZRotation(Quaternion.Euler(0f, 0f, 180f));
        }

        hpSlider.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    public void Move()
    {
        if (traceType == TraceType.TRACE1)
        {
            if (Mathf.Abs(transform.position.x - targetTransform.position.x) <= enemy.GetMinDistance())
            {
                // X축 거리는 유지하고 Z축만 이동
                if (playerDirectionX == PlayerDirectionX.LEFT)
                    transform.position = Vector2.MoveTowards(transform.position, (Vector2)targetTransform.position + new Vector2(+enemy.GetMinDistance(), 0f), enemy.GetEnemySpeed() * Time.deltaTime);
                else
                    transform.position = Vector2.MoveTowards(transform.position, (Vector2)targetTransform.position + new Vector2(-enemy.GetMinDistance(), 0f), enemy.GetEnemySpeed() * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, (Vector2)targetTransform.position, enemy.GetEnemySpeed() * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemy.GetEnemySpeed() * Time.deltaTime);
        }
    }
    public void CheckArrivedInPosition()
    {
        if ((Vector2)transform.position == targetPosition)
        {
            if (traceType == TraceType.TRACE2 || traceType == TraceType.TRACE3)
            {
                traceTypeValue = Random.Range(0, 3);
                traceType = (TraceType)traceTypeValue;
                SetAddPosition();
            }
        }
    }
    public void SetAddPosition()
    {
        float x, y;

        x = 0;
        y = Random.Range(0f, 3f);

        if (traceType == TraceType.TRACE1)
            x = 0;
        else if (traceType == TraceType.TRACE2)
            x = 10;
        else if (traceType == TraceType.TRACE3)
            x = -10;

        AddPosition = new Vector2(0, 0);

        if (traceType != TraceType.TRACE1)
        {
            AddPosition = new Vector2(x, y);
            targetPosition += AddPosition;
            targetPosition.x = Mathf.Clamp(targetPosition.x, GM.CurRoomMgr.MapSizeMin.x, GM.CurRoomMgr.MapSizeMax.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, GM.CurRoomMgr.MapSizeMin.y, GM.CurRoomMgr.MapSizeMax.y);
        }
    }
    public void TraceTarget()
    {
        if (enemy.GetChaseRange() < 10)
            enemy.SetChaseRange(enemy.GetChaseRange() * 3);

        CheckPlayerDirectionX();
        ChangeRotation();

        Move();
        CheckArrivedInPosition();
    }
    public void Attack()
    {
        traceTypeValue = 0;
        traceType = (TraceType)traceTypeValue;

        float randomValue = 0f;
        randomValue = Random.Range(1f, 100f);

        if (randomValue <= enemyStateProbability.attackToEscapeProbability)
        {
            ChangeState(EscapeState);
            return;
        }
        CheckPlayerDirectionX();
        ChangeRotation();

        enemyAnimator.SetTrigger("isAttack");
        enemy.StartAttackCoroutine();
    }
    public void Escape()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemy.GetEnemySpeed() * Time.deltaTime);

        CheckPlayerDirectionX(targetPosition);
        ChangeRotation();

        if ((Vector2)transform.position == targetPosition)
        {
            isEscape = true;
        }
    }
    public bool EscapeCompleted()
    {
        return isEscape;
    }
    public bool GetIsAttackActivation()
    {
        return enemy.GetIsAttackActivation();
    }
    public void SetIsRushAttack(bool set)
    {
        isRushAttack = set;
    }
    public void SetAnimation(string animeName)
    {
        enemyAnimator.SetTrigger(animeName);
    }
    public EnemyStateProbability GetEnemyStateProbability()
    {
        return enemyStateProbability;
    }
    public void PrintHitEffect()
    {
        StartCoroutine(enemy.PrintHitEffect(player.transform.position));
    }
    public void CheckAnimation()
    {
        if (!IsAlive())
        {
            if (hpSlider.activeSelf == true)
                hpSlider.SetActive(false);
            if (skeletonAnimation.AnimationName != "dead" && skeletonAnimation.AnimationName != "dead2" && skeletonAnimation.AnimationName != "dead_change")
                enemyAnimator.SetTrigger("isDead");
        }
    }

}