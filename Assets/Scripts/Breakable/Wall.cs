using UnityEngine;

public class Wall : MonoBehaviour, IBreakable
{
    public void Break()
    {
        // trigger anim, do stuff, have fun and then...
        gameObject.SetActive(false);
        // or set collider to inactive and then disable object.. or stuff
    }

}
