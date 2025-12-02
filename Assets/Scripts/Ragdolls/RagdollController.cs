using UnityEngine;

/*
 * Orchestrator for ragdoll physics and behaviors.
 * 
 * Attach as component to a GameObject with a Rigidbody representing the root of the ragdoll.
 */
public class RagdollController : MonoBehaviour
{
    private RagdollMotor[] motors;
    private RagdollContext ragdollContext;

    [SerializeField]
    private PlayerInputRouter inputRouter;

    [SerializeField]
    private RagdollCharacteristicSO ragdollCharacteristic;

    private MotorOutput _debug_UpdateOutput;
    private MotorOutput _debug_FixedUpdateOutput;

    private void Start()
    {
        InitRagdollContext();
        InitRagdollMotors();
    }

    private void Update()
    {
        MotorOutput motorOutput = new MotorOutput();

        foreach (RagdollMotor motor in motors) {
            motor.UpdateMotor(ragdollContext, motorOutput);
        }

        ApplyForces(motorOutput);
        _debug_UpdateOutput = motorOutput;
    }

    private void FixedUpdate()
    {
        UpdateRagdollCoM();
        MotorOutput motorOutput = new MotorOutput();

        foreach (RagdollMotor motor in motors) {
            motor.FixedUpdateMotor(ragdollContext, motorOutput);
        }

        ApplyForces(motorOutput);
        _debug_FixedUpdateOutput = motorOutput;
    }

    private void ApplyForces(MotorOutput motorOutput)
    {
        ResolveLocomotiveForce(motorOutput);
        ResolveLocalizedForces(motorOutput);
    }

    private void ResolveLocomotiveForce(MotorOutput motorOutput)
    {
        this.ragdollContext.rootRigidBody.AddForce(motorOutput.locomotiveForce, ForceMode.Force);
    }

    private void ResolveLocalizedForces(MotorOutput motorOutput)
    {
        foreach (LocalizedForce localizedForce in motorOutput.localizedForces) {
            switch (localizedForce.forceType) {
                case ForceType.Force:
                    localizedForce.rb.AddForce(localizedForce.force, localizedForce.forceMode);
                    break;
                case ForceType.Torque:
                    localizedForce.rb.AddTorque(localizedForce.force, localizedForce.forceMode);
                    break;
            }
        }
    }

    private void InitRagdollContext()
    {
        this.ragdollContext = new RagdollContext
        {
            rootRigidBody = GetComponent<Rigidbody>(),
            rigidbodies = this.transform.parent.GetComponentsInChildren<Rigidbody>(),
            inputRouter = this.inputRouter,
            characteristic = this.ragdollCharacteristic,
            totalMass = MassUtils.GetTotalMass(this.gameObject),
        };
    }

    private void UpdateRagdollCoM()
    {
        this.ragdollContext.worldCoM = MassUtils.GetWorldCenterOfMass(ragdollContext.rigidbodies, ragdollContext.totalMass);
    }

    private void InitRagdollMotors()
    {
        motors = GetComponents<RagdollMotor>();
    }

    private void OnDrawGizmos()
    {
        if (_debug_UpdateOutput == null) return;
        if (_debug_FixedUpdateOutput == null) return;


        // Locomotive Gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            this.ragdollContext.rootRigidBody.position,
            this.ragdollContext.rootRigidBody.position+ this._debug_FixedUpdateOutput.locomotiveForce
        );

        foreach(LocalizedForce force in _debug_FixedUpdateOutput.localizedForces) {
            if (force._debug_ForceName == "Spine Stabilization") {
                Gizmos.color = Color.aliceBlue;
                Gizmos.DrawLine(force.rb.worldCenterOfMass, force.force);
            }
        }

        Gizmos.color = Color.turquoise;
        Gizmos.DrawWireSphere(ragdollContext.worldCoM, 0.2f);
    }
}
