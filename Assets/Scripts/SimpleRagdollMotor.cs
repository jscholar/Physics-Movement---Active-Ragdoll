using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleRagdollMotor : MonoBehaviour
{
    private Rigidbody rootRB;
    private List<Rigidbody> ragdollRigidbodies;
    private float totalMass;
    private JumpMotor jumpMotor;

    [SerializeField]
    private PlayerInputRouter inputRouter;

    [SerializeField]
    private RagdollCharacteristicSO ragdollMotorCharacteristic;

    private Vector3 _debugTau;

    private void Awake()
    {
        this.rootRB = GetComponent<Rigidbody>();
        this.ragdollRigidbodies = Utils.GetAllRigidbodies(this.gameObject);
        this.totalMass = MassUtils.GetTotalMass(this.gameObject);

        JumpMotor jumpMotor = this.transform.AddComponent<JumpMotor>();
    }

    private void FixedUpdate()
    {
        if (inputRouter.CurrentDebugIntent.balloonPressed)
        {
            TestBalloon();
        }
    }


    
    private void OnDrawGizmos()
    {
        Rigidbody spine = this.transform.Find("Character1_Spine").GetComponent<Rigidbody>();

        Gizmos.color = Color.red;
        Gizmos.DrawRay(spine.position, spine.position - spine.transform.right * 2000f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(spine.position, spine.position + Vector3.up * 2000f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(spine.position, spine.position + _debugTau * 2000f);

        Gizmos.color = Color.blue;
        Vector3 removedYaw = -spine.transform.right;
        removedYaw.x = 0;
        Gizmos.DrawRay(spine.position, spine.position + removedYaw * 2000f);
    }

    private void TestBalloon()
    {
        float force = totalMass * Physics.gravity.magnitude;
        rootRB.AddForce(Vector3.up * force, ForceMode.Force);
    }
}
