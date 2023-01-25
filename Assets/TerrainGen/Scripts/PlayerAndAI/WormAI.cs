using UnityEngine;

public class WormAI : MonoBehaviour
{
    // Variables to control the movement of the worm
    public float speed = 2f;
    public float changeDirectionTime = 2f;
    private Vector2 direction;
    private float timer;

    // Variable to store the player
    public GameObject player;

    // Variable to store the detection range
    public float detectionRange = 2f;

    // variable to store the hit player timer
    private float hitPlayerTimer = 0;

    // variable to store the out of range timer
    private float outOfRangeTimer = 0;

    TerrainGeneration terrainGen;

    Health health;

    private void Start()
    {
        // Set the initial direction and start the timer
        direction = RandomDirection();
        timer = changeDirectionTime;
        terrainGen = FindObjectOfType<TerrainGeneration>();
        // Find the player object
        player = GameObject.FindWithTag("Player");
        health = GetComponent<Health>();
    }

    private void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;
        if (hitPlayerTimer > 0)
        {
            hitPlayerTimer -= Time.deltaTime;
        }

        // Check if the player is in range
        if (Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            // reset out of range timer
            outOfRangeTimer = 0;
            // Move towards the player
            direction = (player.transform.position - transform.position).normalized;
        }
        else
        {
            outOfRangeTimer += Time.deltaTime;
            // Check if it's time to change direction
            if (timer <= 0 && hitPlayerTimer <= 0)
            {
                direction = RandomDirection();
                timer = changeDirectionTime;
            }
        }
        // Restrict the maximum y level
        if (transform.position.y > terrainGen.mapHeight + 4)
        {
            direction = DownDirection();
        }
        //if (outOfRangeTimer > 8f)
        //{
        //    // Despawn the worm
        //    Destroy(gameObject);
        //}

        // Move the worm
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    // function to detect collision with player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hitPlayerTimer = 4f;
        }
    }

    private Vector2 RandomDirection()
    {
        // Generate a random direction
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                return Vector2.up;
            case 1:
                return Vector2.down;
            case 2:
                return Vector2.left;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
    private Vector2 DownDirection()
    {
        return Vector2.down;
    }
}
