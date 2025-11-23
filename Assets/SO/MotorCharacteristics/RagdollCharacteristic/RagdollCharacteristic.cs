using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RagdollMotorCharacteristic", menuName = "ScriptableObjects/RagdollMotorCharacteristic", order = 1)]
public class RagdollCharacteristicSO : ScriptableObject
{
    // Velocity
    [Header("Velocity")]
    public float maxSpeed;
    public float maxAcceleration;

    // Stability
    public StabilizedJoint[] stabilizedJoints;
}

