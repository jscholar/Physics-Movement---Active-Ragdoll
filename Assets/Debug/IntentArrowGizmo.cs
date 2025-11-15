using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGizmo : MonoBehaviour
{
    public float arrowScale = 2f;
    PlayerInputRouter router;

    void OnEnable() { router = GetComponent<PlayerInputRouter>(); }

    void OnDrawGizmos()
    {
        if (!router) return;
        PlayerMoveIntent moveIntent = router.CurrentPlayerMoveIntent;
        var p = transform.position + Vector3.up * 0.1f;
        var d = new Vector3(moveIntent.moveDir.x, 0f, moveIntent.moveDir.y) * arrowScale;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p, p + d);
        Gizmos.DrawSphere(p + d, 0.05f);
    }
}
