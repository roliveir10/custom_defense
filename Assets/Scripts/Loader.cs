using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{
    public void Click_play()
    {
        UIManager.Instance.ChangeLobby();
        MapManager.Map.MapSetup();
        Camera.main.transform.position = MapManager.Grid.CellToWorld(new Vector3Int(MapManager.StartPosition.x, MapManager.StartPosition.y, -10));
    }
}
