using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleRagdollMotor : MonoBehaviour
{
    private Rigidbody rootRB;
    private List<Rigidbody> ragdollRigidbodies;
    private float totalMass;
    private JumpMotor jumpMotor;

    [SerializeField]
    private PlayerInputRouter inputRouter;

    [SerializeField]
    private RagdollMotorCharacteric ragdollMotorCharacteristic;

    private Vector3 _debugTau;

    private void Awake()
    {
        this.rootRB = GetComponent<Rigidbody>();
        this.ragdollRigidbodies = Utils.GetAllRigidbodies(this.gameObject);
        this.totalMass = MassUtils.GetTotalMass(this.gameObject);

        JumpMotor jumpMotor = this.transform.AddComponent<JumpMotor>();
    }

    private void FixedUpdate()
    {
        if (inputRouter.CurrentDebugIntent.balloonPressed)
        {
            TestBalloon();
        }

        // TestBalloon();
        HorizontalLocomotion();
        StabilizeJoints();
    }

    private void HorizontalLocomotion()
    {
        this.PelvisPD();
    }

    private void PelvisPD()
    {
        Vector3 targetVel = new Vector3(inputRouter.CurrentPlayerMoveIntent.moveDir.x, 0, inputRouter.CurrentPlayerMoveIntent.moveDir.y) * ragdollMotorCharacteristic.maxSpeed;
        Vector3 velocityError = targetVel - new Vector3(this.rootRB.linearVelocity.x, 0, this.rootRB.linearVelocity.z);

        // Simple PD controller for velocity
        Vector3 force = 
            (velocityError * this.ragdollMotorCharacteristic.kpVelocity) 
          - (this.ragdollMotorCharacteristic.kdVelocity * this.rootRB.linearVelocity);

        this.rootRB.AddForce(force, ForceMode.Acceleration);
    }

    private void StabilizeJoints()
    {
        foreach (StabilizedJoint joint in ragdollMotorCharacteristic.stabilizedJoints) {
            StabilizeJoint(joint);
        }
    }

    private void StabilizeJoint(StabilizedJoint joint)
    {
        Rigidbody jointRB = this.transform.Find(joint.jointPath).GetComponent<Rigidbody>();
        Vector3 spineUp = -jointRB.transform.right;

        // Remove the "yaw" component
        spineUp.x = 0;

        // Get rotation of jointRB up vector to world up vector
        Quaternion qUp = Quaternion.FromToRotation(-jointRB.transform.right, Vector3.up);

        // Remove yaw component
        Quaternion qDes = qUp * jointRB.rotation;
        Quaternion qErr = qDes * Quaternion.Inverse(jointRB.rotation);

        qErr.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 e = axis * Mathf.Deg2Rad * angle;
        Vector3 tau = (joint.kp * e);
        jointRB.AddTorque(tau, ForceMode.Force);
    }

    private void OnDrawGizmos()
    {
        Rigidbody spine = this.transform.Find("Character1_Spine").GetComponent<Rigidbody>();

        Gizmos.color = Color.red;
        Gizmos.DrawRay(spine.position, spine.position - spine.transform.right * 2000f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(spine.position, spine.position + Vector3.up * 2000f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(spine.position, spine.position + _debugTau * 2000f);

        Gizmos.color = Color.blue;
        Vector3 removedYaw = -spine.transform.right;
        removedYaw.x = 0;
        Gizmos.DrawRay(spine.position, spine.position + removedYaw * 2000f);
    }

    private void TestBalloon()
    {
        Debug.Log("Balloon activated! Total Mass: " + totalMass);
        float force = totalMass * Physics.gravity.magnitude;
        rootRB.AddForce(Vector3.up * force, ForceMode.Force);
    }
}
