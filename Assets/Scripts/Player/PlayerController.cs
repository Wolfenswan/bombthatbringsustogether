using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour 
{   
    public event Action JumpEvent;
    public event Action ThrowEvent;

    [SerializeField] BallController _chainBall;
    [SerializeField] GameObject _otherPlayer;
    [SerializeField] GameData _data;
    [SerializeField] PlayerData _playerData;

    PlayerInput _playerInput;
    GFXController _gfxController;
    StateMachine _stateMachine;
    Rigidbody2D _rb2d;
    Transform _ballAttach;

    bool _canCatch = true;

    public bool CarryingBall{get; private set;} = false;
    public Vector2 MovementInput{get; private set;} = Vector2.zero;
    public Rigidbody2D RB2D{get=>_rb2d;}
    public GameData GameData{get=>_data;}
    public PlayerData PlayerData{get=>_playerData;}

    public EntityFacing Facing{get; private set;} = EntityFacing.RIGHT;    
    public bool IsGrounded{
        get => 
            Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f,0), Vector3.down, _data.DistanceToGround, _data.GroundLayer) 
            || Physics2D.Raycast(new Vector3(transform.position.x - _playerData.GroundCheckOffset.x, transform.position.y + _playerData.GroundCheckOffset.y,0), Vector3.down, _data.DistanceToGround, _data.GroundLayer)
            || Physics2D.Raycast(new Vector3(transform.position.x + _playerData.GroundCheckOffset.x, transform.position.y + _playerData.GroundCheckOffset.y,0), Vector3.down, _data.DistanceToGround, _data.GroundLayer)
        ;}

    //public bool IsGrounded{get => _collisionController.Initialized && _collisionController.CollisionStates[CollisionID.GROUND];}

    public bool CanPickBall{get => Vector2.Distance(transform.position, _chainBall.transform.position) < _data.PickupDistance;}
    public bool IsAiming{get => (PlayerStateID) _stateMachine.CurrentState.ID == PlayerStateID.AIM;}
    public bool FacingOtherPlayer{get => (Facing == EntityFacing.RIGHT && transform.position.x < _otherPlayer.transform.position.x) ||  (Facing == EntityFacing.LEFT && transform.position.x > _otherPlayer.transform.position.x);}
    public bool IsTooFarAwayFromOtherPlayer{get => 
        (Vector2.Distance(transform.position, _otherPlayer.transform.position) > _data.MaxDistanceBetweenPlayers) && 
        ((MovementInput.normalized.x == 1 && transform.position.x > _otherPlayer.transform.position.x) || (MovementInput.normalized.x == -1 && transform.position.x < _otherPlayer.transform.position.x));
    }

    public Vector2 BallConnectPoint{get => new Vector2(transform.position.x+_playerData.BallAttachOffsets.x * (int) Facing, transform.position.y + _playerData.BallAttachOffsets.y);}

    void Awake()
    {   
        _playerInput = GetComponent<PlayerInput>();
        _rb2d = GetComponent<Rigidbody2D>();
        _gfxController = transform.Find("GFX").GetComponent<GFXController>();
        //_collisionController = transform.Find("CollisionController").GetComponent<CollisionController>();
        
        InitializeStateMachine();
    }

    void Start()
    {
        _playerInput.onActionTriggered += ParseInputAction;
        BallController.GameLostEvent += OnGameLostEvent;
    }

    void OnDisable() 
    {
        _playerInput.onActionTriggered -= ParseInputAction;
        BallController.GameLostEvent -= OnGameLostEvent;
    }
    
    void InitializeStateMachine()
    {
        var states = new List<State>
        {
            new PlayerIdleState(PlayerStateID.IDLE, this),
            new PlayerMoveState(PlayerStateID.MOVE, this),
            new PlayerJumpState(PlayerStateID.JUMP, this),
            new PlayerLandState(PlayerStateID.LAND, this),
            new PlayerAimState(PlayerStateID.AIM, this, _chainBall),
        };
        _stateMachine = new StateMachine(states, PlayerStateID.IDLE, true);
    }
    
    void Update() 
    {   
        //_collisionController.UpdateCollisions();

        _gfxController.SetAnimatorVariable("MoveX", (int) MovementInput.x);
        _gfxController.SetAnimatorVariable("YVelocity", _rb2d.velocity.y);
        _gfxController.SetAnimatorVariable("IsGrounded", IsGrounded);
        _gfxController.SetAnimatorVariable("CarryingBomb", CarryingBall);

        if(IsTooFarAwayFromOtherPlayer)
            MovementInput = Vector2.zero;

        _stateMachine.Tick(tickMode:TickMode.UPDATE);
    }

    void FixedUpdate() 
    {
        _stateMachine.Tick(tickMode:TickMode.FIXED_UPDATE);
    }

    void OnCollisionEnter2D(Collision2D other) 
    {   
        if (other.gameObject.GetComponent<BallController>() && _canCatch && _chainBall.CanBeCaught)
            Catch();
    }

    public void MoveStep(int dir, float speed)
    {   
        var moveVector = new Vector3(speed * dir * Time.deltaTime, 0f);
        transform.position += moveVector;
        UpdateFacing((EntityFacing) dir);
    }

    public void ExecuteJump()
    {   
        if (!IsGrounded)
            return;
        var jumpVector = new Vector2(0f,_playerData.JumpPower);
        if (CarryingBall)
            jumpVector.y *= _playerData.JumpWithBallMultipl;
        _rb2d.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    void UpdateFacing(EntityFacing newFacing)
    {
        if (Facing != newFacing)
        {   
            Facing = newFacing;
            _gfxController.transform.localScale = new Vector3((float) Facing, 1,1);

            if (CarryingBall)
            {
                _chainBall.transform.position = new Vector3(transform.position.x + _playerData.BallAttachOffsets.x * (float) Facing, transform.position.y + _playerData.BallAttachOffsets.y,0);
            }
        }     
    }

    void SetFacingTowards<T> (T otherEntity) where T:Transform
    {
        var newFacing = otherEntity.position.x > transform.position.x?EntityFacing.RIGHT:EntityFacing.LEFT;
        UpdateFacing(newFacing);
    }

    void ParseInputAction(InputAction.CallbackContext ctx)
    {
        switch(ctx.action.name)
        {
            case "Move":
                MovementInput = ctx.action.ReadValue<Vector2>().normalized;
                break;
            case "Jump":
                if (ctx.performed && ctx.action.ReadValue<float>() == 1)
                    JumpEvent?.Invoke(); // Listened to by the Player State
                break;
            case "Fire":
                if (ctx.performed && ctx.action.ReadValue<float>() == 1)
                {
                    if (CarryingBall)
                        _stateMachine.ForceState(PlayerStateID.AIM);
                    else if (!CarryingBall && CanPickBall & _chainBall.CanBePickedUp)
                        Pickup();
                    else if (!CarryingBall && !CanPickBall && _chainBall.CarriedBy == null)
                        Yank();
                }

                if (ctx.canceled && ctx.action.ReadValue<float>() == 0 && IsAiming) // Throw ended
                {
                    ThrowEvent?.Invoke();
                }
            break;
            default:
                Debug.LogError("Unconfigured Input Event!");
                break;
        }
    }

    void Pickup()
    {   
        CarryingBall = true;
        _chainBall.SetCarried(transform);
        AudioManager.Instance.PlaySoundEffect($"BombPickedUp");
        //_chainBall.transform.position = new Vector3(transform.position.x + _playerData.BallAttachOffsets.x * (float) Facing, transform.position.y + _playerData.BallAttachOffsets.y,0);
        
    }

    void Catch()
    {
        SetFacingTowards(_otherPlayer.transform);
        // Maybe lower _chainball Velocity?
        Pickup();
    }

    public void Throw(float angle)
    {
        CarryingBall = false;
        _chainBall.SetCarried(null);
        AudioManager.Instance.PlaySoundEffect($"Throw");
        StartCoroutine(CatchCooldown()); // Prevent player from catching the ball they just threw
        _chainBall.Throw((int) Facing, _playerData.ThrowForce, angle);
    }

    void Yank()
    {
        AudioManager.Instance.PlaySoundEffect($"Yank");
        if (_playerData.CanYank)
            _chainBall.Yank(transform, _playerData.YankForce);
    }

    IEnumerator CatchCooldown()
    {
        _canCatch = false;
        yield return new WaitForSeconds(0.05f);
        _canCatch = true;
    }

    void OnGameLostEvent() => gameObject.SetActive(false);

    void OnDrawGizmos() 
    {   
        Gizmos.color = IsGrounded?Color.green:Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + _playerData.GroundCheckOffset.y - _data.DistanceToGround,0f),_data.DebugSphereSize);
        Gizmos.DrawSphere(new Vector3(transform.position.x - _playerData.GroundCheckOffset.x, transform.position.y + _playerData.GroundCheckOffset.y - _data.DistanceToGround,0f),_data.DebugSphereSize);
        Gizmos.DrawSphere(new Vector3(transform.position.x + _playerData.GroundCheckOffset.x, transform.position.y + _playerData.GroundCheckOffset.y - _data.DistanceToGround,0f),_data.DebugSphereSize);
    
        Gizmos.color = CanPickBall?Color.green:Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y,0f),_data.DebugSphereSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(transform.position.x + _playerData.BallAttachOffsets.x, transform.position.y + _playerData.BallAttachOffsets.y,0f),_data.DebugSphereSize);
    }
}