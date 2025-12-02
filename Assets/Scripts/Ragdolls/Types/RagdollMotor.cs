using UnityEngine;

public abstract class RagdollMotor : MonoBehaviour {
    public virtual void FixedUpdateMotor(RagdollContext ragdollContext, MotorOutput output) { }
    public virtual void UpdateMotor(RagdollContext ragdollContext, MotorOutput output) { }
}
