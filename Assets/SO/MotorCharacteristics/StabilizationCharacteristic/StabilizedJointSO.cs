using UnityEngine;

[CreateAssetMenu(fileName = "StabilizedJoint", menuName = "ScriptableObjects/StabilizedJoint", order = 1)]
public class StabilizedJoint : ScriptableObject
{
    public float kp;
    public float kd;
    public float maxTorque;
    public string jointPath;
}