using System.Collections.Generic;
using UnityEngine;

public enum ForceType
{
    Force,
    Torque
}

public struct LocalizedForce
{
    public string _debug_ForceName;
    public Vector3 force;
    public ForceType forceType;
    public ForceMode forceMode;
    public Rigidbody rb;

    public void Log()
    {
        // Plan (pseudocode):
        // 1. Build a readable log message containing:
        //    - _debug_ForceName
        //    - force vector (formatted)
        //    - forceType (Force or Torque)
        //    - forceMode (Unity's ForceMode enum)
        //    - rb info: null or rb.name and rb.mass and associated GameObject name
        // 2. If rb != null, use Debug.Log(message, rb.gameObject) to allow Unity to ping the object in the console.
        // 3. Otherwise use Debug.Log(message).
        //
        // Implementation notes:
        // - Use string interpolation for clarity.
        // - Format floats to reasonable precision for readability.
        // - Keep the message compact but informative.

        string forceStr = force.ToString("F3");
        string rbInfo;
        if (rb != null) {
            // Try to include useful details from the Rigidbody and its GameObject
            string rbName = string.IsNullOrEmpty(rb.name) ? "(no name)" : rb.name;
            string goName = rb.gameObject != null ? rb.gameObject.name : "(no gameObject)";
            rbInfo = $"{rbName}, gameObject={goName}, mass={rb.mass:F3}";
        }
        else {
            rbInfo = "null";
        }

        string message = $"[LocalizedForce] Name='{_debug_ForceName}', Force={forceStr}, Type={forceType}, Mode={forceMode}, RB={rbInfo}, MAG={force.magnitude}";

        if (rb != null && rb.gameObject != null) {
            Debug.Log(message, rb.gameObject);
        }
        else {
            Debug.Log(message);
        }
    }
}

public class MotorOutput
{
    // Force applied to the root rigidbody for steering purposes.
    // APPLED AS A FORCE
    public Vector3 locomotiveForce;

    public List<LocalizedForce> localizedForces;

    public MotorOutput()
    {
        locomotiveForce = Vector3.zero;
        localizedForces = new List<LocalizedForce>();
    }
}

public struct RagdollContext
{
    public Rigidbody rootRigidBody;
    public Rigidbody[] rigidbodies;
    public PlayerInputRouter inputRouter;
    public RagdollCharacteristicSO characteristic;
    public float totalMass;
    public Vector3 worldCoM;
}

interface IRagdollMotor { }

interface IFixedUpdateRagdollMotor : IRagdollMotor
{
    void FixedUpdateMotor(RagdollContext ragdollContext, MotorOutput output);
}

interface IUpdateRagdollMotor : IRagdollMotor
{
    void UpdateMotor(RagdollContext ragdollContext, MotorOutput output);
}

/*
 * Orchestrator for ragdoll physics and behaviors.
 * 
 * Attach as component to a GameObject with a Rigidbody representing the root of the ragdoll.
 */
public class RagdollController : MonoBehaviour
{
    private IRagdollMotor[] motors;
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

        foreach (IRagdollMotor motor in motors) {
            if (!(motor is IUpdateRagdollMotor updateMotor)) continue;

            updateMotor.UpdateMotor(ragdollContext, motorOutput);
        }

        ApplyForces(motorOutput);
        _debug_UpdateOutput = motorOutput;
    }

    private void FixedUpdate()
    {
        UpdateRagdollCoM();
        MotorOutput motorOutput = new MotorOutput();

        foreach (IRagdollMotor motor in motors) {
            if (!(motor is IFixedUpdateRagdollMotor updateMotor)) continue;

            updateMotor.FixedUpdateMotor(ragdollContext, motorOutput);
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
        motors = GetComponents<IRagdollMotor>();
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
