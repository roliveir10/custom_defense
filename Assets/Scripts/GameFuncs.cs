using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameFuncs
{
    public static void ChangeMenu(GameObject[] gos, int index)
    {
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].SetActive(i == index ? true : false);
        }
    }
    public static bool IsOnTheMap(Vector3Int pos)
    {
        if (pos.x < MapManager.Width && pos.x >= 0 && pos.y >= 0 && pos.y < MapManager.Height && pos.z == 0)
            return true;
        return false;
    }
    public static Vector3 WorldPosToWorldCellPos(Vector3 pos)
    {
        Vector2 screenPos = Camera.main.ScreenToWorldPoint(pos);
        Vector3Int cellPos = MapManager.Grid.WorldToCell(screenPos);
        return MapManager.Grid.CellToWorld(cellPos) + Gap();
    }
    public static Vector3 PosToWorldCellPos(Vector3 pos)
    {
        Vector3Int cellPos = MapManager.Grid.WorldToCell(pos);
        return MapManager.Grid.CellToWorld(cellPos) + Gap();
    }
    public static void InstantiateBuilding(GameObject building, Vector2 posToInstantiate)
    {
        Vector2 realPosToInstantiate = PosToWorldCellPos(posToInstantiate);
        GameObject g = GameObject.Instantiate(building, realPosToInstantiate, Quaternion.identity);
        Building b = g.GetComponent<Building>();

        b.Prefab = building;
        b.IsBeingPlaced = true;
        MapManager.Map.BuildingBeingPlaced = true;
        MapManager.Map.ObjectBuilding = g;

        Vector3Int tmpCellPos = MapManager.Grid.WorldToCell(realPosToInstantiate);
        if (IsOnTheMap(tmpCellPos))
        {
            b.CellPos = tmpCellPos;
            b.GetComponent<IPlaceable>().Placeable();
            b.BuildArea.GetComponent<Renderer>().material.color = b.IsPlaceable ? Color.green : Color.red;
        }
        else
            b.BuildArea.GetComponent<Renderer>().material.color = Color.red;
    }
    public static Vector3 Gap()
    {
        return new Vector3(0f, 0.6f, 0f);
    }
}