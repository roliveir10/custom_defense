using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static GameStats Instance { get; set; }

    [SerializeField] private BaseStats coin = null;
    [SerializeField] private BaseStats wood = null;

    public static BaseStats Coin
    {
        get { return Instance.coin; }
        set { Instance.coin = value; }
    }
    public static BaseStats Wood
    {
        get { return Instance.wood; }
        set { Instance.wood = value; }
    }

    void Awake()
    {
        if (Instance != this)
            Instance = this;
    }

    private void Update()
    {
        coin.UpdateStat();
        wood.UpdateStat();
    }
}