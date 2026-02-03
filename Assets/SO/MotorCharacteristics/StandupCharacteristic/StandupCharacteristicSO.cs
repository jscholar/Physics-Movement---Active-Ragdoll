using UnityEngine;

[CreateAssetMenu(fileName = "StandupCharacteristicSO", menuName = "Scriptable Objects/StandupCharacteristicSO")]
public class StandupCharacteristicSO : ScriptableObject
{
    public float desiredDistance = 1.8f;
    public float springKp = 1.0f;
}
