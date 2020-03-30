using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mill : Building, IInteractable, IPlaceable
{
    [SerializeField] private int amount;
    [SerializeField] private GameConsts.COST_TYPE amountType;
    [SerializeField] private float currTime = 0f;
    [SerializeField] private float rate = 0f;

    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
    public GameConsts.COST_TYPE AmountType
    {
        get { return amountType; }
        set { amountType = value; }
    }
    public float CurrTime
    {
        get { return currTime; }
        set { currTime = value; }
    }
    public float Rate
    {
        get { return rate; }
        set { rate = value; }
    }

    void IInteractable.ActionCanceled()
    {
        return;
    }
    void IInteractable.ActionDown()
    {
        if (IsBeingPlaced)
        {
            MouseManager.MouseState = GameConsts.MOUSE_STATE.DRAGGING;
            IsDragging = true;
        }
        
    }
    void IInteractable.ActionUp()
    {
        if (IsBeingPlaced)
            IsDragging = false;
        else
            BuildingPanel.SetActive(!BuildingPanel.activeInHierarchy);
    }

    private void Update()
    {
        if (!IsCompleted)
            UpdateBuilding();
        else
        {
            if (currTime >= rate)
            {
                if (amountType == GameConsts.COST_TYPE.GOLD)
                    GameStats.Coin.Currency += amount;
                currTime = 0f;
            }
            else
                currTime += Time.deltaTime;
        }
    }

    void IPlaceable.Placeable()
    {
        MapTile tile = MapManager.Terrain[CellPos.x, CellPos.y];
        IsPlaceable = tile.Type == GameConsts.TILE_TYPE.FENCE && tile.CanBuild && tile.Discover;
    }
}