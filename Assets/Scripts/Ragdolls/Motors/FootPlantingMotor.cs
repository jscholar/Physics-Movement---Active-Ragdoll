using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class FootPlantingMotor : RagdollMotor
{
    [SerializeField]
    Transform leftFoot;

    [SerializeField]
    Transform rightFoot;

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
    }
}
