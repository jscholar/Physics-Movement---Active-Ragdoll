using UnityEngine;

[CreateAssetMenu(fileName = "UprightJointMotorCharacteristicSO", menuName = "Scriptable Objects/UprightJointMotorCharacteristicSO")]
public class UprightJointMotorCharacteristicSO : ScriptableObject
{
    public float kp;
    public float kd;
    public float maxTorque;
}
