using System;
using UnityEngine;

public class PlayerMoveState : PlayerState 
{
    public PlayerMoveState(PlayerStateID id, PlayerController player) : base(id, player){}

    public override void OnEnter(Enum fromState)
    {
        base.OnEnter(fromState);

        AudioManager.Instance.PlaySoundEffect($"Move{_player.PlayerData.Name}");
    }

    public override void OnExit(Enum toState)
    {
        base.OnExit(toState);

        AudioManager.Instance.StopSoundEffect($"Move{_player.PlayerData.Name}");
    }

    public override Enum FixedTick()
    {
        if (_jumpEventReceived)
            return PlayerStateID.JUMP;

        if (_player.IsTooFarAwayFromOtherPlayer)
            return PlayerStateID.IDLE;

        if (_player.MovementInput!= Vector2.zero)
        {
            var dir = (int) _player.MovementInput.normalized.x;
            var speed = _player.CarryingBall?_player.PlayerData.MoveSpeedWithBall:_player.PlayerData.MoveSpeed;
            _player.MoveStep(dir,speed);
        } else if (_player.MovementInput == Vector2.zero)
        {
            return PlayerStateID.IDLE;
        }

        return null;
    }
}