using UnityEngine;

public class PlayerMovementMotor : RagdollMotor
{
    [SerializeField]
    private PlayerMovementCharacteristicSO locoChar;

    [SerializeField]
    private Camera cam;

    private Vector3 prevVelocity = Vector3.zero;

    public override void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        Vector3 IntentVector = IntentToCameraRelativeVector(context);
        Vector3 horizontalVelocity = GetHorizontalVelocity(context);
        Vector3 errorVector = IntentVector * locoChar.maxSpeed - horizontalVelocity;

        Vector3 proportionalComponent = locoChar.kpVelocity * errorVector * context.totalMass;
        Vector3 derivativeComponent = locoChar.kdVelocity * (horizontalVelocity - prevVelocity) / Time.fixedDeltaTime;

        Vector3 steeringForce = proportionalComponent - derivativeComponent;

        prevVelocity = horizontalVelocity;
        output.locomotiveForce += steeringForce;
    }

    private Vector3 IntentToCameraRelativeVector(RagdollContext context)
    {
        PlayerInputRouter inputRouter = context.inputRouter;

        Vector3 absoluteDir = new Vector3(
            inputRouter.CurrentPlayerMoveIntent.moveDir.x,
            0,
            inputRouter.CurrentPlayerMoveIntent.moveDir.y);

        Vector3 forward = cam.transform.forward;
        forward.y = 0;

        Vector3 localDir = Quaternion.FromToRotation(Vector3.forward, forward.normalized) * absoluteDir;
        return localDir.normalized;
    }

    private Vector3 GetHorizontalVelocity(RagdollContext context)
    {
        return new Vector3(
            context.rootRigidBody.linearVelocity.x,
            0,
            context.rootRigidBody.linearVelocity.z);
    }
}
