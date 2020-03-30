using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private GameObject prefab = null;

    public void BuyItem()
    {
        Building b = prefab.GetComponent<Building>();

        switch(b.CostType)
        {
            // TODO : add checks to see if the player can purchase
            case GameConsts.COST_TYPE.GOLD:
                {
                    if (GameStats.Coin.Currency >= b.Cost)
                        InstantiateItem();
                    break;
                }
            case GameConsts.COST_TYPE.WOOD:
                {
                    if (GameStats.Wood.Currency >= b.Cost)
                        InstantiateItem();
                    break;
                }
        }
    }

    private void InstantiateItem()
    {
        Vector2 posToInstantiate;

        if (MapManager.Map.BuildingBeingPlaced)
        {
            MapManager map = MapManager.Map;

            posToInstantiate = MapManager.Grid.CellToWorld(map.ObjectBuilding.GetComponent<Building>().CellPos);
            Destroy(map.ObjectBuilding);
        }
        else
            posToInstantiate = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        GameFuncs.InstantiateBuilding(prefab, posToInstantiate);
        UIManager.Instance.ChangeBuildMenu();
    }
}