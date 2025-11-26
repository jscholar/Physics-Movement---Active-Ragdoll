using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassUtils : MonoBehaviour
{
    public static float GetTotalMass(GameObject gameObject)
    {
        float totalMass = 0f;

        Rigidbody rootRb = gameObject.GetComponent<Rigidbody>();
        if (rootRb != null) {
            totalMass += rootRb.mass;
        }

        void AccumulateMass(GameObject obj)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                totalMass += rb.mass;
            }
        }

        Utils.InvokeOnAllChildren(gameObject, AccumulateMass);
        return totalMass;
    }

    /**
     * Get Center of Mass of all RigidBodies in the tree
     */
    public static Vector3 GetWorldCenterOfMass(Rigidbody[] rbs)
    {
        float totalMass = 0f;
        Vector3 center = Vector3.zero;
        foreach (Rigidbody rb in rbs) {
            totalMass += rb.mass;
            center += rb.position * rb.mass;
        }
        Debug.Log(totalMass);


        return center / totalMass;
    }
    public static Vector3 GetWorldCenterOfMass(Rigidbody[] rbs, float totalMass)
    {
        Vector3 center = Vector3.zero;
        foreach (Rigidbody rb in rbs) {
            center += rb.position * rb.mass;
        }

        return center / totalMass;
    }
}
