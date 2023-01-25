using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TieAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Environment")]
    public TileClass stone;
    public TileClass dirt;
    public TileClass grass;
    public TileClass bottomLog;
    public TileClass log;
    public TileClass leaf;
    public TileClass tallGrass;
    public TileClass snow;
    public TileClass sand;
    [Header("Ores")]
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
