using UnityEngine;

public class BreakThings : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IBreakable>(out IBreakable breakAble))
        {
            breakAble.Break();
        }
        
    }

}
