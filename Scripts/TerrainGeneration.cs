using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class TerrainGeneration : MonoBehaviour
{
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
    

    [Header("Noise Settings")]
    public float caveFreq = 0.05f;
    public float terrainFreq = 0.05f;
    public Texture2D caveNoiseTexture;

    [Header("Ore Settings")]
    public OreClass[] ores;

    private GameObject[] worldChunks;
    private List<Vector2> worldTiles = new List<Vector2>();
    private BiomeClass curBiome;
    private Color[] biomeCols;
    // Start is called before the first frame update

    /*private void Awake()
    {
        Jobz jobz = new Jobz();
        JobHandle jobHandle = jobz.Schedule();
        jobHandle.Complete();
    }*/
    void Start()
    {
        Stopwatch sw = new();
        sw.Start();
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

        //DrawTextures();
        DrawBiomeMap();
        UnityEngine.Debug.Log("Time to generate BiomeMap " + sw.Elapsed);
        sw.Restart();
        DrawCavesAndOres();
        UnityEngine.Debug.Log("Time to generate Caves and ores" + sw.Elapsed);
        sw.Restart();
        CreateChunks();
        UnityEngine.Debug.Log("Time to generate Chucks" + sw.Elapsed);
        sw.Restart();
        GenerateTerrain();
        UnityEngine.Debug.Log("Time to generate Terrain" + sw.Elapsed);
        sw.Stop();
    }

    public void DrawBiomeMap()
    {
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
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        float v;
        float o;
        int count = 0;
        foreach (var biome in biomes)
        {
                
        }
        for (int x = 0; x < caveNoiseTexture.width; x++)
        {
            for (int y = 0; y < caveNoiseTexture.height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                if (v > curBiome.surfaceValue)
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                else
                    caveNoiseTexture.SetPixel(x, y, Color.black);

                for (int i = 0; i < ores.Length; i++)
                {
                    ores[i].spreadTexture.SetPixel(x, y, Color.black);
                    if (curBiome.ores.Length >= i + 1)
                    {
                        count++;  
                            o = Mathf.PerlinNoise((x + seed) * curBiome.ores[i].rarity, (y + seed) * curBiome.ores[i].rarity);
                        if (o > curBiome.ores[i].size)
                            ores[i].spreadTexture.SetPixel(x, y, Color.white);

                    }
                    ores[i].spreadTexture.Apply();
                }
            }
        }
        UnityEngine.Debug.Log(string.Format("Generating {0} textures",
            count));

    }
    public void DrawTextures()
    {
        
        

        for (int i = 0; i < biomes.Length; i++)
        {
           
           

            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                biomes[i].ores[o].spreadTexture = new Texture2D(worldSize, worldSize);
                GenerateNoiseTextures(biomes[i].ores[o].rarity, biomes[i].ores[o].size, biomes[i].ores[o].spreadTexture);

            }


        }

    }
    private void GenerateNoiseTextures(float frequency, float limit,  Texture2D noiseTexture)
    {
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
    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
    public BiomeClass GetCurrentBiome(int x, int y)
    {

        if (System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];

        return curBiome;
    }
    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
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
                    
                    tileSprites = curBiome.tileAtlas.stone.tileSprites;

                    if (ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[0].maxSpawnHeight)
                        tileSprites = tileAtlas.coal.tileSprites;
                    if (ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[1].maxSpawnHeight)
                        tileSprites = tileAtlas.iron.tileSprites;
                    if (ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[2].maxSpawnHeight)
                        tileSprites = tileAtlas.gold.tileSprites;
                    if (ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > ores[3].maxSpawnHeight)
                        tileSprites = tileAtlas.diamond.tileSprites;
                   
                        


                }
                else if (y < height - 1)
                {
                    tileSprites = curBiome.tileAtlas.dirt.tileSprites;
                }
                else
                {
                    tileSprites = curBiome.tileAtlas.grass.tileSprites;
                }

                if (generateCaves)
                {
                    if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                    {
                        PlaceTile(tileSprites, x, y);
                    }
                }
                else 
                {
                    PlaceTile(tileSprites, x, y);
                }

                if (y >= height - 1)
                {
                    //Tree generation on top layer
                    int t = Random.Range(0, curBiome.treeChance);

                    if (t == 1)
                    {
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            if (curBiome.biomeName == "Desert")
                            {
                                GenerateCactus(curBiome.tileAtlas, Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                            }

                            else
                            {
                                GenerateTree(curBiome.tileAtlas, x, y + 1);
                            }

                        }

                    }
                    else
                    {
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        if (i == 1)
                        {
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                {
                                    PlaceTile(curBiome.tileAtlas.tallGrass.tileSprites, x, y + 1);
                                }
                                
                            }
                        }



                    }
                }

            }
        }
   }
    void GenerateCactus(TileAtlas atlas, int treeHeight, int x, int y)
    {
        //Define Cactus Blocks
        //Check if there is a tree already spawned next to spawnpoint
        if (worldTiles.Contains(new Vector2(x - 1, y)))
        {
            return;
            //Generate Logs

        }
        else if (worldTiles.Contains(new Vector2(x + 1, y)))
        {
            return;
            //Generate Logs

        }
        else
        {
            for (int i = 0; i <= treeHeight; i++)
            {
                PlaceTile(atlas.log.tileSprites, x, y + i);
            }
        }
    }
    void GenerateTree(TileAtlas atlas, int x, int y)
    {
        //Define Tree Blocks
            //Check if there is a tree already spawned next to spawnpoint
        if (!worldTiles.Contains(new Vector2(x - 1, y)) && !worldTiles.Contains(new Vector2(x + 1, y)))
        {
            //Generate Logs

            PlaceTile(tileAtlas.log.tileSprites, x, y);
            
        //Generate Leaves
        //PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        //PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);
        //PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 3);

        //PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);
        //PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 2);

        //PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);
        //PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 2);
        }
        
    }

    public void PlaceTile(Sprite[] tileSprites, int x, int y)
    {
     if (!worldTiles.Contains(new Vector2(x, y)))
        {
            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);

            chunkCoord /= chunkSize;

            newTile.transform.parent = worldChunks[chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();

            int spriteIndex = Random.Range(0, tileSprites.Length);

            newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];
            newTile.name = tileSprites[0].name;

            //if (newTile.name == "Tree1")
            //{
            //    newTile.AddComponent<PolygonCollider2D>();
            //    newTile.AddComponent<Blockscript>();
            //}
            
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            //Add generated tiles to list, easily get position of tiles
            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
        }

    }


}

/*public struct Jobz : IJob
{
    public int worldSize;
    public int chunkSize;
    public int[,] worldChunks;
    public void Execute()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }
}*/