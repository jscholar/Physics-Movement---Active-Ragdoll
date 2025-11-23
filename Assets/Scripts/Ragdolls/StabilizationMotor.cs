using Unity.VisualScripting;
using UnityEngine;

public class StabilizationMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        foreach(StabilizedJoint joint in context.characteristic.stabilizedJoints) {

            // Fix this ASAP!! No call to Find every frame.
            Rigidbody jointRb = context.rootRigidBody.transform.Find(joint.jointPath).GetComponent<Rigidbody>();
            Vector3 torque = GetJointTorque(joint, jointRb);

            output.localizedForces.Add(new LocalizedForce()
            {
                _debug_ForceName = "Stabilize " + joint.jointPath,
                force = torque,
                forceType = ForceType.Torque,
                forceMode = ForceMode.Force,
                rb = jointRb
            });
        }
    }

    private Vector3 GetJointTorque(StabilizedJoint joint, Rigidbody jointRb)
    {
        Vector3 spineUp = -jointRb.transform.right;

        // Remove the "yaw" component
        spineUp.x = 0;

        // Get rotation of jointRb up vector to world up vector
        Quaternion qUp = Quaternion.FromToRotation(-jointRb.transform.right, Vector3.up);

        // Remove yaw component
        Quaternion qDes = qUp * jointRb.rotation;
        Quaternion qErr = qDes * Quaternion.Inverse(jointRb.rotation);

        qErr.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 e = axis * Mathf.Deg2Rad * angle;
        Vector3 tau = (joint.kp * e);

        return tau;
    }
}
