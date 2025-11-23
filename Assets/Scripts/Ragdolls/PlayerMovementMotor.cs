using UnityEngine;

public class PlayerMovementMotor : MonoBehaviour, IFixedUpdateRagdollMotor
{
    [SerializeField]
    private PlayerMovementCharacteristicSO locoChar;

    public void FixedUpdateMotor(RagdollContext context, MotorOutput output)
    {
        if (IsPlayerMovementIntentEmpty(context)) return;

        Vector3 steerVector = IntentToWorldSpaceVector(context);

        Vector3 steeringForce = ScaleSteeringVector(context, steerVector);

        output.locomotiveForce += steeringForce;
    }

    private bool IsPlayerMovementIntentEmpty(RagdollContext context)
    {
        Vector2 moveDir = context.inputRouter.CurrentPlayerMoveIntent.moveDir;
        return moveDir.sqrMagnitude < 0.01f;
    }

    private Vector3 IntentToWorldSpaceVector(RagdollContext context)
    {
        PlayerInputRouter inputRouter = context.inputRouter;

        return new Vector3(
            inputRouter.CurrentPlayerMoveIntent.moveDir.x,
            0,
            inputRouter.CurrentPlayerMoveIntent.moveDir.y);
    }

    private Vector3 ScaleSteeringVector(RagdollContext context, Vector3 scalingVector)
    {
        float scalingForce = context.totalMass * locoChar.maxAcceleration;

        return scalingVector * scalingForce;
    }
}
