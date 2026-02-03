using UnityEngine;

/*
 * Orchestrator for ragdoll physics and behaviors.
 * 
 * Attach as component to a GameObject with a Rigidbody representing the root of the ragdoll.
 */
public class RagdollController : MonoBehaviour
{
    private RagdollContext ragdollContext;
    private RagdollSensor[] sensors;
    private RagdollMotor[] motors;

    [SerializeField]
    private PlayerInputRouter inputRouter;

    [SerializeField]
    private Rigidbody rootRigidbody;

    [SerializeField]
    private RagdollCharacteristicSO ragdollCharacteristic;

    private MotorOutput _debug_UpdateOutput;
    private MotorOutput _debug_FixedUpdateOutput;

    private void Start()
    {
        InitRagdollContext();
        InitRagdollSensors();
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
        foreach(RagdollSensor sensor in sensors) {
            sensor.GetReading(ragdollContext);
        }

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
            rootRigidBody = this.rootRigidbody ?? this.GetComponent<Rigidbody>(),
            rigidbodies = this.GetComponentsInChildren<Rigidbody>(),
            inputRouter = this.inputRouter,
            characteristic = this.ragdollCharacteristic,
            totalMass = MassUtils.GetTotalMass(this.gameObject),
            readingStore = new ReadingStore()
        };
    }

    private void InitRagdollMotors()
    {
        motors = GetComponents<RagdollMotor>();
    }

    private void InitRagdollSensors()
    {
        sensors = GetComponents<RagdollSensor>();
    }

    private void OnDrawGizmos()
    {
        if (_debug_UpdateOutput == null) return;
        if (_debug_FixedUpdateOutput == null) return;


        if (ragdollContext.readingStore.TryGet<CoMReading>(out var comReading)) {
            Gizmos.color = Color.turquoise;
            Gizmos.DrawWireSphere(comReading.value, 0.3f);
        };
    }
}
