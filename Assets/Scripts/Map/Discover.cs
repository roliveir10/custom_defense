using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discover : MonoBehaviour
{
    public static Discover Instance { get; set; }
	[SerializeField] private Count discoverZone = new Count(6, 16);
	[SerializeField] private int isolateMax = 7;

    private void Awake()
    {
		if (Instance != this)
			Instance = this;
    }

    public void DiscoverPressed(Vector3Int cellPos)
	{
		List<Vector3Int> tilesToDiscover = new List<Vector3Int> { cellPos };
		BoundsInt closeArea = new BoundsInt(-1, -1, 0, 3, 3, 1);

		TileToDiscoverByRecursive(tilesToDiscover, cellPos, Random.Range(discoverZone.Min, discoverZone.Max + 1));
		//		Debug.Log(tilesToDiscover.Count);
		for (int i = 0; i < tilesToDiscover.Count; i++)
		{
			foreach (var b in closeArea.allPositionsWithin)
				RemoveDiscoverTile(b.x + tilesToDiscover[i].x, b.y + tilesToDiscover[i].y);
		}
		UpdateDiscoverButton();
	}

	private int TileToDiscoverByRecursive(List<Vector3Int> list, Vector3Int pos, int count)
	{
		BoundsInt closeArea = new BoundsInt(-1, -1, 0, 3, 3, 1);
		List<Vector3Int> neighb = new List<Vector3Int>();
		Vector3Int randomTile;

		MapManager.Terrain[pos.x, pos.y].Discover = true;
		if (count == 0)
			return 1;
		neighb.Clear();
		foreach (var b in closeArea.allPositionsWithin)
		{
			if (b.x == 0 && b.y == 0)
				continue;
			else if (GameFuncs.IsOnTheMap(new Vector3Int(b.x + pos.x, b.y + pos.y, 0)) && !MapManager.Terrain[b.x + pos.x, b.y + pos.y].Discover)
				neighb.Add(new Vector3Int(b.x + pos.x, b.y + pos.y, 0));
		}
		while (neighb.Count > 0)
		{
			randomTile = neighb[Random.Range(0, neighb.Count)];
			neighb.Remove(randomTile);
			list.Add(randomTile);
			if (TileToDiscoverByRecursive(list, randomTile, count - 1) == 1)
				return 1;
		}
		return 0;
	}

	private void UpdateDiscoverButton()
	{
		List<Vector3Int> isolateTile = new List<Vector3Int>();
		Vector3Int[,] tileTab = new Vector3Int[MapManager.Width, MapManager.Height];
		List<Vector3Int> tileList = new List<Vector3Int>();

		for (int x = 0; x < MapManager.Width; x++)
			for (int y = 0; y < MapManager.Height; y++)
			{
				if (MapManager.Terrain[x, y].Discover)
					tileTab[x, y] = new Vector3Int(x, y, -1);
				else
				{
					tileTab[x, y] = new Vector3Int(x, y, 0);
					tileList.Add(new Vector3Int(x, y, 0));
				}
			}
		while (tileList.Count > 0)
		{
			isolateTile.Clear();
			int a = CountDiscoverTile(isolateTile, tileList, tileList[0], tileTab);
			Debug.Log(a);
			if (a < isolateMax)
			{
				for (int i = 0; i < isolateTile.Count; i++)
					RemoveDiscoverTile(isolateTile[i].x, isolateTile[i].y);
			}
		}
		for (int x = 0; x < MapManager.Width; x++)
            for (int y = 0; y < MapManager.Height; y++)
            {
				if (MapManager.Terrain[x, y].DiscoverButton && MapManager.Terrain[x, y].Discover)
					RemoveButtonDiscovered(MapManager.Terrain[x, y]);
			}
		MapManager.Map.InstantiateDiscoverButton();
	}

    private int CountDiscoverTile(List<Vector3Int> isolateTile, List<Vector3Int> tileList, Vector3Int currTile, Vector3Int[,] tileTab)
    {
		BoundsInt area = new BoundsInt(0, 0, 0, 2, 2, 1);
		foreach (var b in area.allPositionsWithin)
        {
			Vector3Int tile = new Vector3Int(currTile.x + b.x, currTile.y + b.y, 0);
            if (GameFuncs.IsOnTheMap(tile) && tileTab[tile.x, tile.y].z != -1)
            {
				isolateTile.Add(tile);
				tileList.Remove(tile);
				tileTab[tile.x, tile.y].z = -1;
				CountDiscoverTile(isolateTile, tileList, tile, tileTab);
            }
        }
		return isolateTile.Count;
    }

    private void RemoveDiscoverTile(int x, int y)
    {
		Vector3Int tilePos = new Vector3Int(x, y, 0);
		if (GameFuncs.IsOnTheMap(tilePos))
        {
			MapManager.Terrain[x, y].Discover = true;
            MapManager.Tilemap.SetTile(tilePos, MapManager.Tiles[(int)MapManager.Terrain[x, y].Type]);
        }
    }

    private void RemoveButtonDiscovered(MapTile tile)
    {
		Destroy(tile.GoAssociated);
		tile.DiscoverButton = false;
    }
}