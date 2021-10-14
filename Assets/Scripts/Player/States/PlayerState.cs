using System;
using UnityEngine;

public abstract class PlayerState : State
{   
    protected readonly PlayerController _player;

    protected bool _jumpEventReceived;
    protected bool _animFinished;

    public PlayerState(PlayerStateID id, PlayerController player, int? baseAnimationHash = null) 
    {   
        ID = id;
        _player = player;
    }

    public override void OnEnter(Enum fromState) 
    {
        base.OnEnter(fromState);

        _jumpEventReceived = false;
        _player.JumpEvent += Player_JumpEvent;            
    }
    
    public override void OnExit(Enum toState) 
    {
        _player.JumpEvent -= Player_JumpEvent;
    }

    protected virtual void Player_JumpEvent() => _jumpEventReceived = true;
}