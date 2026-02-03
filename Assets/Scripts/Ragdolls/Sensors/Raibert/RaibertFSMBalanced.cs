using UnityEngine;

public class RaibertFSMBalanced : RaibertFSMState
{

    public override BalanceState Execute(RagdollContext ragdollContext, RaibertContext raibertContext)
    {
        // Check if unbalanced
        Vector3 footCoMGround = Vector3.ProjectOnPlane((raibertContext.leftFoot.position + raibertContext.rightFoot.position) * 0.5f, Vector3.up);
        ragdollContext.readingStore.TryGet<CoMReading>(out CoMReading com);
        Vector3 comGround = Vector3.ProjectOnPlane(com.value, Vector3.up);
        _debugCoMGround = comGround;

        // We'll do a simple distance check along the XZ plane. Can potentially improve by factoring in moment of inertia in the future?
        float distanceXZ = (footCoMGround - comGround).magnitude;
        if (distanceXZ > raibertContext.characteristicSO.unbalancedDistanceThreshold) {
            return BalanceState.Unbalanced;
        }

        return BalanceState.Balanced;
    }
}
