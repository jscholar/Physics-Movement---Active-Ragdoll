using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassUtils : MonoBehaviour
{
    public static float GetTotalMass(GameObject gameObject)
    {
        float totalMass = 0f;
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
}
