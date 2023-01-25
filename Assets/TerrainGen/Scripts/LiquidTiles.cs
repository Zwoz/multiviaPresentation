using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static Enums;

public class LiquidTiles
{
    TerrainGeneration terrain;
    LiquidTileClass liquidData;
    int x;
    int y;

    public LiquidTiles(int x, int y, TerrainGeneration terrain, LiquidTileClass liquidData)
    {
        this.x = x;
        this.y = y;
        this.terrain = terrain;
        this.liquidData = liquidData;
    }

    public IEnumerator CalculatePhysics()
    {
   
        yield return new WaitForSeconds(liquidData.flowSpeed);
        if (y > 0 &&
            terrain.tileData[x, y - 1, 0] is null)
        {
            terrain.PlaceTile(liquidData, x, y - 1);
        }
        else if (x > 0 &&
            y > 0 &&
            terrain.tileData[x - 1, y, 0] is null && !(terrain.tileData[x, y - 1, 0] is null))
        {
            terrain.PlaceTile(liquidData, x - 1, y);
        }
        else if (x < terrain.worldSize &&
            y > 0 &&
            terrain.tileData[x + 1, y, 0] is null && !(terrain.tileData[x, y - 1, 0] is null))
        {
            terrain.PlaceTile(liquidData, x + 1, y);
        }

        else
            yield return null;
    }
}
