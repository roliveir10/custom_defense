using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConsts
{
    public enum COST_TYPE
    {
        GOLD,
        WOOD
    }

    public enum TILE_TYPE
    {
        GROUND,
        MOUNT,
        FENCE,
        SELECT,
        ROCK
    }

    public enum MOUSE_STATE
    {
        NONE,
        CLICK,
        UI,
        ZOOM,
        DRAGGING
    }
}
