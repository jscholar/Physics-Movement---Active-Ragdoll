using UnityEngine;

// Draws gizmos for a rig hierarchy, useful for visualizing skeletons in the editor.
public class RigGizmos : MonoBehaviour
{
    // assign GhostRig skeleton root
    public Transform root;
    public Color boneColor = Color.cyan;
    public float radius = 0.02f;

    private void OnDrawGizmos()
    {
        if (!root) return;
        DrawRecursive(root);
    }

    private void DrawRecursive(Transform t)
    {
        Gizmos.color = boneColor;
        Gizmos.DrawSphere(t.position, radius);

        foreach (Transform child in t) {
            Gizmos.DrawLine(t.position, child.position);
            DrawRecursive(child);
        }
    }
}