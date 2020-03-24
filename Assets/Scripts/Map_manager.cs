using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Map_manager : MonoBehaviour
{
	public static Map_manager instance = null;

	[Range(0, 100)]
	public int mountPercent;

	[Range(0, 100)]
	public int fencePercent;

/*	[Range(0, 8)]
	public int birth_fence_limit;

	[Range(0,8)]
	public int death_fence_limit;

	[Range(1,10)]
	public int repet_fence;
*/
    public int width;
   	public int height;
	public Tile[] tiles;
	public GameObject[] button;
	public Grid grid;
	public Vector2Int startPosition;
	public Vector2Int startArea;
	public MapTile[,] terrain_map;
	public Tilemap map;

	private Count mountCount;
	private Count fenceCount;
	private Tilemap mapSelect;
	private List<Vector2Int> gridPositions = new List<Vector2Int>();
	private float time = 0f;

	public void MapSetup()
	{
		if (instance == null)
			instance = this;
		grid = CreateGrid("Grid");
		map = CreateTilemap("map", 0);
		mapSelect = CreateTilemap("mapSelect", 5);
		InitialiseList();
		InitialiseTerrain();
		mountCount = new Count(mountPercent * width * height / 120, mountPercent * width * height / 80);
		fenceCount = new Count(fencePercent * width * height / 120, fencePercent * width * height / 80);
		Gen_map();
	}

	private void InitialiseList()
    {
		gridPositions.Clear();
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				gridPositions.Add(new Vector2Int(x, y));
    }

	private void InitialiseTerrain()
    {
		terrain_map = new MapTile[width, height];
		startPosition = RandomStartPosition();
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
			{
				terrain_map[x, y] = new MapTile(new Vector2Int(x, y));
				if (Mathf.Abs(startPosition.x - x) < startArea.x && Mathf.Abs(startPosition.y - y) < startArea.y)
					terrain_map[x, y].Discover = true;
			}
		terrain_map[startPosition.x, startPosition.y].Color = new Color(1f, 0.9f, 0.9f, 1f);
	}

	private void Gen_map()
	{
		LayoutObjectAtRandom(1, mountCount.minimum, mountCount.maximum);
		LayoutObjectAtRandom(2, fenceCount.minimum, fenceCount.maximum);
		//	for (int i = 0; i < repet_fence; i++)
		//		terrain_map = gen_fence(terrain_map);
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
			{
				if (!terrain_map[x, y].Discover)
					map.SetTile(new Vector3Int(x, y, 0), tiles[4]);
				else
					map.SetTile(new Vector3Int(x, y, 0), tiles[terrain_map[x, y].Type]);
				map.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None); // only for startPosition
				map.SetColor(new Vector3Int(x, y, 0), terrain_map[x, y].Color); // same here
			}
		InstantiateDiscoverButton();
	}

	private void LayoutObjectAtRandom(int tile, int minimum, int maximum)
    {
		int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
			Vector2Int randomPosition = RandomPosition();
			terrain_map[randomPosition.x, randomPosition.y].Type = tile;
        }
    }

	private Vector2Int RandomPosition()
    {
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector2Int randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
    }

	private Vector2Int RandomStartPosition()
    {
		Count startWidth = new Count(width / 3, width * 2 / 3);
		Count startHeight = new Count(height / 3, height * 2 / 3);

		Vector2Int randomPosition = new Vector2Int(Random.Range(startWidth.minimum, startWidth.maximum), Random.Range(startHeight.minimum, startHeight.maximum));
        int index = gridPositions.FindIndex(x => x == randomPosition);
		gridPositions.RemoveAt(index);
		return randomPosition;
    }

	public void InstantiateDiscoverButton()
	{
		BoundsInt largeArea = new BoundsInt(-5, -5, 0, 10, 10, 1);
		BoundsInt smallArea = new BoundsInt(-1, -1, 0, 3, 3, 1);
		int tileDiscovered;
		int discoverButton;

		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
			{
				if (!terrain_map[x, y].Discover && !terrain_map[x, y].DiscoverButton)
				{
					tileDiscovered = 0;
					discoverButton = 0;
					foreach (var a in smallArea.allPositionsWithin)
					{
						if (IsOnTheMap(new Vector3Int(x + a.x, y + a.y, 0)) && terrain_map[x + a.x, y + a.y].Discover)
						{
							tileDiscovered++;
							break;
						}
					}
					if (tileDiscovered > 0)
					{
						foreach (var b in largeArea.allPositionsWithin)
						{
							if (IsOnTheMap(new Vector3Int(x + b.x, y + b.y, 0)) && terrain_map[x + b.x, y + b.y].DiscoverButton)
							{
								discoverButton++;
								break;
							}
						}
					}
					if (tileDiscovered > 0 && discoverButton == 0)
					{
						terrain_map[x, y].DiscoverButton = true;
						Vector3 posToInstantiate = grid.CellToWorld(new Vector3Int(x, y, 0));
						posToInstantiate.y += 0.6f;
						GameObject go = Instantiate(button[0], posToInstantiate, Quaternion.identity);
						terrain_map[x, y].GoAssociated = go;
					}
				}
			}
	}

	/*	public int[,] gen_fence(int[,] old_map)
		{
			int[,] new_map = new int[width, height];
			int neighb;
			BoundsInt my_b = new BoundsInt(-1, -1, 0, 3, 3, 1);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					neighb = 0;
					foreach (var b in my_b.allPositionsWithin)
					{
						if (b.x == 0 && b.y == 0)
							continue;
						else if (x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height && old_map[x + b.x, y + b.y] == 2)
							neighb ++;
					}
					if (old_map[x,y] == 2 && (neighb < death_fence_limit || neighb > death_fence_limit + 1))
						new_map[x,y] = 0;
					else
						new_map[x,y] = old_map[x,y];
				}
			}
			return new_map;
		}
	*/
	private Tilemap CreateTilemap(string map, int orderInLayer)
	{
		var go = new GameObject(map);
		var tm = go.AddComponent<Tilemap>();
		var tr = go.AddComponent<TilemapRenderer>();

		tr.mode = TilemapRenderer.Mode.Individual;
		tr.sortingOrder = orderInLayer;
		tm.tileAnchor = new Vector3(0.8f, 0.8f, 0);
		go.transform.SetParent(grid.transform);

		return tm;
	}

	private Grid CreateGrid(string grid)
	{
		var go = new GameObject(grid);
		var gr = go.AddComponent<Grid>();

		gr.cellSize = new Vector3(1, 0.75f, 1);
		gr.cellLayout = GridLayout.CellLayout.Isometric;
		return gr;
	}

	public void Update()
    {
		if (Input.GetMouseButtonDown(0))
			time = Time.time;
		else if (Input.GetMouseButtonUp(0) && Input.touchCount < 2 && Time.time - time < 0.12f)
		{
			Vector2 posTouched = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3Int posCellTouched = grid.WorldToCell(posTouched);
			mapSelect.ClearAllTiles();
			if (IsOnTheMap(posCellTouched) && terrain_map[posCellTouched.x, posCellTouched.y].Discover)
				mapSelect.SetTile(posCellTouched, tiles[3]);
		}
	}

    public bool IsOnTheMap(Vector3Int pos)
    {
	    if (pos.x < width && pos.x >= 0 && pos.y >= 0 && pos.y < height)
		    return true;
		return false;
    }
}   