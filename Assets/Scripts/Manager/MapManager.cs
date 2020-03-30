using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager Map { get; set; }

	[Range(0, 100)]
	[SerializeField] private int mountPercent = 0;

	[Range(0, 100)]
	[SerializeField] private int fencePercent = 0;

	/*	[Range(0, 8)]
		public int birth_fence_limit;

		[Range(0,8)]
		public int death_fence_limit;

		[Range(1,10)]
		public int repet_fence;
	*/
	[SerializeField] private int width = 0;
	[SerializeField] private int height = 0;
	[SerializeField] private Tile[] tiles = null;
	[SerializeField] private Vector2Int startPosition;
	[SerializeField] private Count mountCount;
	[SerializeField] private Count fenceCount;
	[SerializeField] private Vector2Int startArea;
	[SerializeField] private Tilemap mapSelect;
	[SerializeField] private bool buildingBeingPlaced = false;
	[SerializeField] private GameObject objectBuilding;

	private Grid grid;
	private MapTile[,] terrain_map;
	private Tilemap tilemap;
	[SerializeField] private GameObject[] button = null;
	private List<Vector2Int> gridPositions = new List<Vector2Int>();

	public static int Height
	{
		get { return Map.height; }
	}
	public static int Width
	{
		get { return Map.width; }
	}
    public static Grid Grid
    {
        get { return Map.grid; }
    }
    public static MapTile[,] Terrain
    {
        get { return Map.terrain_map; }
        set { Map.terrain_map = value; }
    }
    public static Tilemap Tilemap
    {
        get { return Map.tilemap; }
    }
    public static Tile[] Tiles
    {
        get { return Map.tiles; }
    }
    public static Vector2Int StartPosition
    {
        get { return Map.startPosition; }
    }
    public static Vector2Int StartArea
    {
        get { return Map.startArea; }
    }
    public static Tilemap MapSelect
    {
        get { return Map.mapSelect; }
        set { Map.mapSelect = value; }
    }
	public bool BuildingBeingPlaced
	{
		get { return buildingBeingPlaced; }
		set { buildingBeingPlaced = value; }
	}
    public GameObject ObjectBuilding
    {
        get { return objectBuilding; }
        set { objectBuilding = value; }
    }

	private void Awake()
    {
		if (Map != this)
			Map = this;
    }

    public void MapSetup()
	{
		if (Map == null)
			Map = this;
		grid = CreateGrid("Grid");
		tilemap = CreateTilemap("map", 0);
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
		LayoutObjectAtRandom(1, mountCount.Min, mountCount.Max);
		LayoutObjectAtRandom(2, fenceCount.Min, fenceCount.Max);
		//	for (int i = 0; i < repet_fence; i++)
		//		terrain_map = gen_fence(terrain_map);
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
			{
				if (!terrain_map[x, y].Discover)
					tilemap.SetTile(new Vector3Int(x, y, 0), tiles[4]);
				else
					tilemap.SetTile(new Vector3Int(x, y, 0), tiles[(int)terrain_map[x, y].Type]);
                // only for start position
				tilemap.SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
				tilemap.SetColor(new Vector3Int(x, y, 0), terrain_map[x, y].Color);
				//.........................
			}
		InstantiateDiscoverButton();
	}

	private void LayoutObjectAtRandom(int tile, int minimum, int maximum)
    {
		int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
			Vector2Int randomPosition = RandomPosition();
			terrain_map[randomPosition.x, randomPosition.y].Type = (GameConsts.TILE_TYPE)tile;
			if ((GameConsts.TILE_TYPE)tile == GameConsts.TILE_TYPE.MOUNT)
				terrain_map[randomPosition.x, randomPosition.y].CanBuild = false;
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

		Vector2Int randomPosition = new Vector2Int(Random.Range(startWidth.Min, startWidth.Max), Random.Range(startHeight.Min, startHeight.Max));
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
						if (GameFuncs.IsOnTheMap(new Vector3Int(x + a.x, y + a.y, 0)) && terrain_map[x + a.x, y + a.y].Discover)
						{
							tileDiscovered++;
							break;
						}
					}
					if (tileDiscovered > 0)
					{
						foreach (var b in largeArea.allPositionsWithin)
						{
							if (GameFuncs.IsOnTheMap(new Vector3Int(x + b.x, y + b.y, 0)) && terrain_map[x + b.x, y + b.y].DiscoverButton)
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
                        go.GetComponent<DiscoverButton>().CellPos = new Vector3Int(x, y, 0);
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

    public void SelectTile()
    {
		Vector2 posTouched = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int posCellTouched = grid.WorldToCell(posTouched);
		mapSelect.ClearAllTiles();
		if (GameFuncs.IsOnTheMap(posCellTouched) && terrain_map[posCellTouched.x, posCellTouched.y].Discover)
			mapSelect.SetTile(posCellTouched, tiles[3]);
	}
}   