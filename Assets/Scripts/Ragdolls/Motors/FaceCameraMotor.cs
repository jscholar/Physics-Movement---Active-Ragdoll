using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

// Faces **away** from the camera, but you get the idea
public class FaceCameraMotor : RagdollMotor
{
    [SerializeField]
    private Camera cam;

    private Vector3 debugFacing;

    public float kp = 300f;
    public float kd = 25f;
    public float maxTorque = 250f;
    public float deadZoneDeg = 2f;

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        Vector3 desired = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized; // camera forward projected onto ground plane
        debugFacing = desired;

        Vector3 current = Vector3.ProjectOnPlane(context.rootRigidBody.transform.forward, Vector3.up).normalized;

        float errDeg = Vector3.SignedAngle(current, desired, Vector3.up);
        if (Mathf.Abs(errDeg) < deadZoneDeg) return;

        float errRad = errDeg * Mathf.Deg2Rad;
        float yawVel = Vector3.Dot(context.rootRigidBody.angularVelocity, Vector3.up); // rad/s

        float torqueY = Mathf.Clamp(kp * errRad - kd * yawVel, -maxTorque, maxTorque);

        output.localizedForces.Add(new LocalizedForce
        {
            _debug_ForceName = "FaceCameraMotor_TorqueY",
            force = Vector3.up * torqueY,
            forceType = ForceType.Torque,
            forceMode = ForceMode.Acceleration,
            rb = context.rootRigidBody
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + debugFacing * 100f);
    }
}
