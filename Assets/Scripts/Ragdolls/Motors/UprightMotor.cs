using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StabilizedJointEntry
{
    public string _debugName;
    public Rigidbody rb;
    public UprightJointMotorCharacteristicSO characteristic;
    public Vector3 localUp = Vector3.up;
}

public class UprightMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    [SerializeField]
    private List<StabilizedJointEntry> joints = new();

    [SerializeField]
    bool debugEnabled = false;
    Vector3 desiredOrientation = Vector3.up;
    Vector3 debugLine = Vector3.zero;

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        foreach(StabilizedJointEntry joint in this.joints) {
            Vector3 torque = GetJointTorque(joint);

            output.localizedForces.Add(new LocalizedForce()
            {
                _debug_ForceName = joint._debugName,
                force = torque,
                forceType = ForceType.Torque,
                forceMode = ForceMode.Force,
                rb = joint.rb
            });
        }
    }

    private Vector3 GetJointTorque(StabilizedJointEntry joint)
    {
        Vector3 actualLocalUp = joint.rb.transform.TransformDirection(joint.localUp);
        Quaternion qErr = Quaternion.FromToRotation(actualLocalUp, Vector3.up);

        qErr.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        Vector3 angularError = axis * Mathf.Deg2Rad * angle;
        Vector3 tau = (joint.characteristic.kp * angularError);

        return tau;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawLine(this.transform.position, this.transform.position + debugLine);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.up);
    }
}
