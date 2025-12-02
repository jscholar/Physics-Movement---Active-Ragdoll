using UnityEngine;

// Applies a Vertical Force to a RB to maintain a vertical distance form the ground
public class StandUpMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    private float _TEMP_desiredDistance = 1.5f; // Todo: Replace with SO
    private float _TEMP_springKp = 1.5f;

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        if (!Physics.Raycast(
            context.rootRigidBody.position,
            Vector3.down,
            out RaycastHit hitInfo,
             _TEMP_desiredDistance,
             LayerMask.GetMask(new string[] { "Ground" })
        )) return;


        if (hitInfo.distance > _TEMP_desiredDistance) return;
        float error = (_TEMP_desiredDistance - hitInfo.distance) / _TEMP_desiredDistance;

        Vector3 verticalForce = Vector3.up * error * _TEMP_springKp * context.totalMass * Physics.gravity.magnitude;

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
