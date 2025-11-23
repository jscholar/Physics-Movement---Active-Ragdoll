using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    // Invokes a callback on all children of the given GameObject recursively.
    // Uses DFS and invokes on leaf nodes first.
    public static void InvokeOnAllChildren(GameObject gameObject, Action<GameObject> callback)

    {
        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            InvokeOnAllChildren(child, callback);

            callback(child);
        }
    }

    public static List<Rigidbody> GetAllRigidbodies(GameObject gameObject)
    {
        List<Rigidbody> rigidbodies = new List<Rigidbody>();
        InvokeOnAllChildren(gameObject, (child) =>
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidbodies.Add(rb);
            }
        });
        return rigidbodies;
    }
}
