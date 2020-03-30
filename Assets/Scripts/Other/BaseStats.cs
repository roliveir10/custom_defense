using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseStats
{
    [SerializeField] private float currency;
    [SerializeField] private float maxCurrency;
    public Text statText;
    public Image statImage;

    public float Currency
    {
        get { return currency; }
        set
        {
            if (value >= maxCurrency)
                currency = maxCurrency;
            else if (value <= 0)
                currency = 0;
            else
                currency = value;
        }
    }

    public float MaxCurrency
    {
        get { return maxCurrency; }
        set { maxCurrency = value; }
    }

    public void UpdateStat()
    {
        if (statText != null)
            statText.text = ToString();
        if (statImage != null)
            statImage.fillAmount = Percent();
    }

    public override string ToString()
    {
        return string.Format("{0} / {1}", currency, maxCurrency);
    }

    public float Percent()
    {
        return currency / maxCurrency;
    }
}