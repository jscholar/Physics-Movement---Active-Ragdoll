using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StabilizedJointEntry
{
    public string _debugName;
    public Rigidbody rb;
    public StabilizedJointCharacteristic stabilizedJointCharacteristic;
}

public class StabilizationMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    [SerializeField]
    private List<StabilizedJointEntry> joints = new();

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        foreach(StabilizedJointEntry joint in this.joints) {
            Vector3 torque = GetJointTorque(joint);

            output.localizedForces.Add(new LocalizedForce()
            {
                _debug_ForceName = "Stabilize " + joint._debugName,
                force = torque,
                forceType = ForceType.Torque,
                forceMode = ForceMode.Force,
                rb = joint.rb
            });
        }
    }

    private Vector3 GetJointTorque(StabilizedJointEntry joint)
    {
        Vector3 spineUp = -joint.rb.transform.right;

        // Remove the "yaw" component
        spineUp.x = 0;

        // Get rotation of jointRb up vector to world up vector
        Quaternion qUp = Quaternion.FromToRotation(-joint.rb.transform.right, Vector3.up);

        // Remove yaw component
        Quaternion qDes = qUp * joint.rb.rotation;
        Quaternion qErr = qDes * Quaternion.Inverse(joint.rb.rotation);

        qErr.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 e = axis * Mathf.Deg2Rad * angle;
        Vector3 tau = (joint.stabilizedJointCharacteristic.kp * e);

        return tau;
    }
}
