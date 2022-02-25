using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class PlayerManager : MonoBehaviour
{

    enum MoveType { MovePosition, Velocity, Position, NULL };
    [SerializeField]
    MoveType m_MoveType = MoveType.NULL;
    MoveType m_CurrentType;
    CMan m_GetStat;


    CWeapon m_Weapon = new CW_Sword();
    public bool[] bWeaponActive;
    //static bool[] weaponActive = new bool[m_Weapon.WeaponID];

    [SerializeField]
    CMan m_Player = new CMan();
    [SerializeField]
    float m_MovingTurnSpeed = 360;         // 달리기 턴 움직임 스피드.
    [SerializeField]
    float m_StationaryTurnSpeed = 180;      // 가만히 턴 움직임 스피드.
    [SerializeField]
    float m_JumpPower = 12f;                   // 점프 파워.
    [Range(1f, 4f)]
    [SerializeField]
    float m_Gravity = 2f;     // 중력.
    [SerializeField]
    float m_MoveSpeed = 1f;      // 캐릭터 스피드.
    public float m_AnimSpeed = 1f;      // 애니메이션 스피드.
    [SerializeField]
    float m_GroundDistance = 0.1f;        // 바닥과의 거리 체크.
    [SerializeField]
    float m_DashDistance = 5;

    Rigidbody m_Rigidbody;
    [HideInInspector]
    public Animator m_Animator;
    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    CapsuleCollider m_Capsule;
    bool m_Crouching;
    bool m_Attack;
    bool m_ComboAttack;

    public Transform LeftFoot;
    public Transform RightFoot;

    [HideInInspector]
//    public Property m_Property;
    Vector3 VectorMove;
    public float m_TurnSpeed;
    public Collider m_WeaponCollider;
    public Collider m_HammerCollider;
    AudioSource m_Audio;

    int m_WeaponCount;

    void Awake()
    {
        m_GetStat = m_Player;
        m_GetStat.Init();
    }

    // Use this for initialization
    void Start()
    {
        m_CurrentType = m_MoveType;
        m_Audio = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
//        m_Property = GetComponent<Property>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundDistance;

        m_WeaponCount = m_Weapon.WeaponID;
        bWeaponActive = new bool[10];
        bWeaponActive[0] = true;
        for (int i = 1; i < bWeaponActive.Length; i++)
        {
            bWeaponActive[i] = false;
        }
        m_WeaponCollider.gameObject.SetActive(true);
        m_HammerCollider.gameObject.SetActive(false);

        m_GetStat.setWeapon(m_Weapon);
        StartCoroutine(ComboAttack());

    }

    void Update()
    {
//        m_GetStat.FeverTime();
        if(GameManager.Instance.CopyGameTutorial == GameManager.Tutorial.ATTACK && m_GetStat.m_Combo >= 10)
        {
            GameManager.Instance.TutorialChange();   
        }
        m_GetStat.TimeCancelCombo();

        if (GameManager.Instance.isGameOver)
        {
            if (!m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsName("Die"))
            {
                m_Animator.SetTrigger("IsDie");
            }
        }
    }

    public float m_Stepheight;

    public void Move(Vector3 move, bool jump, bool attack, bool dash)
    {
        if (GameManager.Instance.isGameOver) return;
//        m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        //        Debug.DrawRay(transform.position + Vector3.up, transform.forward * m_DashDistance, Color.green);
        VectorMove = move;
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
//        move = Vector3.ProjectOnPlane(move, m_GroundNormal);

        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z * 2;
        RaycastHit hitInfo;
//        Debug.DrawLine(transform.position + (Vector3.up * 0.1f) - (transform.forward * 0.2f), transform.position + (Vector3.up * 0.1f) + (transform.forward)*2, Color.green);
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f) - (transform.forward * 0.2f), transform.forward, out hitInfo, 2f))
        {
            if (hitInfo.transform.CompareTag("Box"))
            {
                //                m_Rigidbody.MovePosition(m_Rigidbody.position + Vector3.up * Mathf.Abs(m_ForwardAmount) * Time.fixedDeltaTime * m_Stepheight);
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y + Time.fixedDeltaTime * m_Stepheight * Mathf.Abs(m_ForwardAmount), m_Rigidbody.velocity.z);
            }
        }


        if (m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Ground"))
        {
            if (!m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Air"))
                ApplyExtraMovement();   // 이동

            ApplyExtraTurnRotation();       // 회전
        }
        
        if (m_IsGrounded)
        {
            OnDash(dash, attack, jump);
//            if (dash) m_Animator.SetTrigger("IsDash");  // 대쉬
            //if(!m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Dash"))
            //     HandleGroundedMovement(dash, jump); // 점프
  
        }
        //else
        //{
        //    HandleAirborneMovement();   // 중력
        //}
        //        ScaleCapsuleForCrouching(crouch);
        //        PreventStandingInLowHeadroom();
        if (!m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Air") &&
            !m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Dash"))
            OnAttack(dash, attack); // 공격
        UpdateAnimator(move); // 애니메이션
    }

    float time = 0.1f;
    void UpdateAnimator(Vector3 move)
    {
        m_Animator.SetFloat("Forward", Mathf.Abs(m_ForwardAmount), 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("Crouch", m_Crouching);
        m_Animator.SetBool("Ground", m_IsGrounded);
        m_Animator.SetBool("Attack", m_Attack);
        m_Animator.SetBool("Combo", m_ComboAttack);
        m_Animator.SetFloat("TestFloat", Mathf.Abs(m_ForwardAmount), 0.1f, Time.deltaTime);
        if (Mathf.Abs(m_ForwardAmount) > 0)
        {
            time = 0.02f;
            m_Animator.SetBool("IsRun", true);
        }
        else
        {
            if(time<0)
                m_Animator.SetBool("IsRun", false);
            else
                time -= Time.deltaTime;
        }
        //if (!m_IsGrounded)
        //{
        //    m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
        //}
        //if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) ||
        //    Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) ||
        //    m_ForwardAmount != 0)
        //{
        //    time = 0.2f;
        //    m_Animator.SetBool("RunCheck", true);
        //}else
        //{
        //    if(time < 0)
        //    m_Animator.SetBool("RunCheck", false);
        //    time -= Time.deltaTime;
        //}

        //float runCycle =
        //    Mathf.Repeat(
        //        m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).normalizedTime, 1);
        //float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
        //if (m_IsGrounded)
        //{
        //    m_Animator.SetFloat("JumpLeg", jumpLeg);
        //}

        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeed;
        }
        else
        {
            m_Animator.speed = m_AnimSpeed;
        }
    }


    void HandleAirborneMovement()
    {
        Vector3 extraGravityForce = (Physics.gravity * m_Gravity) - Physics.gravity;

        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }


    void HandleGroundedMovement(bool dash, bool jump)
    {
        if (jump && !dash && m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Ground") && !m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsName("Dash"))
        {
            m_GroundDistance = 0.01f;
            m_Rigidbody.velocity = new Vector3(VectorMove.x * Mathf.Abs(m_ForwardAmount)/2f * m_MoveSpeed, m_JumpPower, VectorMove.z * Mathf.Abs(m_ForwardAmount)/2 * m_MoveSpeed);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
        }
    }

    void OnDash(bool dash, bool attack, bool jump)
    {
        
        if (dash && !attack && !jump)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        RaycastHit hitInfo;
        m_Animator.SetTrigger("IsDash");
        yield return new WaitForSeconds(0.2f);
        m_Rigidbody.velocity = Vector3.zero;
        Vector3 forward = transform.forward;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hitInfo, m_DashDistance))
        {
            if(!hitInfo.collider.CompareTag("Enemy") && hitInfo.collider.name != "arrow" && !hitInfo.collider.CompareTag("Nav"))
                transform.position += forward * hitInfo.distance;
            else
                transform.position += forward * m_DashDistance;
        }
        else
        {
            transform.position += forward * m_DashDistance;
        }

        SoundManager.Instance.PlaySingle(m_Audio, SoundManager.Instance.Dash);
        
    }

    void OnAttack(bool dash, bool attack)
    {
        //  && m_Animator.GetCurrentAnimatorStateInfo(1).IsName("Ground")
        if (attack && !dash)
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Animator.applyRootMotion = true;
            m_Attack = true;
        }
        else
        {
            m_Attack = false;
        }
    }
    public void ApplyExtraMovement()
    {
        Vector3 v;
        switch (m_MoveType)
        {
            case MoveType.MovePosition:
                m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
                v = transform.position + transform.forward * Mathf.Abs(m_ForwardAmount) * Time.deltaTime * m_MoveSpeed;
                m_Rigidbody.MovePosition(v);
                break;
            case MoveType.Velocity:
                v = transform.forward * Mathf.Abs(m_ForwardAmount) * m_MoveSpeed;
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
                break;
            case MoveType.Position:
                v = transform.forward * Mathf.Abs(m_ForwardAmount) * Time.deltaTime * m_MoveSpeed;
                transform.position += v;
                break;
            case MoveType.NULL:
                break;
        }
        
    }

    public void ApplyExtraTurnRotation()        // 캐릭터 회전
    {
        
        if(VectorMove != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(VectorMove), Time.deltaTime * m_TurnSpeed);
//        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
////        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
//        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime* m_TurnSpeed, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;

//        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundDistance), Color.green);
//        Debug.DrawLine(transform.position + ((Vector3.up + Vector3.forward * 2) * 0.1f), transform.position + ((Vector3.up + Vector3.forward * 2) * 0.1f) + (Vector3.down * m_GroundDistance), Color.green);
//        Debug.DrawLine(transform.position + ((Vector3.up + Vector3.right * 2) * 0.1f), transform.position + ((Vector3.up + Vector3.right * 2) * 0.1f) + (Vector3.down * m_GroundDistance), Color.green);
//        Debug.DrawLine(transform.position + ((Vector3.up + Vector3.left * 2) * 0.1f), transform.position + ((Vector3.up + Vector3.left * 2) * 0.1f) + (Vector3.down * m_GroundDistance), Color.green);
//        Debug.DrawLine(transform.position + ((Vector3.up + Vector3.back * 2) * 0.1f), transform.position + ((Vector3.up + Vector3.back * 2) * 0.1f) + (Vector3.down * m_GroundDistance), Color.green);

        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundDistance) ||
            Physics.Raycast(transform.position + ((Vector3.up + Vector3.left * 2f) * 0.1f), Vector3.down, out hitInfo, m_GroundDistance) ||
            Physics.Raycast(transform.position + ((Vector3.up + Vector3.right * 2f) * 0.1f), Vector3.down, out hitInfo, m_GroundDistance) ||
            Physics.Raycast(transform.position + ((Vector3.up + Vector3.forward * 2f)* 0.1f), Vector3.down, out hitInfo, m_GroundDistance) ||
            Physics.Raycast(transform.position + ((Vector3.up + Vector3.down * 2f) * 0.1f), Vector3.down, out hitInfo, m_GroundDistance))
        {
//            Debug.Log(hitInfo.transform.name);
//            transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            m_Animator.applyRootMotion = false;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            m_Animator.applyRootMotion = false;
        }
        
        if (m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Attack") ||
            m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Hit") ||
            m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsName("Skill1"))
        {
            m_Animator.applyRootMotion = true;
        }else m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);

        if (m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsName("Dash")) m_Animator.applyRootMotion = false;
    }

    //public float StepHeight;
    //void OnCollisionEnter(Collision coll)
    //{
    //    if (coll.gameObject.CompareTag("Box"))
    //    {

    //        foreach (ContactPoint cp in coll.contacts)
    //        {

    //            if (cp.thisCollider == GetComponent<Collider>())
    //            {
    //                //                    Debug.Log();
    //                // cp.normal.y
    //                if (cp.otherCollider.bounds.max.y < StepHeight && cp.otherCollider.bounds.max.y > GetComponent<Collider>().bounds.min.y)
    //                {
    //                    Debug.Log("충돌");
    //                    //Debug.Log(cp.normal);
    //                    //Debug.Log(GetComponent<Collider>().bounds.min.y);
    //                    //                        m_Animator.applyRootMotion = true;
    //                    transform.position = Vector3.MoveTowards(transform.position + transform.up / 10, cp.point, Time.deltaTime);
    //                    //                        m_Rigidbody.MovePosition(Vector3.MoveTowards(m_Rigidbody.position + transform.up / 10, cp.point, Time.deltaTime));
    //                    //m_Rigidbody.MovePosition(transform.position + (transform.up / 10) + (transform.forward) * Time.deltaTime);

    //                }
    //            }
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.isGameOver) return;
        if (other.CompareTag("EnemySkill"))
        {
            m_Animator.applyRootMotion = false;
            EnemySkillBowl Enemy = other.GetComponent<EnemySkillBowl>();
            Enemy.GetComponent<Collider>().enabled = false;
            //            StartCoroutine(m_Property.MyProperty(Enemy.Property));
            Enemy.PlayerAttackHit(transform);
            if(m_Weapon.WeaponID == 2 && m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Attack")) { }
                else
                    PlayerLookAt(Enemy.Enemy.transform, transform, 0);
//            int rand = m_Property.HitProperty(Enemy.Property.m_State);

            // 속성에 따라 데미지 변경.
            //            GetComponent<Property>().TargetPropertyDamage(Enemy.Property.m_State);
            if (Enemy.SkillAdditionDamage != 0)
            {
//                Debug.Log("추가뎀");
                m_GetStat.FromOnDamage(Enemy.Enemy.GetStat().OnAttack() *
                    Enemy.SkillAdditionDamage / 100);       // 데미지 정산.
                                                            // 데미지 = 데미지 * 퍼센트데미지 / 100.
            }
            else
            {
//                Debug.Log("기본뎀");
                m_GetStat.FromOnDamage(Enemy.Enemy.GetStat().OnAttack());       // 데미지 정산.
                                                                                // 데미지 = 데미지 * 퍼센트데미지 / 100.
            }
//            UIManager.Instance.PlayerHPView();
            if (m_GetStat.m_HP <= 0)
            {
                GameManager.Instance.isGameOver = true;
                m_Animator.SetTrigger("IsDie");
                return;
            }
            else
            {
                if (GameManager.Instance.isGameOver) return;
                if (m_Weapon.WeaponID == 2 && m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Attack")) { }
                else
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Animator.SetInteger("IsSetHit", 0);
                    //            rand != -1 &&
                    if (!m_Animator.GetCurrentAnimatorStateInfo(m_Weapon.WeaponID).IsTag("Hit"))
                        m_Animator.SetTrigger("IsHit");
                    
                }

            }
        }else if (other.CompareTag("MapWarp"))
        {
            //if(GameManager.Instance.GameStage == GameManager.Stage.STAGEFINAL)
                StartCoroutine(SceneLoadManager.Instance.BossScene());
        }else if (other.CompareTag("Tutorial"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.TutorialChange();
        }
    }


    public CMan GetStat() { return m_GetStat; }
    public CWeapon GetWeapon() { return m_Weapon; }


    public void PlayerLookAt(Transform From, Transform target, float time)
    {
        Quaternion lookAt = Quaternion.identity;    // Querternion 함수 선언.
        Vector3 lookatVec = (From.position - target.position).normalized; //타겟 위치 - 자신 위치 -> 노멀라이즈.
        lookAt.SetLookRotation(lookatVec);  // 쿼터니언의 SetLookRotaion 함수 적용.

        iTween.RotateTo(gameObject, new Vector3(0, lookAt.eulerAngles.y, 0), time);
    }

    public IEnumerator ComboAttack()
    {
        while (Application.isPlaying)
        {
            if (!m_ComboAttack && m_Attack)
            {
                m_ComboAttack = true;
            }
            yield return new WaitForEndOfFrame();
        }

    }

    public void OnComboAttack()
    {
        m_ComboAttack = false;
    }

    // 현재 적들의 거리를 체크 하여 가까운 오브젝트를 향해 몸을 돌린다.
    // 거리는 공격 범위에서 공격했을 때 몸을 돌린다.
    public Transform TargetFromAttack()
    {
        Transform target = null;
        float dir = 99999;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, 3f);

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.CompareTag("Enemy") || hitColliders[i].gameObject.CompareTag("Stage"))
            {
                float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                if (dir >= distance)
                {
                    dir = distance;
                    target = hitColliders[i].transform;
                }
            }
        }
        return target;
    }

    public void EffectStart(int i)
    {
        EffectManager.Instance.m_effectCheck[i] = true;
    }

    //public void EffectStart(int i)
    //{
    //    EffectManager.Instance.EffectCheck[i] = true;
    //}

    public void AttackRotation()
    {
        if (m_TurnAmount != 0)
        {
            Vector3 relativePos = (transform.position + VectorMove) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            Transform target = TargetFromAttack();
            if (target == null) return;
            PlayerLookAt(target, transform, 0.5f);
        }
    }

    public void OnAttackHit()           // 어택 오버랩스피어
    {
        GetComponent<PlayerControl>().OnAttackCheck();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.5f + Vector3.up, 1.5f);
        for (int i = 0; i < hitColliders.Length; i++)
        { 
            if (hitColliders[i].gameObject.CompareTag("Enemy") || hitColliders[i].gameObject.CompareTag("Stage"))
                hitColliders[i].SendMessage("OnEnemyDamage",gameObject);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + transform.forward * 1.5f + Vector3.up, 1.5f);
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position + Vector3.up, 3f);
    //}

    public void OnAttacking()
    {
        m_Animator.SetBool("IsAttacking", true);
    }

    public void OffAttackKing()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Animator.SetBool("IsAttacking", false);
    }

    public void Audio_Slash1()
    {
        m_Audio.clip = SoundManager.Instance.slash_1;
        SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
    }

    public void Audio_Slash2()
    {
        m_Audio.clip = SoundManager.Instance.slash_2;
        SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
    }

    public void Audio_Slash3()
    {
        m_Audio.clip = SoundManager.Instance.slash_3;
        SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
    }

    public void Audio_Slash4()
    {
        //m_Audio.clip = SoundManager.Instance.slash_4;
        //SoundManager.Instance.SFXSource = m_Audio;
        //SoundManager.Instance.PlaySingle(m_Audio.clip);
        m_Audio.clip = SoundManager.Instance.slash_4;
        SoundManager.Instance.PlaySingle(m_Audio, m_Audio.clip);
        //        m_Audio.PlayOneShot(SoundManager.Instance.slash_4);
        SoundManager.Instance.RandomizeSFX(m_Audio, SoundManager.Instance.shout);
//        m_Audio.PlayOneShot(SoundManager.Instance.shout[UnityEngine.Random.Range(0, 3)]);
    }

    public void PlayDashEffect()
    {
        EffectManager.Instance.m_isDash = true;
    }

    public void FootStep(int footcheck)
    {
        if (!EffectManager.Instance.m_FootStepCheck) {
            SoundManager.Instance.PlayerRandomizeSFX(m_Audio, SoundManager.Instance.BossRoomWalk);
            return;
        }
        SoundManager.Instance.PlayerRandomizeSFX(m_Audio, SoundManager.Instance.WaterWalk);
        GameObject newEffect;
        newEffect = Instantiate(EffectManager.Instance.m_playerFootStep);
        if (footcheck ==0)
            newEffect.transform.position = LeftFoot.position;
        else
            newEffect.transform.position = RightFoot.position;
        newEffect.transform.rotation = Quaternion.identity;
        Destroy(newEffect, 0.9f);
    }

    public void GameOver()
    {
        StartCoroutine(UIManager.Instance.GameFinishPlay(UIManager.Instance.GameOver));
    }

}