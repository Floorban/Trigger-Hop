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
            GetCurrentCoin();
            coinText.text = $"Coins : {numOfCoin}";
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
    private void GetCurrentCoin() => numOfCoin = PlayerPrefs.GetInt("CoinCount");
    private void EarnCoin(int amount)
    {
        GetCurrentCoin();
        numOfCoin += amount;
        PlayerPrefs.SetInt("CoinCount", numOfCoin);
        coinText.text = $"Coins : {numOfCoin}";
    }
    public void ConsumeCoin()
    {
        GetCurrentCoin();
        if (numOfCoin <= 0) return;
        numOfCoin--;
        PlayerPrefs.SetInt("CoinCount", numOfCoin);
        coinText.text = $"Coins : {numOfCoin}";
    }
    private void OnEnable()
    {
        Reward.OnCoinReward += EarnCoin;
    }
    private void OnDisable()
    {
        Reward.OnCoinReward -= EarnCoin;
    }
}
