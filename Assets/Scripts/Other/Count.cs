using UnityEngine;

[System.Serializable]
public class Count
{
	[SerializeField] private int minimum;
	[SerializeField] private int maximum;

    public int Min
    {
        get { return minimum; }
    }

    public int Max
    {
        get { return maximum; }
    }

	public Count(int min, int max)
	{
		minimum = min;
		maximum = max;
	}
}
