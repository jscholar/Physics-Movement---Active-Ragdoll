using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GhostRigBuilder
{
    [MenuItem("Tools/Active Ragdoll/Build Ghost Rig (Selected)")]
    public static void BuildGhostRig()
    {
        var go = Selection.activeGameObject;
        if (!go) {
            Debug.Log("No");
            return;
        }

        var ragdollGhost = go.GetComponent<RagdollGhost>();
        if (!ragdollGhost) {
            Debug.Log("No");
            return;
        }

        Build(ragdollGhost);
    }

    private static HashSet<Type> allowList = new() {
        typeof(Transform),
        typeof(Animator),
    };

    public static void Build(RagdollGhost ragdollGhost)
    {
        string ghostName = ragdollGhost.RagdollModelRoot.name + "_Ghost";
        var existingGhost = ragdollGhost.transform.parent.Find(ghostName);

        if (existingGhost) {
            Debug.Log("Ghost rig already exists. I'm too lazy to make an update script?");
        }

        Transform ghost = UnityEngine.Object.Instantiate(ragdollGhost.RagdollModelRoot, ragdollGhost.transform.parent);
        ghost.name = ghostName;

        StripComponents(ghost.gameObject);
        AddAnimator(ghost.gameObject);

        ragdollGhost.GhostRigRoot = ghost;

        Debug.Log("Ghost rig built");
    }

    private static void StripComponents(GameObject ghostGO)
    {
        Undo.RegisterCreatedObjectUndo(ghostGO.gameObject, "Create Ghost Rig");

        List<Component> stripList = new();

        foreach (Component c in ghostGO.GetComponentsInChildren<Component>(true)) {
            if (c == null) continue;

            if (!GhostRigBuilder.allowList.Contains(c.GetType())) {
                stripList.Add(c);
            }
        }

        stripList.Sort((a, b) => DeletePriority(a).CompareTo(DeletePriority(b)));

        foreach (Component c in stripList) {
            Undo.DestroyObjectImmediate(c);
        }
    }

    private static void AddAnimator(GameObject ghostGO)
    {
        Animator animator = Undo.AddComponent<Animator>(ghostGO);
        animator.applyRootMotion = false;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    static int DeletePriority(Component c) => c switch
    {
        // Scripts first so RequireComponent stops fighting you
        MonoBehaviour => 0,

        // Physics chain next
        Joint => 10,
        Collider => 20,
        Rigidbody => 30,

        // Cosmetics/other
        Renderer => 40,

        // Transform last (but you’ll never delete it anyway)
        Transform => int.MaxValue,

        _ => 50,
    };
}

