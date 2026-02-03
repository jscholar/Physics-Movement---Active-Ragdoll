using UnityEngine;

[CreateAssetMenu(fileName = "RaibertCharacteristicSO", menuName = "Scriptable Objects/RaibertCharacteristicSO")]
public class RaibertCharacteristicSO : ScriptableObject
{
    // Foot desired lift height
    [SerializeField]
    public float stepHeight = 0.2f;

    [SerializeField] // Time between steps
    public float stepInterval = 0.5f;

    [SerializeField]
    public float unbalancedDistanceThreshold = 0.3f;

    [SerializeField]
    public float stepOvercompensation = 1.1f;

    [SerializeField]
    public float strideLength = 1.0f;
}
