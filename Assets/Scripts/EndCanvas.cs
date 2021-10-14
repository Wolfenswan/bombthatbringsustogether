using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EndController.GameWonEvent += GameWon;
        BallController.GameLostEvent += GameLost;
    }

    void OnDisable() 
    {
        EndController.GameWonEvent -= GameWon;
        BallController.GameLostEvent -= GameLost;
    }

    void GameWon() => transform.Find("FinalText").gameObject.SetActive(true);
    void GameLost() => transform.Find("FinalText2").gameObject.SetActive(true);
}
