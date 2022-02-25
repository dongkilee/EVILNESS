using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AIManager))]
public abstract class AIController : MonoBehaviour
{
    protected CUnit m_GetStat;
    protected CWeapon m_Weapon = new CW_Default();
    [SerializeField]
    protected CEnemy m_Enemy = new CEnemy();

    [SerializeField]
    protected List<AISkillAttack> m_Skill = new List<AISkillAttack>();

    public GameObject[] m_WayPoint { set; get; }

    protected AISkillAttack nowSkill;
    protected int m_SkillCount = 0;
    public float m_TraceDist;              // 추적 거리.
    public Vector3 m_CurrentPos;          // 원래 나의 포지션
    public float m_TargetDist;
    public enum State
    {
        STUN,           // 기절.
        IDLE,         // MOVE
        TRACE,          // TRACE
        ATTACK,
        DIE
    }

    [HideInInspector]
    public bool m_OnRush = false;

    public State m_State;
    protected bool m_IsDie;             // IsDie

    public float m_MoveSpeed = 0.5f;        // MoveSpeed

    public float m_TraceSpeed = 1f;           // TraceSpeed
    public GameObject m_Target;


    public float m_DieTime;           // 죽는 시간(필요 예상).
    public int m_SkillProbability;         // 스킬 확률.
    public bool m_AttackCheck;        // 현재 공격상태인지 체크.
    public bool m_StunCheck;        // 현재 스턴상태인지 체크.
    public bool m_GroggyStunCheck;      // 그로기스턴 상태인지 체크
    public float m_GroggyStunCoolTime;    // 그로기 상태 쿨타임

    public bool m_AttackDelayCheck;      // 공격 하고 난 뒤 스킬 딜레이.
    public bool m_EnemyStrike;          // 몬스터 끼리 부딪쳤을 때
    protected Transform m_EnemyTr;     // 유닛 트랜스폼.
    protected Transform m_TargetTr;      // 타겟 트랜스폼.
    protected Vector3 m_NowTargetPos;       // 최종 목표 지점 포지션.
    public AIManager m_EnemyMove;
    protected AINavMeshAgent m_AINavMesh;
//    protected Property m_Property;
//    protected float m_StoppingDir;
    protected bool m_isAttackMoving;
    public AudioSource m_Audio;
    
    protected AISkillAttack.AttackKinds m_NowAttackKind;

    Coroutine CancelStun;
    Coroutine FootStun;

    void Start()
    {
        
    }

    protected void Init()
    {
        m_CurrentPos = transform.position;
        m_Target = GameObject.FindWithTag("Player");
        m_GetStat = m_Enemy;
        m_GetStat.setWeapon(m_Weapon);
        m_GetStat.Init();
        m_EnemyMove = GetComponent<AIManager>();
        m_AINavMesh = GetComponent<AINavMeshAgent>();
//        m_Property = GetComponent<Property>();
//        m_StoppingDir = m_AINavMesh.m_StoppingDistance;
        m_TargetTr = m_Target.transform;
        m_NowTargetPos = transform.position;
        m_State = State.IDLE;

        m_Audio = GetComponent<AudioSource>();

        m_IsDie = true;
        m_AttackCheck = false;

        for (int i = 0; i < m_Skill.Count; i++)
        {
            m_Skill[i].SetEnemy(this);
//            m_Skill[i].SetProperty(m_Property);
            m_Skill[i].SetTarget(m_Target);
            if(m_Skill[i].SkillCheck)
                StartCoroutine(m_Skill[i].CoolTimeReset());
        }

        StartCoroutine(StateAction());
        StartCoroutine(FSM());
        StartCoroutine(AttackStart1());

        // 스킬 거리순으로 정렬.
        m_Skill.Sort(delegate (AISkillAttack a, AISkillAttack b) { return a.AttackDist.CompareTo(b.AttackDist); });
    }

    protected IEnumerator StateAction()   // 상태별 행동 변화.
    {
        while (m_IsDie)
        {
            if (m_Target != null)
                m_TargetDist = Vector3.Distance(transform.position, m_TargetTr.position);
            else
                m_TargetDist = 0;
            // 네비 메쉬 돌리는 곳.
            m_AINavMesh.MovingPath(m_NowTargetPos, m_TargetDist);
 //           m_AINavMesh.TargetFromDraw();
            
            // 상태별로 변경시켜준다.
            if (SetStateStun())
                m_State = State.STUN;
            else if (SetStateAttack())
                m_State = State.ATTACK;
            else if (SetStateTrace())
                m_State = State.TRACE;
            else
                m_State = State.IDLE;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FSM()
    {
        while (m_IsDie)
        {
            switch (m_State)
            {
                case State.IDLE:
                    SetMove();
                    break;
                case State.TRACE:
                    SetTrace();
                    break;
                case State.ATTACK:
                    SetAttack();
                    break;
                case State.STUN:
                    SetStun();
                    break;
                default:
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public abstract IEnumerator SetTimer(float time);   // 시간초 만큼 움직임( 필요 예상 ).

    protected abstract bool SetStateMove();              // 움직이는 조건.
    protected abstract bool SetStateAttack();             // 공격하는 조건.
    protected abstract bool SetStateTrace();
    protected abstract bool SetStateStun();               // 스턴 조건.
    protected abstract void SetMove();                    // 움직이는 부분.
    protected abstract void SetAttack();                   // 공격하는 부분.
    public abstract void SetHit();                       // 히트 부분.
    public abstract void SetDie();                      // 죽는 부분.
    protected abstract void SetTrace();
    public abstract void SetStun();

    public Transform GetUnitTr()
    {
        return m_EnemyTr;
    }

    public Transform GetTargetTr()
    {
        return m_TargetTr;
    }

    public bool GetCheckDie()
    {
        return m_IsDie;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            m_EnemyStrike = true;
        }
    }

    void AttackSound(GameObject p)
    {
        if (p.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack1"))
        {
            m_Audio.clip = SoundManager.Instance.attack4_hit;
            SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
        }
        else if (p.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack2"))
        {
            m_Audio.clip = SoundManager.Instance.attack2_hit;
            SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
        }
        else if (p.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack3"))
        {
            m_Audio.clip = SoundManager.Instance.attack3_hit;
            SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
        }
        else if (p.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack4"))
        {
            m_Audio.clip = SoundManager.Instance.attack4_hit;
            SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);

        }


    }
    bool AttackEffectCheck = false;
    public void OnEnemyDamage(GameObject Player)
    {

        if (!m_IsDie) return;

        if (GetComponentInChildren<BoxCollider>() != null)
        {
            GetComponentInChildren<BoxCollider>().enabled = false;
        }
        if(!m_GroggyStunCheck && this is MiddleEnemy) StunCoroutine();
        //            GetComponent<Rigidbody>().AddForce(Player.transform.forward * 3000 * Time.fixedDeltaTime, ForceMode.VelocityChange);
        Player.GetComponent<PlayerManager>().GetStat().Combodisplay(); // 콤보
        // 상태에 따라 데미지 환산.
        PlayerLookAt();
//        m_Property.TargetPropertyDamage(Player.GetComponent<Property>().m_State);
        m_GetStat.FromOnDamage(Player.GetComponent<PlayerManager>().GetStat().OnAttack());
        // 사운드 플레이
        AttackSound(Player);

        int HitNum = Random.Range(0, 2);
        if (Player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attack4") && !AttackEffectCheck)
        {
            AttackEffectCheck = true;
            Camera.main.GetComponentInParent<MouseOrbit>().DoShake(1f, 0.1f);
            EffectManager.Instance.PlayEnemyHitEffect(transform, HitNum);
        }
        else
        {
            AttackEffectCheck = false;
            Camera.main.GetComponentInParent<MouseOrbit>().DoShake(0.3f, 0.05f);
            if (Player.GetComponent<PlayerManager>().GetWeapon().WeaponID == 1)
                EffectManager.Instance.PlayEnemyHitEffect(transform, HitNum);
            else if (Player.GetComponent<PlayerManager>().GetWeapon().WeaponID == 2)
                EffectManager.Instance.PlayEnemyHitEffect(transform, 2);
        }
        if (this is MiddleEnemy)
        {
            WaitForAnimation();
        }
        m_EnemyMove.m_Animator.applyRootMotion = true;
        if (m_GetStat.Health <= 0)
        {
            if (GameManager.Instance.NowGameSpawner() != null)
            {
                foreach (var item in GameManager.Instance.NowGameSpawner().m_AllEnemy)
                {
                    if (item == gameObject)
                    {
                        GameManager.Instance.NowGameSpawner().m_AllEnemy.Remove(item);
                        break;
                    }
                }
            }
            //            m_Property.PropertyEnemyDie(Player.GetComponent<Property>());   // 몬스터가 죽었을 경우 차지
            SetDie();       // 다이 모션.

        }
        else if (m_GetStat.Health > 0 && !m_GroggyStunCheck)
        {
            if (!m_EnemyMove.m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
            {
                m_EnemyMove.m_Animator.SetInteger("IsHitNum", Random.Range(0, 4));
                if(this is MiddleEnemy)
                    SetHit();           // 히트 모션.
                if (this is Enemy)
                    if (Player.GetComponent<PlayerManager>().GetWeapon().WeaponID != 1)
                        SetHit();
                if (this is BossEnemy)
                    if(!m_AttackCheck)
                        SetHit();
            }
            //                m_Property.PropertySkill(Player.GetComponent<Property>());      // 확률 차지
            //                m_Property.PropertySkillGauge(Player.GetComponent<Property>()); // 게이지 차지
        }
    }

    public CUnit GetStat()
    {
        return m_GetStat;
    }

    public void OnAttackCheck() { GetComponentInChildren<BoxCollider>().enabled = true; }
    public void CloseAttackCheck() { GetComponentInChildren<BoxCollider>().enabled = false; }

    IEnumerator AttackStart1()
    {
        while (m_IsDie)
        {
            if (!GameManager.Instance.isGameOver)
            {
                if (m_State == State.TRACE && !m_AttackDelayCheck)
                {
                    int num = Random.Range(1, m_Skill.Count);


                    if (m_TargetDist <= m_Skill[0].AttackDist && m_TargetDist > 0 &&
                         !m_Skill[0].SkillCheck)
                    {
                        nowSkill = m_Skill[0];
                        m_AttackCheck = true;
                        m_Skill[0].Skill();
                        m_SkillCount = m_Skill[0].SkillNumber;
                        m_AttackDelayCheck = true;

                        m_NowAttackKind = m_Skill[0].m_AttackKind;      // 현재 공격 종류
                        //                    Debug.Log("기본 몬스터가 어택");
                    }
                    else if (m_TargetDist <= m_Skill[num].AttackDist && m_TargetDist > m_Skill[num - 1].AttackDist &&
                         !m_Skill[num].SkillCheck)
                    {
                        nowSkill = m_Skill[num];
                        m_AttackCheck = true;
                        m_Skill[num].Skill();
                        m_SkillCount = m_Skill[num].SkillNumber;
                        m_AttackDelayCheck = true;

                        m_NowAttackKind = m_Skill[num].m_AttackKind;
                        //                    Debug.Log("몬스터가 어택");
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void WaitForAnimation()
    {
        StartCoroutine(Delay());
    }

    public abstract IEnumerator Delay();

    public void StunCoroutine()
    {
        CancelStun = StartCoroutine(Stun());
    }

    public IEnumerator Stun()
    {
        m_StunCheck = true;
        m_OnRush = false;
        if (nowSkill != null) 
            nowSkill.SkillCancel = true;
//        Debug.Log("스턴");
        yield return new WaitForSeconds(3);
        m_StunCheck = false;
        if (nowSkill != null)
            nowSkill.SkillCancel = false;
        m_AttackDelayCheck = false;
//        Debug.Log("스턴해제");
    }

    public void GroggyStunCoroutine()       // 그로기 상태 실행
    {
        StartCoroutine(GroggyStun(8));
    }

    public void FootStunCoroutine()
    {
        if(CancelStun != null)
            StopCoroutine(CancelStun);
        if (FootStun != null)
            StopCoroutine(FootStun);
        FootStun = StartCoroutine(GroggyStun(m_GroggyStunCoolTime));
    }

    public IEnumerator GroggyStun(float time)
    {
//        m_EnemyMove.m_Animator.SetBool("IsGroggyCheck", true);
        m_EnemyMove.m_Animator.SetTrigger("IsGroggy");
        m_EnemyMove.m_Animator.SetBool("IsStun", true);
        m_StunCheck = true;
        m_GroggyStunCheck = true;
        m_OnRush = false;
        m_AttackCheck = false;
        m_EnemyMove.OnAttack(m_AttackCheck, 0);
        if (nowSkill != null)
            nowSkill.SkillCancel = true;
//        Debug.Log("그로기스턴");
        yield return new WaitForSeconds(time);

        m_StunCheck = false;
        m_GroggyStunCheck = false;
        if (nowSkill != null)
            nowSkill.SkillCancel = false;
        m_AttackDelayCheck = false;
//        m_EnemyMove.m_Animator.SetBool("IsGroggyCheck", false);
        m_EnemyMove.m_Animator.SetBool("IsStun", false);
        //        Debug.Log("스턴해제");
    }

    public void PlayerLookAt()
    {
        Quaternion lookAt = Quaternion.identity;    // Querternion 함수 선언.
        Vector3 lookatVec = (m_TargetTr.transform.position - transform.position).normalized; //타겟 위치 - 자신 위치 -> 노멀라이즈.
        lookAt.SetLookRotation(lookatVec);  // 쿼터니언의 SetLookRotaion 함수 적용.

        iTween.RotateTo(gameObject, new Vector3(0, lookAt.eulerAngles.y, 0), 1f);
    }

    public void IsAttackMoving()
    {
        m_isAttackMoving = true;
    }
}