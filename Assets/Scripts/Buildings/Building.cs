using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private Vector3Int cellPos;
    [SerializeField] private GameObject sprite;
    [SerializeField] private BaseStats health = null;
    [SerializeField] private int cost = 0;
    [SerializeField] private GameConsts.COST_TYPE costType;
    [SerializeField] private float buildTime;
    [SerializeField] private float timeToMake;
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private bool isDragging;
    [SerializeField] private bool isBuilding;
    [SerializeField] private bool isCompleted;
    [SerializeField] private bool isBeingPlaced;
    [SerializeField] private GameObject buildArea;
    [SerializeField] private GameObject draggingPanel;
    [SerializeField] private bool isPlaceable;
    //upgrades

    public GameObject Prefab
    {
        get { return prefab; }
        set { prefab = value; }
    }
    public Vector3Int CellPos
    {
        get { return cellPos; }
        set { cellPos = value; }
    }
    public GameObject Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }
    public BaseStats Health
    {
        get { return health; }
    }
    public int Cost
    {
        get { return cost; }
        set { cost = value; }
    }
    public GameConsts.COST_TYPE CostType
    {
        get { return costType; }
        set { costType = value; }
    }
    public float TimeToMake
    {
        get { return timeToMake; }
        set { timeToMake = value; }
    }
    public GameObject BuildingPanel
    {
        get { return buildingPanel; }
        set { buildingPanel = value; }
    }
    public bool IsDragging
    {
        get { return isDragging; }
        set { isDragging = value; }
    }
    public bool IsBuilding
    {
        get { return isBuilding; }
        set { isBuilding = value; }
    }
    public bool IsCompleted
    {
        get { return isCompleted; }
        set { isCompleted = value; }
    }
    public bool IsBeingPlaced
    {
        get { return isBeingPlaced; }
        set { isBeingPlaced = value; }
    }
    public GameObject BuildArea
    {
        get { return buildArea; }
        set { buildArea = value; }
    }
    public GameObject DraggingPanel
    {
        get { return draggingPanel; }
        set { draggingPanel = value; }
    }
    public bool IsPlaceable
    {
        get { return isPlaceable; }
        set { isPlaceable = value; }
    }

    protected void UpdateBuilding()
    {
        if (isDragging)
        {
            Vector3 tmpWorldCellPos = GameFuncs.WorldPosToWorldCellPos(Input.mousePosition);
            Vector3Int tmpCellPos = MapManager.Grid.WorldToCell(tmpWorldCellPos);
            if (GameFuncs.IsOnTheMap(tmpCellPos))
            {
                transform.position = tmpWorldCellPos;
                cellPos = tmpCellPos;
            }
            GetComponent<IPlaceable>().Placeable();
            BuildArea.GetComponent<Renderer>().material.color = isPlaceable ? Color.green : Color.red;
        }
        else
        {
            if (isBuilding)
            {
                if (buildTime < timeToMake)
                    buildTime += Time.deltaTime;
                else
                {
                    isBuilding = false;
                    isCompleted = true;
                }
            }
        }
    }

    public void InstantiateNewBuilding()
    {
        Vector2 posToInstantiate = MapManager.Grid.CellToWorld(cellPos + new Vector3Int(0, -1, 0));

        switch (costType)
        {
            // TODO : add checks to see if the player can purchase
            case GameConsts.COST_TYPE.GOLD:
                {
                    if (GameStats.Coin.Currency >= cost)
                        GameFuncs.InstantiateBuilding(prefab, posToInstantiate);
                    break;
                }
            case GameConsts.COST_TYPE.WOOD:
                {
                    if (GameStats.Wood.Currency >= cost)
                        GameFuncs.InstantiateBuilding(prefab, posToInstantiate);
                    break;
                }
        }
    }
}
