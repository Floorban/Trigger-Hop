using System;
using UnityEngine;

public class Coin : Pickup
{
    public int amount = 1;
    public static event Action<int> OnCoinCollected;

    public override void Collect()
    {
        OnCoinCollected?.Invoke(amount);
        Destroy(gameObject);
    }
}
