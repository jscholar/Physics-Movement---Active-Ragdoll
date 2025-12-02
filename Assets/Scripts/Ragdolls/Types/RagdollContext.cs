using UnityEngine;

public struct RagdollContext
{
    public Rigidbody rootRigidBody;
    public Rigidbody[] rigidbodies;
    public PlayerInputRouter inputRouter;
    public RagdollCharacteristicSO characteristic;
    public float totalMass;

    public ReadingStore readingStore;
}