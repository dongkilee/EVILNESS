using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class AINavMeshAgent : Obstacle, IAvoider
{
    public NavMeshPath m_Path;
    public bool m_InTargetRange = false;
    public Vector3 m_CurrentDist;
    public int m_WalkArea;
    public Vector3[] m_MovingPath = new Vector3[10];
    int m_MovingPathInx = 0;
    float m_PathDistance = 0f;
    Vector3 m_move = Vector3.zero;

    [SerializeField]
    float m_AvoidanceDistace = 3.0f;
    [SerializeField]
    float m_AvoidanceAngle = 90.0f;
    [SerializeField]
    float m_AvoidanceStrength = 4.0f;
    [SerializeField]
    float m_AvoidancePower = 0.1f;
    [SerializeField]
    float m_AvoidancePersistance = 1.0f;
    public float WaypointDistance = 1.0f;

    public float avoidanceDistance
    {
        get { return m_AvoidanceDistace; }
        set { m_AvoidanceDistace = value; }
    }
    public float avoidanceAngle
    {
        get { return m_AvoidanceAngle; }
        set { m_AvoidanceAngle = value; }
    }
    public float avoidanceStrength
    {
        get { return m_AvoidanceStrength; }
        set { m_AvoidanceStrength = value; }
    }
    public float avoidancePower
    {
        get { return m_AvoidancePower; }
        set { m_AvoidancePower = value; }
    }
    public float persistance
    {
        get { return m_AvoidancePersistance; }
        set { m_AvoidancePersistance = value; }
    }
    public Obstacle ownObstacle
    {
        get { return this; }
    }

    void Awake()
    {
        m_Path = new NavMeshPath();
        m_MovingPath = new Vector3[10];
        m_WalkArea = 1 << NavMesh.GetAreaFromName("Walkable");

        for (int i = 0; i < m_MovingPath.Length; i++)
        {
            m_MovingPath[i] = Vector3.zero;
        }
    }

    public void MovingPath(Vector3 targetpos, float targetdist)
    {
        m_CurrentDist = targetpos;
        m_InTargetRange = targetdist < WaypointDistance;
        if (!m_InTargetRange)
        {
            m_move = Vector3.zero;
            CurrentDirectionMovement(ref m_move);
            LocalAvoidance(ref m_move);

            Vector3 pos = transform.position + Vector3.up;
//            Debug.DrawLine(pos, pos + m_move, Color.blue);
        }
        else
        {
            m_move = Vector3.zero;
        }
    }

    public void TargetFromDraw()
    {
//        if (m_MovingPath.Length == 0) return;
//        for (int i = 0; i < m_MovingPath.Length - 1; i++)
//        {
//            Debug.DrawLine(m_MovingPath[i], m_MovingPath[i + 1], Color.red);
//        }
    }

    // 목표물의 거리 계산
    public void CalculateDistanceTarget()
    {
        float TargetDist = Vector3.Distance(transform.position, m_CurrentDist);
        
    }
    // 현재 이동 방향
    protected bool CurrentDirectionMovement(ref Vector3 movedir)
    {
        Vector3 TransformPos = transform.position;
        if(!NavMesh.CalculatePath(TransformPos, m_CurrentDist, m_WalkArea, m_Path))
        {
            m_InTargetRange = false;
            return false;
        }
        if(m_Path.status != NavMeshPathStatus.PathComplete)
        {
            m_InTargetRange = false;
            return false;
        }
        if(m_Path.corners.Length < 2)
        {
            m_InTargetRange = false;
            return false;
        }

        Vector3 m_CurrentWaypoint = m_Path.corners[1];
        Vector3 Target = m_CurrentWaypoint - TransformPos;
        movedir += Target.normalized;
        m_PathDistance = 0f;
        for (int i = 0; i < m_Path.corners.Length - 1; i++)
        {
            float d = Vector3.Distance(m_Path.corners[i], m_Path.corners[i + 1]);
            m_PathDistance += d;
        }
        m_InTargetRange = m_PathDistance < WaypointDistance;
        return true;
    }
    // 지역 회피
    protected void LocalAvoidance(ref Vector3 movedir)
    {
        ObstacleManager.CalculateObstacleAvoidance(ref movedir, this);
        movedir.Normalize();

        m_MovingPath[m_MovingPathInx] = movedir;
        m_MovingPathInx++;
        if (m_MovingPathInx >= m_MovingPath.Length)
            m_MovingPathInx = 0;
        movedir = getMoving();
    }
    Vector3 getMoving()
    {
        int num = m_MovingPath.Length;
        Vector3 moving = Vector3.zero;
        for (int i = 0; i < num; i++)
        {
            moving += m_MovingPath[i];
        }
        moving /= num;
        return moving;
    }
    public Vector3 Move()
    {
        return m_move;
    }

}
