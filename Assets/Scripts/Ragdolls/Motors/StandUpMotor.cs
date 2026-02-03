using UnityEngine;

public class StandUpMotor : RagdollMotor
{
    [SerializeField]
    private StandupCharacteristicSO characteristic;

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        if (!Physics.Raycast(
            context.rootRigidBody.position,
            Vector3.down,
            out RaycastHit hitInfo,
             characteristic.desiredDistance,
             LayerMask.GetMask(new string[] { "Ground" })
        )) return;


        if (hitInfo.distance > characteristic.desiredDistance) return;
        float error = (characteristic.desiredDistance - hitInfo.distance) / characteristic.desiredDistance;

        Vector3 verticalForce = Vector3.up * error * characteristic.springKp * context.totalMass * Physics.gravity.magnitude;

        output.localizedForces.Add(new LocalizedForce()
        {
            force = verticalForce,
            forceMode = ForceMode.Force,
            forceType = ForceType.Force,
            rb = context.rootRigidBody,
            _debug_ForceName = "Stand Up"
        });
    }
}
