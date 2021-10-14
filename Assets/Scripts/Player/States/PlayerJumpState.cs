using System;
using UnityEngine;

public class PlayerJumpState : PlayerState 
{
    public PlayerJumpState(PlayerStateID id, PlayerController player) : base(id, player){}

    public override void OnEnter(Enum fromState)
    {
        base.OnEnter(fromState);

        _player.ExecuteJump();

        AudioManager.Instance.PlaySoundEffect($"Jump{_player.PlayerData.Name}");
    }

    public override Enum FixedTick()
    {   
        if (_player.MovementInput != Vector2.zero && !_player.CarryingBall)
        {
            var dir = (int) _player.MovementInput.x;
            _player.MoveStep(dir, _player.PlayerData.AirSteeringSpeed);
        }

        if (_player.RB2D.velocity.y <= 0)
        {
            _player.RB2D.velocity = new Vector2(_player.RB2D.velocity.x, _player.RB2D.velocity.y * 1.05f);
        }

        if (_runTime > 0.05f && _player.IsGrounded)
            return PlayerStateID.LAND;

        return null;
    }
}