using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;

public class MobSpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject[] mobPrefab;
    public int[] mobWeights;
    public int maxMobs = 10;
    public float despawnDistance = 10f;
    public Transform player;
    public float spawnInterval = 30f;
    private float spawnTimer = 0f;
    private List<GameObject> spawnedMobs = new List<GameObject>();
    public Tilemap[] blockingTilemaps;
    void Start()
    {
        
        // Initialize the weight array
        int totalWeight = 0;
        for (int i = 0; i < mobWeights.Length; i++)
        {
            totalWeight += mobWeights[i];
        }
        for (int i = 0; i < mobWeights.Length; i++)
        {
            mobWeights[i] = mobWeights[i] * 100 / totalWeight;
        }
    }
    void Update()
    {
        transform.position = player.transform.position;
        spawnTimer += Time.deltaTime;

        // Spawn mobs if there are fewer than the maximum allowed and spawn timer is greater than the spawn interval
        if (spawnedMobs.Count < maxMobs && spawnTimer >= spawnInterval)
        {
            // Choose a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Vector3 spawnPos = spawnPoints[spawnIndex].transform.position;

            // Ensure the spawn point is not above a certain y level
            if (spawnPos.y <= 125)
            {
                // check if there is a tile on the spawn point
                bool isBlocked = false;
                foreach (Tilemap tilemap in blockingTilemaps)
                {
                    if (tilemap.HasTile(tilemap.WorldToCell(spawnPos)))
                    {
                        isBlocked = true;
                        break;
                    }
                }

                if (!isBlocked)
                {
                    Debug.Log("1");
                    //Choose a random mob based on the weight
                    int random = Random.Range(0, 100);
                    int mobIndex = 0;
                    int weightSum = 0;
                    for (int i = 0; i < mobWeights.Length; i++)
                    {
                        weightSum += mobWeights[i];
                        if (random < weightSum)
                        {
                            mobIndex = i;
                            break;
                        }
                    }
                    GameObject mob = Instantiate(mobPrefab[mobIndex], spawnPos, Quaternion.identity);
                    spawnedMobs.Add(mob);
                    spawnTimer = 0f;
                }
            }
        }

        for (int i = spawnedMobs.Count - 1; i >= 0; i--)
        {
            GameObject mob = spawnedMobs[i];
            if (mob == null || Vector3.Distance(mob.transform.position, player.position) > despawnDistance)
            {
                if (mob != null)
                {
                    Destroy(mob);
                }
                spawnedMobs.RemoveAt(i);
            }
        }

    }
}
