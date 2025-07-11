using DG.Tweening;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    private bool isOpen;
    private int numOfCoin;
    [SerializeField] private TextMeshProUGUI coinText;
    private void Awake() => shopPanel.SetActive(false);
    public void ToggleShop(bool toOpen)
    {
        if (toOpen && !isOpen)
        {
            shopPanel.SetActive(true);
            isOpen = true;
            shopPanel.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 2, 0.2f).SetEase(Ease.InOutQuad);
        }
        else
        {
            shopPanel.SetActive(false);
            isOpen = false;
            shopPanel.transform.localScale = Vector3.zero;
            shopPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }
    public void GetCoin() => numOfCoin = PlayerPrefs.GetInt("CoinCount");
    public void ConsumeCoin()
    {
        GetCoin();
        numOfCoin--;
        coinText.text = "Coins: " + numOfCoin.ToString();
    }
}
