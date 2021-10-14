using UnityEngine;
using System;

public class EndController : MonoBehaviour 
{
    public static event Action GameWonEvent;

    [SerializeField] Collider2D _p1Coll;
    [SerializeField] Collider2D _p2Coll;
    
    Collider2D _ownCollider;
    bool _gameFinished = false;

    public bool PlayersInExit {get => _p1Coll.IsTouching(_ownCollider) && _p2Coll.IsTouching(_ownCollider);}

    void Awake() 
    {
        _ownCollider = GetComponent<Collider2D>();
    }

    void Update() 
    {
        if (PlayersInExit && !_gameFinished)
        {   
            _gameFinished = true;
            GameWonEvent?.Invoke();
        }    
    }
}