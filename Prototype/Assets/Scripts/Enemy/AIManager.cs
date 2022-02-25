using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AINavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class AIManager : MonoBehaviour
{
    enum MoveType { MovePosition, Velocity, Position, NULL };
    [SerializeField]
    MoveType m_MoveType = MoveType.NULL;

    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;
    [SerializeField]
    float m_MoveSpeed = 0.5f;
    [SerializeField]
    float m_AnimSpeed = 1f;

    [HideInInspector]
    public Animator m_Animator;
    [HideInInspector]
    public Rigidbody m_Rigidbody;
    bool m_IsGrounded;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    bool m_Attack;
    int m_AttackNum;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
        //        m_NvAgent = GetComponent<NavMeshAgent>();
        m_IsGrounded = true;
        m_Animator.applyRootMotion = false;
        //        m_NvAgent.updatePosition = true;
        //        m_NvAgent.updateRotation = false;
    }


    public void Move(Vector3 move)
    {
        m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        if (!m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
        {
            m_Animator.applyRootMotion = false;
            ApplyExtraTurnRotation();
            ApplyExtraMovement();
        }else
        {
            m_Animator.applyRootMotion = true;
        }

        UpdateAnimator(move);
    }



    void UpdateAnimator(Vector3 move)
    {
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
        m_Animator.SetBool("OnGround", m_IsGrounded);
        m_Animator.SetBool("IsAttack", m_Attack);
        m_Animator.SetFloat("IsAttackNum", m_AttackNum);

        m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeed;
        }
        else
        {
            m_Animator.speed = 1;
        }
    }

    public void ApplyExtraMovement()
    {
        Vector3 v;
        switch (m_MoveType)
        {
            case MoveType.MovePosition:
                v = m_Rigidbody.position + transform.forward * Mathf.Abs(m_ForwardAmount) * Time.deltaTime * m_MoveSpeed;
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

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }


    //public void OnAnimatorMove()
    //{
    //    if (m_IsGrounded && Time.deltaTime > 0)
    //    {
    //        Vector3 v = (m_Animator.deltaPosition * m_MoveSpeed) / Time.deltaTime;
    //        v.y = m_Rigidbody.velocity.y;
    //        m_Rigidbody.velocity = v;
    //    }
    //}

    public void OnAttack(bool attack, int num)
    {
        m_Attack = attack;
        m_AttackNum = num;
    }
}
