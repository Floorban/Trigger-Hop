using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && SceneController.instance.inLevel)
            SceneController.instance.NextLevel(false);
    }
}
