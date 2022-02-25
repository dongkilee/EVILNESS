using UnityEngine;
using System.Collections;

public class Enemy : AIController
{

    void Awake()
    {
        m_GetStat = m_Enemy;

    }

    void Start()
    {
        Init();
        //        StartCoroutine(GameManager.Instance.CreateDamageMeter1("적 :  풀바람 속성"));
    }

    void Update()
    {
        //        m_TargetDist = Vector3.Distance(transform.position, m_TargetTr.position);
        //        OnAttack(true, GetComponent<EnemySkill>().SkillNum);

    }

    protected override bool SetStateMove()
    {
        // 움직이는 조건.
        //        if (!GameManager.Instance.NowGameSpawner().PrevStage.GetComponent<StageClose>().CloseCheck)
        if (m_TraceDist <= m_TargetDist)
            return true;
        else
            return false;
    }
    protected override bool SetStateAttack()
    {
        // 공격하는 조건.
        //        if (m_TargetDist <= m_AttackDist)
        //            return true;
        //        else
        return m_AttackCheck;
    }
    protected override bool SetStateTrace()
    {
        // 플레이어는 추적하는 조건.
        //        if (GameManager.Instance.NowGameSpawner().PrevStage.GetComponent<StageClose>().CloseCheck)
        if (m_TraceDist > m_TargetDist)
            return true;
        else
            return false;
    }

    protected override bool SetStateStun()
    {
        return m_StunCheck;
    }

    public override void SetStun()
    {
        m_AttackCheck = false;
        m_EnemyMove.OnAttack(false, 0);
        //        m_NvAgent.speed = 0;
        m_EnemyMove.Move(Vector3.zero);
    }

    protected override void SetMove()
    {
        //        m_EnemyMove.m_NvAgent.speed = m_MoveSpeed;    // 스피드 조절.
        //        m_AINavMesh.m_StoppingDistance = 0;
        m_NowTargetPos = m_CurrentPos;
        if (Vector3.Distance(transform.position, m_NowTargetPos) > 2)
            m_EnemyMove.Move(m_AINavMesh.Move() * m_TraceSpeed);
        else
            m_EnemyMove.Move(Vector3.zero);

    }

    protected override void SetAttack()
    {


        // 공격하는 부분.
        // m_EnemyMove.m_NvAgent.speed = 0;

        m_EnemyMove.Move(Vector3.zero);
        m_EnemyMove.OnAttack(m_AttackCheck, m_SkillCount);
        //        Debug.Log("적이 공격한다.");
        if (m_AttackCheck && m_isAttackMoving)
        {
            m_isAttackMoving = false;
            EffectManager.Instance.EnemyRatSkillEffect(transform);
        }
        //        m_NvAgent.Stop();

    }

    public override void SetDie()
    {
        // 사운드
        m_Audio.clip = SoundManager.Instance.Monster_die;
        SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);

        // 죽는 부분.
        //        Debug.Log("몬스터가 죽었다.");
        m_EnemyMove.m_Animator.SetTrigger("IsDie");
        m_IsDie = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        Destroy(gameObject, 2.1f);
    }

    public override void SetHit()
    {
        // 히트 부분.
        //        m_EnemyHitCheck = true;
        m_EnemyMove.m_Animator.SetTrigger("IsHit");

        //        StartCoroutine(EnemyColor());

    }

    protected override void SetTrace()
    {
        // m_EnemyMove.m_NvAgent.speed = m_TraceSpeed;
        //        m_AINavMesh.m_StoppingDistance = m_StoppingDir;
        if (GameManager.Instance.isGameOver)
        {
            m_NowTargetPos = m_CurrentPos;
        }
        else
        {

                m_NowTargetPos = m_Target.transform.position;
        }
        // m_EnemyMove.m_NvAgent.SetDestination(m_Target.transform.position);

        // 방향 벡터 주기.
        m_EnemyMove.Move(m_AINavMesh.Move() * m_TraceSpeed);
        // AttackStart();
    }

    public override IEnumerator SetTimer(float time) // (예상).
    {
        yield return new WaitForSeconds(time);
        // 타이머 지나고 실행될 코드 작성.
    }

    public override IEnumerator Delay()
    {
        m_OnRush = false;
        m_AttackCheck = false;
        m_EnemyMove.OnAttack(m_AttackCheck, 0);
        yield return new WaitForSeconds(2);
        //        Debug.Log("몬스터 공격 딜레이 종료");
        m_AttackDelayCheck = false;
    }


}