using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : Singleton<ObstacleManager> {

    // 모든 장애물 찾기
    public static List<Obstacle> Obstacles = new List<Obstacle>();
    public int ObstaclesCount;
	// Use this for initialization
	void Start () {
        Obstacles = new List<Obstacle>();
        // 장애물 스크립트를 가지고 있는 얘들을 찾아서 담아준다.
        Obstacle[] ObstacleArray = FindObjectsOfType<Obstacle>();
        for(int i = 0;i<ObstacleArray.Length; i++)
        {
            Obstacle newObst = ObstacleArray[i];
            // 스크립트가 켜져있고 게임오브젝트가 켜져있을 경우만 리스트에 담아준다.
            if (newObst.isActiveAndEnabled && newObst.gameObject.activeInHierarchy)
                Obstacles.Add(newObst);
        }

    }

    private void Update()
    {
        ObstaclesCount = Obstacles.Count;
    }

    public static float CalculateObstacleAvoidance(ref Vector3 move, IAvoider avoider)
    {
        
        Vector3 MyPos = avoider.transform.position;
        float RepulseDistance = Mathf.Pow(avoider.avoidanceDistance, 2);
        Vector3 MyForward = avoider.transform.forward;

        float MyPower = avoider.avoidancePower;
        Vector3 Force = Vector3.zero;
        float MyAngle = avoider.avoidanceAngle;
        float MyStr = avoider.avoidanceStrength;

        for (int i = 0; i < Obstacles.Count; i++)
        {
            Obstacle ObstacleBot = Obstacles[i];
            if (!ObstacleBot) continue;
            if (avoider.ownObstacle == ObstacleBot) continue;
            if (!ObstacleBot.enabled) continue;
            if (!ObstacleBot.gameObject.activeInHierarchy) continue;

            Vector3 ToOther;
            Vector3 DirToOther;
            float DistToOther;

            switch (ObstacleBot.obstacleType)
            {
                case ObstacleType.Point:
                    {
                        ToOther = ObstacleBot.transform.position - MyPos;
                        DirToOther = ToOther.normalized;
                        DistToOther = ToOther.sqrMagnitude;
                        //                    Debug.DrawLine(MyPos, ObstacleBot.transform.position, Color.white);
                    }
                    break;
                case ObstacleType.Line:
                    {
                        Vector3 ClosestPoint;
                        Vector3 A = ObstacleBot.pointA.position;
                        Vector3 B = ObstacleBot.pointB.position;
                        Vector3 P = MyPos;

                        Vector3 AP = P - A;       
                        Vector3 AB = B - A;    

                        float magnitudeAB = Vector3.SqrMagnitude(AB);
                        float ABAPproduct = Vector3.Dot(AP, AB);   
                        float distance = ABAPproduct / magnitudeAB; 

                        if (distance < 0)     
                        {
                            ClosestPoint = A;

                        }
                        else if (distance > 1)
                        {
                            ClosestPoint = B;
                        }
                        else
                        {
                            ClosestPoint = A + AB * distance;
                        }

                        ToOther = ClosestPoint - MyPos;
                        DirToOther = ToOther.normalized;
                        DistToOther = ToOther.sqrMagnitude;
                    }
                    break;
                case ObstacleType.Sphere:
                    {
                        Vector3 ClosestPoint = ObstacleBot.transform.position - MyPos;
                        DirToOther = ClosestPoint.normalized;
                        ClosestPoint = ObstacleBot.transform.position - ClosestPoint.normalized * (ObstacleBot.radius * 0.5f);

                        ToOther = ClosestPoint - MyPos;
                        DistToOther = ToOther.sqrMagnitude;
                    }
                    break;
                default:
                    ToOther = ObstacleBot.transform.position - MyPos;
                    DirToOther = ToOther.normalized;
                    DistToOther = ToOther.sqrMagnitude;
                    break;
            }

            if (DistToOther < RepulseDistance)
            {
                float Angle = Vector3.Angle(MyForward, DirToOther);
                if(Angle < MyAngle)
                {
//                    Debug.DrawLine(MyPos, MyPos + ToOther, Color.green);

                    Vector3 Refl = Vector3.Reflect(MyForward, DirToOther);
                    float InvDist = Mathf.Clamp01(DistToOther / RepulseDistance);
                    InvDist = 1 - Mathf.Pow(InvDist, MyPower);
                    InvDist *= MyStr;
                    float CurInfluence = InvDist * ObstacleBot.repulsion;
                    Force += Refl * CurInfluence;
                }
            }
        }

        move = ((move * avoider.persistance) + Force).normalized;
        return move.magnitude;
    }
}
