using System;
using UnityEngine;

public class PlayerIdleState : PlayerState 
{
    public PlayerIdleState(PlayerStateID id, PlayerController player) : base(id, player){}

    public override Enum Tick()
    {
        if(_player.MovementInput != Vector2.zero)
            return PlayerStateID.MOVE;

        if (_jumpEventReceived)
            return PlayerStateID.JUMP;

        return null;
    }
}