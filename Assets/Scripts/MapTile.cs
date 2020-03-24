using UnityEngine;

public class MapTile
{
    private Vector2Int position;

    public MapTile (Vector2Int pos)
    {
        position = pos;
    }

    public Vector2Int Pos
    {
        get { return position; }
        set { position = value; }
    }

    public int Type { get; set; } = 0;

    public bool Discover { get; set; } = false;

    public bool DiscoverButton { get; set; } = false;

    public GameObject GoAssociated { get; set; } = null;

    public Color Color { get; set; } = Color.white;
}
