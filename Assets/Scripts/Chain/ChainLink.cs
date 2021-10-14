using UnityEngine;

public class ChainLink : MonoBehaviour
{

    public GameObject connectedAbove, connectedBelow;

    // Start is called before the first frame update
    void Start()
    {
        // holt sich den link darüber
        connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        ChainLink aboveSegment = connectedAbove.GetComponent<ChainLink>();
        if (aboveSegment != null)
        {
            // macht eine neue verknüpfung
            aboveSegment.connectedBelow = gameObject;
            // da wir center mid als angelpunkt haben, nimmt man die -gesamtlänge als neuen Angelpunkt
            float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y * 0.7f;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -1);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
        }
    }

}
