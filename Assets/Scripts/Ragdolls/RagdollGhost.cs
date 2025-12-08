using UnityEditor;
using UnityEngine;

/**
 * Attach to Ragdoll to create a ghost rig
 */
public class RagdollGhost : MonoBehaviour
{
    [SerializeField]
    private Transform ragdollModelRoot;
    public Transform RagdollModelRoot { get => ragdollModelRoot; private set => ragdollModelRoot = value; }

    [SerializeField]
    private Transform ghostRigRoot;
    public Transform GhostRigRoot { get => ghostRigRoot; set => ghostRigRoot = value; }
}
