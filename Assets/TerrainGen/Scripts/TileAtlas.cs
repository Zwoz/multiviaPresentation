using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TieAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Environment")]
    public TileClass stone;
    public TileClass dirt;
    public TileClass grass;
    public TileClass bottomLog;
    public TileClass log;
    public TileClass log2;
    public TileClass leaf;
    public TileClass tallGrass;
    public TileClass dirtWall;
    public TileClass water;
    public TileClass bottomLayer;
    [Header("Items")]
    public TileClass torch;

    [Header("Ores")]
    //Subject to change
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;

}
