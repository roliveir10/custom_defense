using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDiscover : MonoBehaviour
{
	public Count discoverZone = new Count(6, 16);

	public void OnMouseDown()
    {
		AnimDiscoverButton();
    }

    public void OnMouseUp()
    {
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int mouseCellPosition = Map_manager.instance.grid.WorldToCell(mousePosition);
        Vector3Int posTouched = Map_manager.instance.grid.WorldToCell(transform.position);
		if (posTouched == mouseCellPosition)
			if (Map_manager.instance.IsOnTheMap(posTouched) && Map_manager.instance.terrain_map[posTouched.x, posTouched.y].DiscoverButton)
				DiscoverPressed(posTouched);
			else
				AnimDiscoverButton();
	}

    private void AnimDiscoverButton()
    {
		Animator animator = GetComponent<Animator>();
		if (animator)
		{
			bool isClicked = animator.GetBool("click");
			animator.SetBool("click", !isClicked);
		}
	}

	private void DiscoverPressed(Vector3Int tile)
	{
		Vector2Int buttonCellPosition = Map_manager.instance.terrain_map[tile.x, tile.y].Pos;
		List<Vector2Int> tilesToDiscover = new List<Vector2Int> { buttonCellPosition };
		BoundsInt closeArea = new BoundsInt(-1, -1, 0, 3, 3, 1);

		TileToDiscoverByRecursive(tilesToDiscover, buttonCellPosition, Random.Range(discoverZone.minimum, discoverZone.maximum + 1));
		for (int i = 0; i < tilesToDiscover.Count; i++)
		{
			foreach (var b in closeArea.allPositionsWithin)
			{
				if (Map_manager.instance.IsOnTheMap(new Vector3Int(b.x + tilesToDiscover[i].x, b.y + tilesToDiscover[i].y, 0)))
				{
					Map_manager.instance.terrain_map[b.x + tilesToDiscover[i].x, b.y + tilesToDiscover[i].y].Discover = true;
					Map_manager.instance.map.SetTile(new Vector3Int(tilesToDiscover[i].x + b.x, tilesToDiscover[i].y + b.y, 0), Map_manager.instance.tiles[Map_manager.instance.terrain_map[tilesToDiscover[i].x + b.x, tilesToDiscover[i].y + b.y].Type]);
				}
			}

		}
		UpdateDiscoverButton();
	}

	private int TileToDiscoverByRecursive(List<Vector2Int> list, Vector2Int pos, int count)
	{
		BoundsInt closeArea = new BoundsInt(-1, -1, 0, 3, 3, 1);
		List<Vector2Int> neighb = new List<Vector2Int>();
		Vector2Int randomTile;

		Map_manager.instance.terrain_map[pos.x, pos.y].Discover = true;
		if (count == 0)
			return 1;
		neighb.Clear();
		foreach (var b in closeArea.allPositionsWithin)
		{
			if (b.x == 0 && b.y == 0)
				continue;
			else if (Map_manager.instance.IsOnTheMap(new Vector3Int(b.x + pos.x, b.y + pos.y, 0)) && !Map_manager.instance.terrain_map[b.x + pos.x, b.y + pos.y].Discover)
				neighb.Add(new Vector2Int(b.x + pos.x, b.y + pos.y));
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
		for (int x = 0; x < Map_manager.instance.width; x++)
			for (int y = 0; y < Map_manager.instance.height; y++)
			{
				if (Map_manager.instance.terrain_map[x, y].DiscoverButton && Map_manager.instance.terrain_map[x, y].Discover)
				{
					Destroy(Map_manager.instance.terrain_map[x, y].GoAssociated);
					Map_manager.instance.terrain_map[x, y].DiscoverButton = false;
				}
			}
		Map_manager.instance.InstantiateDiscoverButton();
	}
}
