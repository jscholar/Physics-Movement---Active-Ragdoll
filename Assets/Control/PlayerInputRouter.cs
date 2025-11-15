using UnityEngine;

public class PlayerInputRouter : MonoBehaviour
{
    private InputActions playerInputActions;

    public PlayerMoveIntent CurrentPlayerMoveIntent { get; private set; }
    public DebugIntent CurrentDebugIntent { get; private set; }

    private void OnEnable()
    {
        playerInputActions = new InputActions();
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    private void Update()
    {
        Vector3 moveIntent = this.getMoveIntent();
        bool jumpPressed = this.getJumpIntent();
        CurrentPlayerMoveIntent = new PlayerMoveIntent { jumpPressed = jumpPressed, moveDir = moveIntent };
    
        CurrentDebugIntent = new DebugIntent
        {
            balloonPressed = this.getBalloonIntent()
        };
    }

    private Vector3 getMoveIntent()
    {
        return playerInputActions.PlayerCharacter.Move.ReadValue<Vector2>();
    }
    private bool getJumpIntent()
    {
        return playerInputActions.PlayerCharacter.Jump.triggered;
    }

    private bool getBalloonIntent()
    {
        bool isBalloonIntent = playerInputActions.Debug.Balloon.IsPressed();
        return isBalloonIntent;
    }
}
