using UnityEngine;
using System;
using System.Collections;
using NEINGames.Utilities;

public class BallController : MonoBehaviour 
{
    public static event Action GameLostEvent;

    [SerializeField] GameData _data;
    [SerializeField] GameObject _reticule;

    public GameObject CarriedBy{get;private set;}

    public bool CanDestroy{get => _rb2d.velocity.x > 0 && !IsGrounded;}
    public bool IsGrounded{get => Physics2D.Raycast(transform.position, Vector3.down, 3f, _data.GroundLayer);}
    public bool CanBePickedUp{get => CarriedBy == null && IsGrounded;}
    public bool CanBeCaught{get => CarriedBy == null && !IsGrounded;}

    AudioSource _audio;
    Rigidbody2D _rb2d;
    Collider2D _collider;

    bool _fused;
    float _fuseTimerStart = 0f;
    bool _exploded = false;

    void Awake() 
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _audio = GetComponent<AudioSource>();
        _reticule.SetActive(false);
    }

    void Start()
    {
        SetFuseMode(true);
    }

    void Update() 
    {
        if (!_exploded && _fused && Time.time - _fuseTimerStart >= _data.BombTimer)
        {
            AudioManager.Instance.StopSoundEffect($"FuseLit");
            AudioManager.Instance.PlaySoundEffect($"ExplosionNeu");
            GetComponent<Animator>().Play("Boom");
            Camera.main.transform.parent = null;
            GameLostEvent?.Invoke();
            transform.rotation = new Quaternion(0f, 0f,0f,0f);
            _exploded = true;
        }

    }

    void FixedUpdate() 
    {
        if (CarriedBy!=null)
        {
            transform.position = CarriedBy.GetComponent<PlayerController>().BallConnectPoint;
        }
    }

    public void SetCarried(Transform carriedBy)
    {   
        CarriedBy = carriedBy != null?carriedBy.gameObject:null;
        //transform.parent = carriedBy;
        _rb2d.velocity = Vector2.zero;
        //_rb2d.simulated = carriedBy == null;
        //_rb2d.mass = CarriedBy != null?0:4;
        _rb2d.bodyType = CarriedBy != null?RigidbodyType2D.Kinematic:RigidbodyType2D.Dynamic;
        _collider.enabled = CarriedBy == null;
        if (CarriedBy != null)
        {   
            transform.position = (Vector3) CarriedBy?.GetComponent<PlayerController>().BallConnectPoint;
            _rb2d.velocity = Vector2.zero;
            _rb2d.angularVelocity = 0;
            SetFuseMode(false);
        }
        // _rb2d.angularVelocity = 0;
    }

    public void Throw(int facing, float force, float angle)
    {   
        //StopCoroutine(BombCountdown());
        var faceVector = facing==1?Vector3.right:Vector3.left;
        var dir = Quaternion.AngleAxis(angle, Vector3.forward) * faceVector;
        var forceVector = dir * force;
        Debug.Log($"Ball being thrown with force {forceVector} at angle {angle}");
        _rb2d.AddForce(forceVector, ForceMode2D.Impulse);
        StartCoroutine(WaitTillOnGround());
        SetFuseMode(true);
    }

    // public void Throw(Transform towards, float throwForce, int facing)
    // {   
    //     StopCoroutine(BombCountdown());
    //     var forceVector = new Vector2(throwForce * facing, 50f);
    //     if (towards != null)
    //     {
    //         forceVector = (towards.position - transform.position).normalized * throwForce;
    //         forceVector.y *= 2;
    //     }
            
    //     Debug.Log($"Ball being thrown with force {forceVector}");
    //     _rb2d.AddForce(forceVector, ForceMode2D.Impulse);
    // }

    public void Yank(Transform towards, float yankForce)
    {   
        var forceVector = (towards.position - transform.position).normalized * yankForce;
        Debug.Log($"Ball being yanked with force {forceVector}");
        _rb2d.AddForce(forceVector, ForceMode2D.Impulse);
    }

    public void UpdateTargetingAngle(float? angle, int dir)
    {   
        if (angle == null)
            _reticule.SetActive(false);
        else 
        {
            _reticule.SetActive(true);
            var target = Vector2Utilities.GetOffsetPointFromAngle(transform.position, 6 * dir, (float) angle);
            _reticule.transform.position = target;
            //Debug.DrawLine(transform.position, target, Color.red, 0.001f);
        }
        
    }

    void SetFuseMode(bool fused)
    {
        if (fused)
        {
            // Start hissing sound
            GetComponent<Animator>().Play("Burning");
            AudioManager.Instance.PlaySoundEffect($"FuseLit");
            _fused = true;
            _fuseTimerStart = Time.time;
            //StopCoroutine(BombCountdown());
            //StartCoroutine(BombCountdown());
        } else 
        {
            // Stop sound
            AudioManager.Instance.StopSoundEffect($"FuseLit");
            // Use default sprite
            GetComponent<Animator>().Play("Idle");
            _fused = false;
            _fuseTimerStart = 0;
            //StopCoroutine(BombCountdown());
        }
    }

    IEnumerator WaitTillOnGround()
    {
        yield return new WaitUntil(() => CarriedBy != null || IsGrounded);
        if (IsGrounded)
        {
            AudioManager.Instance.PlaySoundEffect($"BombDropped");
        }
            
    }

    IEnumerator BombCountdown()
    {
        AudioManager.Instance.PlaySoundEffect($"FuseLit");
        yield return new WaitForSeconds(_data.BombTimer);
        if (CarriedBy == null) // The timer should have been stopped when picked up, but to be sure...
        {
            GetComponent<Animator>().Play("Boom");
            AudioManager.Instance.PlaySoundEffect($"ExplosionNeu");
            Camera.main.transform.parent = null;
            GameLostEvent?.Invoke();
        }
    }

    void OnDrawGizmos() 
    {   
        Gizmos.color = IsGrounded?Color.green:Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 3f));
    }
}