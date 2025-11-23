using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRbMotor : MonoBehaviour
{
    [SerializeField]
    private PlayerInputRouter inputRouter;

    public float maxSpeed = 4f;
    public float accel = 18f;
    public float airControl = 0.3f;
    public float jumpVel = 5.5f;
    public float coyoteTime = 0.12f;

    Rigidbody rb;
    float lastGroundedTime;

    void Awake() { rb = GetComponent<Rigidbody>(); rb.constraints = RigidbodyConstraints.FreezeRotation; }

    void FixedUpdate()
    {
        var intent = inputRouter.CurrentPlayerMoveIntent;

        // Horizontal steering
        Vector3 v = rb.linearVelocity; Vector3 horiz = new(v.x, 0f, v.z);
        Vector3 target = new Vector3(intent.moveDir.x, 0, intent.moveDir.y) * maxSpeed;
        Vector3 a = (target - horiz) * (accel);
        rb.AddForce(new Vector3(a.x, 0f, a.z), ForceMode.Acceleration);

        // Jump (buffer already handled by router), add coyote on motor side
        bool canCoyote = Time.time <= lastGroundedTime + coyoteTime;
        if (intent.jumpPressed) {
            Vector3 vel = rb.linearVelocity; vel.y = 0f; rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpVel, ForceMode.VelocityChange);
        }
    }
}
