using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptBuild : MonoBehaviour
{
    public void ValidateBuild(GameObject building)
    {
        Building b = building.GetComponent<Building>();
        if (b.IsPlaceable && GameStats.Coin.Currency >= b.Cost)
        {
            StartBuilding(b);
            EndBuilding();
            if (GameStats.Coin.Currency >= b.Cost)
                b.InstantiateNewBuilding();
        }
    }

    public void CancelBuild(GameObject building)
    {
        Destroy(building);
        EndBuilding();

    }

    private void EndBuilding()
    {
        MapManager.Map.BuildingBeingPlaced = false;
        MapManager.Map.ObjectBuilding = null;
    }

    private void StartBuilding(Building b)
    {
        b.IsDragging = false;
        b.IsBuilding = true;
        b.DraggingPanel.SetActive(false);
        b.IsBeingPlaced = false;
        GameStats.Coin.Currency -= b.Cost;
        MapManager.Terrain[b.CellPos.x, b.CellPos.y].CanBuild = false;
        b.Sprite.GetComponent<Renderer>().sortingLayerName = "item";
    }
}