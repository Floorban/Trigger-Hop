using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public Transform lookAt;
    public float coinRequirement, timeRequirement;
    public static UnityAction<bool> OnLevelFinished;
    private OffScreenIndicator indicator;
    private void Start()
    {
        indicator = GetComponent<OffScreenIndicator>();
        indicator.InitIndicator(gameObject);
        SceneController.instance.lvl = this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();
            OnLevelFinished?.Invoke(true);
            //SceneController.instance.NextLevel();
        }
    }
    private void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedLevels"))
        {
            PlayerPrefs.SetInt("ReachedLevels", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
