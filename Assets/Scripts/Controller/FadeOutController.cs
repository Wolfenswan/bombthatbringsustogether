using UnityEngine;
using System.Collections;

public class FadeOutController : MonoBehaviour 
{
    [SerializeField] float _fadeInStep = 0.05f;
    SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0f);
    }

    void Start() 
    {
        EndController.GameWonEvent += OnGameOverEvent;
        BallController.GameLostEvent += OnGameOverEvent;
    }

    void OnDisable() 
    {
        EndController.GameWonEvent -= OnGameOverEvent;
        BallController.GameLostEvent -= OnGameOverEvent;
    }

    void OnGameOverEvent() => StartCoroutine(GameFinished());

    IEnumerator GameFinished()
    {
        while (_sr.color.a != 1)
        {
            yield return new WaitForEndOfFrame();
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, _sr.color.a + _fadeInStep);
        }
    }
}