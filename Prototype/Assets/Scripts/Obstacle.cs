using UnityEngine;

public enum ObstacleType { Point, Line, Sphere };

public class Obstacle : MonoBehaviour {
    // 움직이는 물체가 밀지 않고 피해가는 계수
    public float repulsion = 1.0f;
    // 장애물 타입
    public ObstacleType obstacleType = ObstacleType.Point;

    // 긴 물체일 경우 앞쪽과 뒷쪽을 포인트로 나눠준다
    [HideInInspector]
    public Transform pointA;

    [HideInInspector]
    public Transform pointB;

    // 장애물의 범위
    [HideInInspector]
    public float radius = 1.0f;

}