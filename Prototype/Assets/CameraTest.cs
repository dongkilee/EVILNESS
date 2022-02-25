using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour {

    [SerializeField]
    float m_MovingTurnSpeed = 360;         // 달리기 턴 움직임 스피드.
    [SerializeField]
    float m_StationaryTurnSpeed = 180;      // 가만히 턴 움직임 스피드.
    [SerializeField]
    float m_MoveSpeed = 1f;      // 캐릭터 스피드.
    [SerializeField]
    float m_TurnSpeed = 10f;
    private Vector3 m_Move;
    private Vector3 m_CamForward;
    float m_TurnAmount;
    float m_ForwardAmount;

    public float distance = 3.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float LerpSpeed = 5;
    public float LerpMoveSpeed = 1;

    public float ZoomInMaxTime;
    public float ZoomInTime;

    public float Responsiveness = 0.01f;    // 마우스 민감도

    float x = 0;
    float y = 0;
    float LerpDistance;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Mouse wheel down
            if (distance < 4f)
                distance += 0.1f;
            else
                distance = 4f;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Mouse wheel up
            if (distance > 2f)
                distance -= 0.1f;
            else
                distance = 2f;
        }

        x += Input.GetAxis("Mouse X") * xSpeed * Responsiveness;
        y -= Input.GetAxis("Mouse Y") * ySpeed * Responsiveness;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        var rotation = Quaternion.Euler(y, x, 0);
        transform.rotation = rotation;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        m_CamForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        m_Move = v * m_CamForward + h * transform.right;
        //        m_Move = v * Vector3.forward + h * Vector3.right;

        Move(m_Move);

    }

    public void Move(Vector3 move)
    {
        Vector3 _move = move;
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z * 2;

        Vector3 v;
        v = transform.forward * Mathf.Abs(m_ForwardAmount) * Time.deltaTime * m_MoveSpeed;
        transform.position += v;

        if(_move != Vector3.zero)
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_move), Time.deltaTime * m_TurnSpeed);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle <= -360)
            angle += 360;
        if (angle >= 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
