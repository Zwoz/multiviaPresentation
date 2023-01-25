using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Enums;

public class TerrainGeneration : MonoBehaviour
{


    [SerializeField] public Tilemap[] tileMaps;

    [HideInInspector] public TileClass[,,] tileData;
    public LightSolver lighting;

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public BiomeClass[] biomes;
    public float seed;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Texture2D biomeMap;


    [Header("Generation Settings")]
    public int chunkSize = 16;
    public int worldSize = 100;
    public int heightAddition = 25;
    public bool generateCaves = true;
    public int worldSize_Y;

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public Texture2D caveNoiseTexture;
    public Texture2D waterNoiseTexture;
    [Header("Ore Settings")]
    public OreClass[] ores;
    private BiomeClass curBiome;
    private Color[] biomeCols;
    public float liquidFreq;
    public float liquidSize;

    [Header("Misc Settings")]
    private Camera mainCam;
    private Vector3 mousePos;
    [SerializeField] private TileLayer breakLayer;
    [SerializeField] private TileClass placeTile;
    public float mapHeight;
    void Start()
    {
        lighting = GetComponent<LightSolver>();
        mainCam = FindObjectOfType<Camera>();

        
        //Terrain Gen Stuff
        worldSize_Y = worldSize + heightAddition;
        tileData = new TileClass[worldSize, worldSize_Y, tileMaps.Length];
        for (int i = 0; i < ores.Length; i++)
        {
            ores[i].spreadTexture = new Texture2D(worldSize, worldSize);
        }

        biomeCols = new Color[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].biomeCol;
        }
        seed = Random.Range(-10000, 10000);
        lighting.Init();
        DrawTextures();
        DrawBiomeMap();
        DrawCavesAndOres();
        GenerateTerrain();
        lighting.IUpdate();
        mapHeight = Mathf.PerlinNoise((worldSize + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAddition;
    }


    private void Update()
    {
        //Debug for placing and breaking blocks, Update can be removed later

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition); //position of our mouse in the world

    }
    public void DrawBiomeMap()
    {
        // Generates biome noise
        float b;
        Color col;
        biomeMap = new Texture2D(worldSize, worldSize);
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                b = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);
            }
        }
        biomeMap.Apply();
    }
    public void DrawCavesAndOres()
    {
        //Here we generate cave noise and water noise for generation

        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        waterNoiseTexture = new Texture2D(worldSize, worldSize);
        float v;
        for (int x = 0; x < caveNoiseTexture.width; x++)
        {
            for (int y = 0; y < caveNoiseTexture.height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                if (v > curBiome.surfaceValue)
                {
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                }

                else
                {
                caveNoiseTexture.SetPixel(x, y, Color.black);
                }

            }
        }
        for (int x = 0; x < waterNoiseTexture.width; x++)
        {
            for (int y = 0; y < waterNoiseTexture.height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * liquidFreq, (y + seed) * liquidFreq);
                if (v > curBiome.surfaceValue)
                {
                    waterNoiseTexture.SetPixel(x, y, Color.white);
                }

                else
                {
                    waterNoiseTexture.SetPixel(x, y, Color.black);
                }

            }
        }

    }
    public void DrawTextures()
    {
        //Generating ore noise texture
        for (int o = 0; o < ores.Length; o++)
        {
            ores[o].spreadTexture = new Texture2D(worldSize, worldSize);
            GenerateNoiseTextures(ores[o].rarity, ores[o].size, ores[o].spreadTexture);

        }

    }
    private void GenerateNoiseTextures(float frequency, float limit, Texture2D noiseTexture)
    {
        //The deep part of noise texture generation

        float v;
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                {
                    noiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    noiseTexture.SetPixel(x, y, Color.black);
                }

            }
        }

        noiseTexture.Apply();
    }

    public BiomeClass GetCurrentBiome(int x, int y)
    {
        //find the current biome for generation

        if (System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];

        return curBiome;
    }
    public void GenerateTerrain()
    {
        TileClass tileToPlace;
        for (int x = 0; x < worldSize; x++)
        {
            float height;
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                height = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * curBiome.heightMultiplier + heightAddition;
                if (y >= height)
                    break;
                if (y < height - curBiome.dirtLayerHeight)
                {

                    tileToPlace = curBiome.tileAtlas.stone; //tiletoplace is for defining what tiles to generate, default is stone

                    //Rules for spawning ores
                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawnHeight)
                        tileToPlace = tileAtlas.coal;
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                        tileToPlace = tileAtlas.iron;
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawnHeight)
                        tileToPlace = tileAtlas.gold;
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                        tileToPlace = tileAtlas.diamond;

                }
                else if (y < height - 1)
                {
                    //generate dirt on top of stone layer

                    tileToPlace = curBiome.tileAtlas.dirt;

                }
                else
                {
                    //generate grass on top of dirt layer
                    tileToPlace = curBiome.tileAtlas.grass;
                }

                if (generateCaves)
                {
                    if (y < 10f)
                    {
                        tileToPlace = curBiome.tileAtlas.bottomLayer;
                        //Bottom layer
                        PlaceTile(tileToPlace, x, y);
                    }
                    else if (caveNoiseTexture.GetPixel(x, y).r > 0.5f || caveNoiseTexture.GetPixel(x, y).r < 0.5f && y > height - curBiome.dirtLayerHeight)
                    {
                        //Generation starts here
                        PlaceTile(tileToPlace, x, y);
                    }
                    else if (caveNoiseTexture.GetPixel(x, y).r > 0.5f && y < height - curBiome.dirtLayerHeight)
                    {
                        //Generation starts here
                        PlaceTile(tileToPlace, x, y);
                    }
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f && y < height - curBiome.dirtLayerHeight || caveNoiseTexture.GetPixel(x, y).r < 0.5f && y < height - curBiome.dirtLayerHeight)
                    {
                        //Generate walls on background layer
                        PlaceTile(curBiome.tileAtlas.dirtWall, x, y);
                    }

                    if (waterNoiseTexture.GetPixel(x, y).r < 0.5f && y > height - curBiome.dirtLayerHeight && y < height - 1)
                    {
                        if (tileData[x, y, 0] != null)
                            RemoveTile(TileLayer.blocks, x, y);
                        if (tileData[x, y + 1 , 0] != null)
                            RemoveTile(TileLayer.blocks, x, y + 1);

                        PlaceTile(curBiome.tileAtlas.water, x, y);

                    }
                }

                else
                {
                    PlaceTile(tileToPlace, x, y);

                }

                if (y >= height - 1)
                {
                    
                    //Tree generation on top layer
                    int t = Random.Range(0, curBiome.treeChance);

                    if (t == 1)
                    {

                        //If we want to generate different trees in other biomes
/*                            if (curBiome.biomeName == "Desert")
                            {
                                GenerateCactus(curBiome.tileAtlas, Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                            }
*/
                                GenerateTree(x, y + 1);
                    }
                    else
                    {
                        //for mushrooms and grass on top of grass layer
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        if (i == 1)
                        {
                            if (tileData[x, y - 1, 0] != null || tileData[x, y - 2, 0] != null)
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                    PlaceTile(curBiome.tileAtlas.tallGrass, x, y + 1);
                            }

                        }

                    }

                }

            }
        }
    }
    void GenerateTree(int x, int y)
    {
        //Place Tree Tiles

        if (tileData[x, y - 1, 0] == null || tileData[x, y - 2, 0] == null) //if tile under is empty, dont place
            return;
        int random = Random.Range(0, 10);
        int height = Random.Range(2, 6);
        if (random <= 5)
        {
            for (int i = 0; i < height; i++)
            {
                PlaceTile(tileAtlas.log, x, y + i);
            }
        }
        else
        {
            for (int i = 0; i < height; i++)
            {
                PlaceTile(tileAtlas.log2, x, y + i);
            }
        }


    }
    public void RemoveTile(TileLayer layer, int x, int y, bool updateLighting = false)
    {
        if (x < 0 || x >= worldSize) return;
        if (y < 0 || y >= worldSize_Y) return;
        if ((GetTile((int)layer, x, y) is null)) return;
        tileData[x, y, (int)layer] = null; //remove from data(x,y pos)
        tileMaps[(int)layer].SetTile(new Vector3Int(x, y, 0), null); //remove from tileMap
        CheckAddOns(x, y);
        if (updateLighting) lighting.IUpdate();
    }
    private void CheckAddOns(int x, int y) //renove any addon tiles attached to this tile
    {   

        List<TileClass> tdList = new List<TileClass> ();

        if (y + 1! > worldSize_Y) tdList.Add(tileData[x, y + 1, 1]);
        else
            return;
        if (x + 1 !> worldSize) tdList.Add(tileData[x + 1, y, 1]);
        else
            return;
        if (x - 1 !< 0) tdList.Add(tileData[x - 1, y, 1]);
        else
            return;
        foreach (TileClass tempTileData in tdList)
        {
            if (tempTileData is LiquidTileClass || !(tempTileData is null))
            {
                PlaceTile(tempTileData, x, y);
                return;
            }
        }
        
/*        if (!(tileData[x, y + 1, 3] is null))
            RemoveTile(TileLayer.interactables, x, y + 1);
        if (!(tileData[x, y - 1, 3] is null))
            RemoveTile(TileLayer.interactables, x, y - 1);
        if (!(tileData[x + 1, y, 3] is null))
            RemoveTile(TileLayer.interactables, x + 1, y);
        if (!(tileData[x - 1, y, 3] is null))
            RemoveTile(TileLayer.interactables, x - 1, y);*/

        

    }
    public TileClass GetTile(int layer, int x, int y) { return tileData[x, y, layer]; } //Find layer,x,y of tiles for tiledata
    public void PlaceTile(TileClass tile, int x, int y, bool updateLighting = false)
    {
        //Place tiles, generate world
            if (x < 0 || x >= worldSize) return;
            if (y < 0 || y >= worldSize_Y) return;
            tileData[x, y, (int)tile.tileLayer] = tile; //add to data
            tileMaps[(int)tile.tileLayer].SetTile(new Vector3Int(x, y, 0), tile.tile); //add to tilemap

            tileData[x, y, (int)tile.tileLayer] = tile;
        if (updateLighting) lighting.IUpdate();
        if (tile is LiquidTileClass)
        {
            LiquidTiles waterTile = new LiquidTiles(x, y, this, (LiquidTileClass)tile);
            StartCoroutine(waterTile.CalculatePhysics());
        }
    }

    public bool isIlluminate(int x, int y) //returns true if tile at this position illuminates light
    {
        for (int i = 0; i < tileData.GetLength(2); i++)
        {
            if (tileData[x, y, i] is null) continue;
            if (tileData[x, y, i].isIlluminate) return true;
        }
        return false;
    }
}

