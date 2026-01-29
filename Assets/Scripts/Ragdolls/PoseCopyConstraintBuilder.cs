using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PoseCopyConstraintBuilder : MonoBehaviour
{
    [SerializeField]
    private Transform sourceRoot;

    [SerializeField]
    private Transform targetRoot;


    private Dictionary<string, Transform> sourceTransformMap = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> targetTransformMap = new Dictionary<string, Transform>();

    [ContextMenu("Build Pose Copy Constraints")]
    public void Build()
    {
        GetSynchronizedTransforms();
        AddConstraints();
    }

    private void AddConstraints()
    {
        foreach (var (name, transform) in sourceTransformMap) {
            var go = new GameObject(name + "_PoseCopyConstraint");
            go.transform.SetParent(this.transform);

            MultiParentConstraint component = go.AddComponent<MultiParentConstraint>();
            component.data.constrainedObject = targetTransformMap[name];
            component.data.sourceObjects = new WeightedTransformArray
            {
                new WeightedTransform
                {
                    transform = sourceTransformMap[name],
                    weight = 1.0f
                }
            };
        }
    }

    private void GetSynchronizedTransforms()
    {
        Debug.Log("RigSynchronizer started");
        // The root has a different name, so we do it manually
        sourceTransformMap.Add("root", sourceRoot);
        targetTransformMap.Add("root", targetRoot);

        DepthFirstRecurse("", sourceRoot);

        Debug.Log("Sanity Check, Number of Transforms Found: " + sourceTransformMap.Count.ToString());
    }

    private void DepthFirstRecurse(string path, Transform sourceTransform)
    {
        for (int i = 0; i < sourceTransform.childCount; i++) {
            var child = sourceTransform.GetChild(i);

            Transform targetTransform = targetRoot.Find(path + child.name);
            if (targetTransform != null) {
                if (!sourceTransformMap.ContainsKey(child.name)) {
                    sourceTransformMap.Add(child.name, child);
                }

                if (!targetTransformMap.ContainsKey(child.name)) {
                    targetTransformMap.Add(child.name, targetTransform);
                }
            }
            DepthFirstRecurse(path + child.name + "/", child);
        }
    }
}
