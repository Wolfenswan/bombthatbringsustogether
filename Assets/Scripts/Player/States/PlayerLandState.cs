using System;
using UnityEngine;

public class PlayerLandState : PlayerState 
{
    public PlayerLandState(PlayerStateID id, PlayerController player) : base(id, player){}

    public override Enum Tick()
    {
        if (_animFinished || _runTime > 0.05f)
        {
            return PlayerStateID.IDLE;
        }

        return null;
    }

    public override Enum FixedTick()
    {
        if(_jumpEventReceived)
            return PlayerStateID.JUMP;

        return null;
    }
}