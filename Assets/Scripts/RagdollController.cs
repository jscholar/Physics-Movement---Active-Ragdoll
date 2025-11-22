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
}

public struct MotorOutput
{
    public List<LocalizedForce> localizedForces;
}

public struct RagdollContext
{
    public Rigidbody rootRigidBody;
    public PlayerInputRouter inputRouter;
}

interface IRagdollMotor{ }

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

    private void Start()
    {
        InitRagdollContext();
        InitRagdollMotors();
    }

    private void Update()
    {
        MotorOutput motorOutput = new MotorOutput
        {
            localizedForces = new List<LocalizedForce>()
        };

        foreach (IRagdollMotor motor in motors)
        {
            if (!(motor is IUpdateRagdollMotor updateMotor)) continue;

            updateMotor.UpdateMotor(ragdollContext, motorOutput);
        }

        ApplyForces(motorOutput);
    }

    private void FixedUpdate()
    {
        MotorOutput motorOutput = new MotorOutput
        {
            localizedForces = new List<LocalizedForce>()
        };

        foreach (IRagdollMotor motor in motors) {
            if (!(motor is IFixedUpdateRagdollMotor updateMotor)) continue;

            updateMotor.FixedUpdateMotor(ragdollContext, motorOutput);
        }

        ApplyForces(motorOutput);
    }

    private void ApplyForces(MotorOutput motorOutput)
    {
        foreach (LocalizedForce localizedForce in motorOutput.localizedForces)
        {
            switch (localizedForce.forceType)
            {
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
            inputRouter = this.inputRouter
        };
    }

    private void InitRagdollMotors()
    {
        motors = GetComponents<IRagdollMotor>();
    }
}
