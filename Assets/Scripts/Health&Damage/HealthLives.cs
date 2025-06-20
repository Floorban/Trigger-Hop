using UnityEngine;
using UnityEngine.UI;

public class HealthLives : MonoBehaviour
{
    public GameObject lifeEmpty;
    public GameObject lifeHalf;
    public GameObject lifeFull;

    public void HideAll()
    {
        lifeEmpty.SetActive(false);
        lifeHalf.SetActive(false);
        lifeFull.SetActive(false);
    }
    public void SetFull()
    {
        HideAll();
        lifeFull.SetActive(true);
    }
    public void SetHalf()
    {
        HideAll();
        lifeHalf.SetActive(true);
    }
    public void SetEmpty()
    {
        HideAll();
        lifeEmpty.SetActive(true);
    }    
}
