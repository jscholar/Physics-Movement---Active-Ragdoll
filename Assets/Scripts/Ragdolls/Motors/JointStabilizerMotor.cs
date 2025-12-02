using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class JointStabilzerEntry
{
    public string _debugName;
    public Joint joint;
    public Rigidbody rb;
    public JointStabilizerCharacteristicSO characteristicSO;

    private Quaternion restQuaternion;
    public Quaternion RestQuaternion { get => restQuaternion; set => restQuaternion = value; }

    public void SetRestQuaternion()
    {
        RestQuaternion = rb.transform.localRotation;
    }
}

public class JointStabilizerMotor : RagdollMotor
{
    [SerializeField]
    List<JointStabilzerEntry> joints;

    private Vector3 _debugTau = Vector3.zero;

    private void Start()
    {
        foreach(JointStabilzerEntry joint in joints) {
            joint.SetRestQuaternion();
        }
    }

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        foreach (JointStabilzerEntry entry in joints) {

            Vector3 relativeTorque = GetRelativeTorque(entry);

            output.localizedForces.Add(new LocalizedForce()
            {
                _debug_ForceName = entry._debugName + ": Hinge",
                force = relativeTorque,
                forceType = ForceType.Torque,
                forceMode = ForceMode.Force,
                rb = entry.rb
            });

            output.localizedForces.Add(new LocalizedForce()
            {
                _debug_ForceName = entry._debugName + ": Hinge",
                force = -relativeTorque,
                forceType = ForceType.Torque,
                forceMode = ForceMode.Force,
                rb = entry.joint.connectedBody
            });
        }
    }

    private Vector3 GetRelativeTorque(JointStabilzerEntry jointEntry)
    {
        Quaternion qErr = jointEntry.RestQuaternion * Quaternion.Inverse(jointEntry.rb.transform.localRotation);

        qErr.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f)
            angle -= 360f;

        Vector3 axisWorld = jointEntry.joint.connectedBody.transform.TransformDirection(axis);

        Vector3 angularError = axisWorld * Mathf.Deg2Rad * angle;
        Vector3 tau =
           jointEntry.characteristicSO.kp
           * angularError;

        return tau;
    }

    private void OnDrawGizmos()
    {
        foreach (var joint in joints) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(joint.rb.position, joint.rb.position + _debugTau);
        }
    }
}
