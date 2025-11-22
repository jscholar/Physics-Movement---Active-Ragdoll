using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RagdollMotorCharacteristic", menuName = "ScriptableObjects/RagdollMotorCharacteristic", order = 1)]
public class RagdollMotorCharacteric : ScriptableObject
{
    // Velocity
    [Header("Velocity")]
    public float maxSpeed;
    public float kpVelocity; // Proportional gain for velocity control
    public float kdVelocity; // Derivative gain for velocity control
    public float maxAcceleration;

    // Stability
    public StabilizedJoint[] stabilizedJoints;
}

[CreateAssetMenu(fileName = "StabilizedJoint", menuName = "ScriptableObjects/StabilizedJoint", order = 1)]
public class StabilizedJoint: ScriptableObject
{
    public float kp;
    public float kd;
    public float maxTorque;
    public string jointPath;
}
