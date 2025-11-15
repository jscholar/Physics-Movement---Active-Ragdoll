using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRagdollMotor : MonoBehaviour
{
    private Rigidbody rootRB;
    private List<Rigidbody> ragdollRigidbodies;
    private float totalMass;

    [SerializeField]
    private PlayerInputRouter inputRouter;

    [SerializeField]
    private RagdollMotorCharacteric ragdollMotorCharacteristic;

    private void Awake()
    {
        this.rootRB = GetComponent<Rigidbody>();
        this.ragdollRigidbodies = Utils.GetAllRigidbodies(this.gameObject);
        this.totalMass = MassUtils.GetTotalMass(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (inputRouter.CurrentDebugIntent.balloonPressed)
        {
            TestBalloon();
        }

        HorizontalLocomotion();
    }

    private void HorizontalLocomotion()
    {
        this.PelvisPD();
    }

    private void PelvisPD()
    {
        Vector3 targetVel = new Vector3(inputRouter.CurrentPlayerMoveIntent.moveDir.x, 0, inputRouter.CurrentPlayerMoveIntent.moveDir.y) * ragdollMotorCharacteristic.maxSpeed;
        Vector3 velocityError = targetVel - this.rootRB.velocity;

        // Simple PD controller for velocity
        Vector3 force = 
            (velocityError * this.ragdollMotorCharacteristic.kpVelocity) 
          - (this.ragdollMotorCharacteristic.kdVelocity * this.rootRB.velocity);

        this.rootRB.AddForce(force, ForceMode.Acceleration);
    }

    private void UpperStability()
    {

    }

    private void TestBalloon()
    {
        Debug.Log("Balloon activated! Total Mass: " + totalMass);
        float force = totalMass * Physics.gravity.magnitude * 2;
        rootRB.AddForce(Vector3.up * force, ForceMode.Acceleration);
    }
}
