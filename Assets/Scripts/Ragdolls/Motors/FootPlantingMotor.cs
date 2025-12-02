using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class FootPlantingMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    [SerializeField]
    Transform leftFoot;

    [SerializeField]
    Transform rightFoot;

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
    }
}
