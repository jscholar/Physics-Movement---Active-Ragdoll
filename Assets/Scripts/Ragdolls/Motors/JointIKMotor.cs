using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct JointMapping
{
    public ConfigurableJoint physJoint;
    public Transform ghostTransform;
}

// Drives physJoint towards IK targets
public class JointIKMotor : RagdollMotor
{
    [SerializeField]
    private List<JointMapping> ikDrivenJoints;

    [SerializeField]
    private Transform GhostRigRoot;

    private Dictionary<string, Quaternion> initialLocalRotations = new Dictionary<string, Quaternion>();


    public void Start()
    {
        GetInitialLocalRotations();
    }

    private void GetInitialLocalRotations()
    {
        foreach (var jointMapping in ikDrivenJoints) {
            initialLocalRotations.Add(jointMapping.physJoint.name, jointMapping.physJoint.transform.localRotation);
        }
    }

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        foreach (JointMapping mapping in ikDrivenJoints) {
            SetTargetRotation();
        }
    }

    private void SetTargetRotation()
    {
        foreach (var mapping in ikDrivenJoints) {
            ConfigurableJoint joint = mapping.physJoint;
            Transform ghost = mapping.ghostTransform;
            ConfigurableJointExtensions.SetTargetRotationLocal(joint, ghost.localRotation, initialLocalRotations[joint.name]);
        }
    }
}
