using Unity.VisualScripting;
using UnityEngine;

public abstract class RaibertFSMState : MonoBehaviour
{
    protected Vector3 _debugCoMGround;

    public virtual void OnEnter(RagdollContext ragdollContext, RaibertContext raibertContext)
    {
        // Optional override for entering the state
    }

    public abstract BalanceState Execute(RagdollContext ragdollContext, RaibertContext raibertContext);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_debugCoMGround, 0.1f);
    }
}
