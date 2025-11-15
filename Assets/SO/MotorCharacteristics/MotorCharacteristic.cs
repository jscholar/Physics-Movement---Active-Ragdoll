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
    public float kpNeck;
    public float kdNeck;
    public float maxNeckTorque;

    public float kpSpine;
    public float kdSpine;
    public float maxSpineTorque;

    // Jump
}
