using UnityEngine;

public class CenterOfMassSensor : RagdollSensor
{
    public override void GetReading(RagdollContext context)
    {
        Vector3 com = MassUtils.GetWorldCenterOfMass(context.rigidbodies);
        context.readingStore.GetOrCreate<CoMReading>().value = com;

        Debug.Log("Center of Mass: " + com);
    }
}
