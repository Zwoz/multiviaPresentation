using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;
    public Color biomeCol;

    public TileAtlas tileAtlas;

    [Header("Addons")]
    public int tallGrassChance = 10;

    [Header("Trees")]
    public int treeChance = 10;
    public int minTreeHeight = 4;
    public int maxTreeHeight = 7;

    [Header("Generation Settings")]
    public bool generateCaves = true;
    public int dirtLayerHeight = 5;
    public float surfaceValue = 0.25f;
    public float heightMultiplier = 4f;

    [Header("Noise Settings")]

    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public OreClass[] ores;
}
