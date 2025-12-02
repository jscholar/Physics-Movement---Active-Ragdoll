using UnityEngine;

public class JumpMotor : RagdollMotor
{
    private float lastJumpTime = -Mathf.Infinity;

    // Update is called once per frame
    public override void UpdateMotor(RagdollContext context, MotorOutput motorOutput)
    {
        if (!IsCanJump(context)) {
            return;
        }

        lastJumpTime = Time.time;

        LocalizedForce jumpForce = new LocalizedForce
        {
            _debug_ForceName = "JumpForce",
            force = Vector3.up * GetForceMagnitude(),
            forceType = ForceType.Force,
            forceMode = ForceMode.Impulse,
            rb = context.rootRigidBody
        };

        motorOutput.localizedForces.Add(jumpForce);
    }

    private float GetForceMagnitude()
    {
        return 100f;
    }


    private bool IsCanJump(RagdollContext ragdoll)
    {
        bool jumpPressed = ragdoll.inputRouter.CurrentPlayerMoveIntent.jumpPressed;
        if (!jumpPressed) return false;

        bool isGrounded = Physics.Raycast(ragdoll.rootRigidBody.position, Vector3.down, 3.0f, LayerMask.GetMask("Ground"));
        if (!isGrounded) return false;

        bool isOnCooldown = (Time.time - lastJumpTime) < 0.5f;
        if (isOnCooldown) return false;

        return true;
    }
}
