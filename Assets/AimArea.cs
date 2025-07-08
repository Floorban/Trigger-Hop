using DG.Tweening;
using Solo.MOST_IN_ONE;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AimArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform panel;
    public float scaleUp = 1.15f;
    public float scaleDuration = 0.1f;
    public float cancelDelay = 0.5f;
    public bool isPointerOver = false;
    public Image fillIn;
    public Most_HapticFeedback.CustomHapticPattern hapticPattern;

    private void Awake()
    {
        panel = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (isPointerOver)
        {
            fillIn.fillAmount += Time.deltaTime / cancelDelay;

            if (fillIn.fillAmount >= 1f)
            {
                fillIn.fillAmount = 1f;
                CancelAiming();
            }
        }
        else
        {
            fillIn.fillAmount = 0f;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        ScalePanelUp();
        Invoke(nameof(CancelAiming), cancelDelay);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        ScalePanelBack();
    }

    /*    public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerOver)
            {
                ScalePanelBack();
                CancelAiming();
            }
        }*/

    private void ScalePanelUp()
    {
        panel.DOScale(scaleUp, scaleDuration).SetEase(Ease.OutBack);
    }

    private void ScalePanelBack()
    {
        panel.DOScale(1f, scaleDuration).SetEase(Ease.InOutQuad);
    }

    private void CancelAiming()
    {
        StartCoroutine(Most_HapticFeedback.GeneratePattern(hapticPattern));
        isPointerOver = false;
        fillIn.fillAmount = 0;
        transform.localScale = Vector3.one;
        SceneController.instance.weaponManager.StopAiming();
        SceneController.instance.player.transform.rotation = Quaternion.identity;
        Debug.Log("Aiming Canceled!");
    }
}
