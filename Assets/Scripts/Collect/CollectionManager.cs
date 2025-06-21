using TMPro;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private int numOfTip = 0;
    [SerializeField] private TextMeshProUGUI amountText;
    private void OnEnable()
    {
        Coin.OnCoinCollected += TipCollected;
    }
    private void OnDisable()
    {
        Coin.OnCoinCollected -= TipCollected;
    }
    private void TipCollected(int increasedAmount)
    {
        numOfTip += increasedAmount;
        amountText.text = numOfTip.ToString();
    }
    public int GetTips() => numOfTip;
    public void ConsumeTips(int amount) => numOfTip -= amount;
}
