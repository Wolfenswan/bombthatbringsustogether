using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _chainStartRB;
    [SerializeField] private GameObject[] _prefabChainLinks;
    [SerializeField] private Rigidbody2D _chainEndRB;
    [SerializeField] private int numlinks = 5;

    // Start is called before the first frame update
    void Start()
    {
        GenerateChain();
        
    }

    void GenerateChain()
    {
        Rigidbody2D prevBody = _chainStartRB;
        for (int i = 0; i < numlinks; i++)
        {
            // falls wir verschiedene links benutzen wollen
            int index = Random.Range(0, _prefabChainLinks.Length);
                GameObject newLink = Instantiate(_prefabChainLinks[index]);
                newLink.transform.parent = transform;
                newLink.transform.position = transform.position;
                HingeJoint2D hj = newLink.GetComponent<HingeJoint2D>();
                hj.connectedBody = prevBody;

                prevBody = newLink.GetComponent<Rigidbody2D>();

            if (i == numlinks - 1)
            {
                _chainEndRB.transform.parent = transform;
                _chainEndRB.transform.position = transform.position;
                HingeJoint2D playerHJ = _chainEndRB.GetComponent<HingeJoint2D>();
                playerHJ.connectedBody = newLink.GetComponent<Rigidbody2D>();
                newLink.GetComponent<SpriteRenderer>().enabled = false;                
                break;
            }

        }

    }
}
