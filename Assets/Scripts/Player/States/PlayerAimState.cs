using System;
using UnityEngine;

public class PlayerAimState : PlayerState 
{
    public PlayerAimState(PlayerStateID id, PlayerController player, BallController chainBall) : base(id, player)
    {
        _chainBall = chainBall;
    }
    
    BallController _chainBall;
    float _angle;
    float _speed = 70;
    bool _executeThrow;
    int _shiftDir;

    public override void OnEnter(Enum fromState)
    {
        base.OnEnter(fromState);

        AudioManager.Instance.PlaySoundEffect($"Aim");

        _player.ThrowEvent += OnThrow;

        _angle = 0;
        _shiftDir = (int) _player.Facing;
        _executeThrow = false;
        _chainBall.UpdateTargetingAngle(_angle, (int) _player.Facing);
    }

    public override void OnExit(Enum toState)
    {
        base.OnExit(toState);

        _player.ThrowEvent -= OnThrow;

        AudioManager.Instance.StopSoundEffect($"Aim");

        _chainBall.UpdateTargetingAngle(null, (int) _player.Facing);
    }

    public override Enum Tick()
    {   
        if (_executeThrow)
        {
            _player.Throw(_angle);
            return PlayerStateID.IDLE;
        }

        if (_jumpEventReceived)
            return PlayerStateID.IDLE;
        
        if (_shiftDir == 1 && _angle >= _player.GameData.ThrowingAngle.Max)
            _shiftDir = -1;
        else if (_shiftDir == -1 && _angle <= _player.GameData.ThrowingAngle.Min)
            _shiftDir = 1;

        var tick = (_speed * Time.deltaTime) * _shiftDir;

        _angle += tick;
        _chainBall.UpdateTargetingAngle(_angle, (int) _player.Facing);

        return null;
    }

    void OnThrow() => _executeThrow = true;
}