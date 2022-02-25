using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArrowNavi : MonoBehaviour {

    public GameObject m_Target;
    public GameObject m_Player;
    public NavMeshPath m_Path;
    public Transform[] m_Box;
    public List<Transform> m_MovingPath;
    public int m_WalkArea;

    [SerializeField]
    float m_MovingTurnSpeed = 360;
    [SerializeField]
    float m_StationaryTurnSpeed = 180;

    float m_TurnAmount;
    float m_ForwardAmount;

    private void Awake()
    {
        m_Path = new NavMeshPath();
        m_MovingPath = new List<Transform>();
        m_WalkArea = 1 << NavMesh.GetAreaFromName("Walkable");

        for(int i = 0; i< m_Box.Length; i++)
        {
            m_MovingPath.Add(m_Box[i]);
        }
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(Moving());
	}

    private void Update()
    {
        transform.position = Vector3.Lerp(m_Player.transform.position - transform.up + Vector3.up*0.5f, transform.position , 0);
    }

    // Update is called once per frame
    IEnumerator Moving()
    {
        while (true)
        {
            Move((m_MovingPath[0].position - m_Player.transform.position).normalized);
            TargetFromDraw();
            yield return new WaitForFixedUpdate();
        }
    }

    public void Move(Vector3 move)
    {

        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);

        ApplyExtraTurnRotation();

    }

    void ApplyExtraTurnRotation()
    {
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, 0);
        transform.RotateAround(m_Player.transform.position, Vector3.up, turnSpeed  * m_TurnAmount * Time.fixedDeltaTime);
        
    }


    public void TargetFromDraw()
    {
        if (m_MovingPath.Count == 0) return;
        for (int i = 0; i < m_MovingPath.Count - 1; i++)
        {
            Debug.DrawLine(m_MovingPath[i].position, m_MovingPath[i + 1].position, Color.red);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Nav"))
        {
            if(m_MovingPath[0].name == other.name)
            {
                m_MovingPath.Remove(m_MovingPath[0]);
            }
        }
    }
}
