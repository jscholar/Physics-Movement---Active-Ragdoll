using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementCharacteristicSO", menuName = "ScriptableObjects/PlayerMovementCharacteristicSO")]
public class PlayerMovementCharacteristicSO : ScriptableObject
{
    public float kpVelocity; // Proportional gain for velocity control
    public float kdVelocity; // Derivative gain for velocity control
    public float maxAcceleration; // Use acceleration for easier configuration of responsiveness to player control.
}
