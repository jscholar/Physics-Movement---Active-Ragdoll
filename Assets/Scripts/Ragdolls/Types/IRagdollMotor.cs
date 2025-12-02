interface IRagdollMotor { }

interface IFixedUpdateRagdollMotor : IRagdollMotor
{
    void FixedUpdateMotor(RagdollContext ragdollContext, MotorOutput output);
}

interface IUpdateRagdollMotor : IRagdollMotor
{
    void UpdateMotor(RagdollContext ragdollContext, MotorOutput output);
}
