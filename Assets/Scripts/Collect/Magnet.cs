using UnityEngine;

public class Magnet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Pickup pickup = collision.GetComponent<Pickup>();
        if (pickup)
        {
            pickup.target = transform;
        }
    }
}
