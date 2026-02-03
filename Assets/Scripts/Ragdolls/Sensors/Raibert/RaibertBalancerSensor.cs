using System;
using UnityEngine;

[Serializable]
public struct RaibertContext
{
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    [SerializeField]
    public RaibertCharacteristicSO characteristicSO;
}

/**
 * Finds desired contact points for a Raibert-style balancer.
 */
public class RaibertBalancerSensor : RagdollSensor
{


    [SerializeField]
    private RaibertContext raibertContext;

    [SerializeField] // Debugging
    private BalanceState currentState;
    private RaibertFSMState raibertFSM;
    
    
    public void Start()
    {
        raibertFSM = new RaibertFSMBalanced();
    }

    public override void GetReading(RagdollContext context)
    {
        var reading = context.readingStore.GetOrCreate<RaibertSensorReading>();
        var state = raibertFSM.Execute(context, raibertContext);

        if (state != currentState) {
            currentState = state;
            switch (state) {
                case BalanceState.Balanced:
                    raibertFSM = new RaibertFSMBalanced();
                    break;
                case BalanceState.Unbalanced:
                    raibertFSM = new RaibertFSMUnbalanced();
                    break;
            }
            raibertFSM.OnEnter(context, raibertContext);
        }
    }

    private void OnDrawGizmos()
    {
        // Balance State
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(raibertContext.leftFootTarget.position, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(raibertContext.rightFootTarget.position, 0.1f);
    }
}
