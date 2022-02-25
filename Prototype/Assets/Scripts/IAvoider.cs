using UnityEngine;

public interface IAvoider {
    float avoidanceDistance { get; set; }

    float avoidanceAngle { get; set; }

    float avoidanceStrength { get; set; }

    float avoidancePower { get; set; }

    float persistance { get; set; }

    Transform transform { get; }

    Obstacle ownObstacle { get; }

}
