using System;
using UnityEngine;

public class Coin : Pickup
{
    public int tipAmount;
    public static event Action<int> OnCoinCollected;

    public override void Collect()
    {
        OnCoinCollected?.Invoke(tipAmount);
        Debug.Log("tip collected");
        Destroy(gameObject);
    }
}
