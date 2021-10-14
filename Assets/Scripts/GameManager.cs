using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameManager : MonoBehaviour 
{
    void Start()
    {
        AudioManager.Instance.StartMusicTrack("MainMusic");

        EndController.GameWonEvent += OnGameWonEvent;
    }

    void OnDisable() 
    {
        EndController.GameWonEvent -= OnGameWonEvent;
    }

    public void OnGameWonEvent()
    {
        AudioManager.Instance.StartMusicTrack("GameWonMusic");
    }

    public void OnPlayerJoinedEvent(PlayerInput pI)
    {
        var gamepads = Gamepad.all;
        var idx = pI.playerIndex;

        if (gamepads.Count == 0)
        {
            if (idx == 0)
            {   
                //pI.SwitchCurrentActionMap("WASD");
            }
            else if (idx == 1)
            {
                //pI.SwitchCurrentActionMap("ArrowKeys");
            }
        } else if (gamepads.Count == 1)
        {
            if (idx == 0)
            {
                //pI.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current);
                //pI.SwitchCurrentActionMap("Gamepad");
            } else if (idx == 1)
            {
                pI.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
            }
        } else if (gamepads.Count > 1)
        {
            if (idx == 0)
            {
                pI.SwitchCurrentControlScheme("Gamepad", gamepads[0]);
                //pI.SwitchCurrentActionMap("Gamepad");
            } else if (idx == 1) 
            {
                pI.SwitchCurrentControlScheme("Gamepad", gamepads[1]);
                //pI.SwitchCurrentActionMap("Gamepad");
            }
        }
    }
}