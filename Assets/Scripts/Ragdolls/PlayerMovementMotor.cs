using UnityEngine;

public class PlayerMovementMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    [SerializeField]
    private PlayerMovementCharacteristicSO locoChar;

    private Vector3 prevVelocity = Vector3.zero;

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        Vector3 IntentVector = IntentToWorldSpaceVector(context);
        Vector3 horizontalVelocity = GetHorizontalVelocity(context);
        Vector3 errorVector = IntentVector * locoChar.maxSpeed - horizontalVelocity;

        Vector3 proportionalComponent = locoChar.kpVelocity * errorVector * context.totalMass;
        Vector3 derivativeComponent = locoChar.kdVelocity * (horizontalVelocity - prevVelocity) / Time.fixedDeltaTime;

        Vector3 steeringForce = proportionalComponent - derivativeComponent;

        prevVelocity = horizontalVelocity;
        output.locomotiveForce += steeringForce;
    }

    private Vector3 IntentToWorldSpaceVector(RagdollContext context)
    {
        PlayerInputRouter inputRouter = context.inputRouter;

        return new Vector3(
            inputRouter.CurrentPlayerMoveIntent.moveDir.x,
            0,
            inputRouter.CurrentPlayerMoveIntent.moveDir.y);
    }

    private Vector3 GetHorizontalVelocity(RagdollContext context)
    {
        return new Vector3(
            context.rootRigidBody.linearVelocity.x,
            0,
            context.rootRigidBody.linearVelocity.z);
    }
}
