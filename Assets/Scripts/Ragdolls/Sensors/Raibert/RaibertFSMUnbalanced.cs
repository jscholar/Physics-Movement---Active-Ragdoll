using UnityEngine;

public class RaibertFSMUnbalanced : RaibertFSMState
{
    private bool _isStepping = false;
    private float t = 0;

    // How far ahead to place the foot to achieve balance

    // Local variables for bezier curve
    private Vector3 p0, p1, p2;

    private Transform targetTransform;

    public override void OnEnter(RagdollContext ragdollContext, RaibertContext raibertContext)
    {
        ragdollContext.readingStore.TryGet<CoMReading>(out CoMReading com);
        Vector3 comGround = Vector3.ProjectOnPlane(com.value, Vector3.up);

        Transform closerFoot;

        if ((raibertContext.leftFoot.transform.position - comGround).magnitude > (raibertContext.rightFoot.transform.position - comGround).magnitude) {
            p0 = raibertContext.leftFoot.position;
            closerFoot = raibertContext.rightFoot;
            targetTransform = raibertContext.leftFootTarget;
        }
        else {
            p0 = raibertContext.rightFoot.position;
            closerFoot = raibertContext.leftFoot;
            targetTransform = raibertContext.rightFootTarget;
        }

        p2 = (comGround - (closerFoot.position - comGround)) * raibertContext.characteristicSO.stepOvercompensation;
        p2.y = closerFoot.position.y;

        p1 = (p0 + p2) / 2 + Vector3.up * raibertContext.characteristicSO.stepHeight;
        // Adjust curve to pass through p1
        // src: https://www.reddit.com/r/Unity3D/comments/1gqgomu/quick_tip_quadratic_bezier_curve_through_all_3/
        p1 = 2 * p1 - 0.5f * (p0 + p2);

        p0 = p0 - ragdollContext.rootRigidBody.position;
        p1 = p1 - ragdollContext.rootRigidBody.position;
        p2 = p2 - ragdollContext.rootRigidBody.position;

        p2 *= raibertContext.characteristicSO.stepOvercompensation;

        // Clamp p2 to be within reachable distance from other foot
        Vector3 stride = p2 - (closerFoot.position - ragdollContext.rootRigidBody.position);
        Vector3.ClampMagnitude(stride, raibertContext.characteristicSO.strideLength);
        p2 = (closerFoot.position - ragdollContext.rootRigidBody.position) + stride;

        _isStepping = true;

        base.OnEnter(ragdollContext, raibertContext);
    }

    public override BalanceState Execute(RagdollContext ragdollContext, RaibertContext raibertContext)
    {
        StepRoutine_TimeBased(ragdollContext, raibertContext);

        if (_isStepping == false) {
            return BalanceState.Balanced;
        }

        return BalanceState.Unbalanced;
    }

    // Approach 1.
    // Intepolate and LERP foot target over time
    // I don't like that this is time-based rather than physics based. It doesn't make sense that steps should have a fixed duration.
    private void StepRoutine_TimeBased(RagdollContext ragdollContext, RaibertContext raibertContext)
    {
        t += Time.deltaTime / raibertContext.characteristicSO.stepInterval;
        if (t >= 1f) {
            t = 1f;
            _isStepping = false;
        }

        Vector3 bezierT = QuadraticBezier(p0, p1, p2, t);
        Vector3 newPosition = ragdollContext.rootRigidBody.position + bezierT;
        targetTransform.position = newPosition;
    }

    public static Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 m1 = Vector3.Lerp(p0, p1, t);
        Vector3 m2 = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(m1, m2, t);
    }
}
