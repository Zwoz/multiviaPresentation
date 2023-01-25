using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAI : MonoBehaviour
{
    [SerializeField] [Tooltip("when inside this radius mushroom will unhide")] float distanceToReveal;
    [SerializeField] [Tooltip("when inside this radius and revealed it will chase the player, else it will hide again")]float distanceToHide;
    [SerializeField] [Tooltip("when outside this radius this enemy will despawn")]float distanceToDespawn;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float acceleration;
    [SerializeField] float accelerationAir;
    [SerializeField] float deacceleration;
    [SerializeField] float deaccelerationAir;
    [SerializeField] float jumpVelocity;
    
    Transform playerTransform;
    Collider2D feetCollider;
    float distanceToPlayer;
    bool revealed = false;
    GameObject revealedChild;
    GameObject hiddenChild;
    GameObject damageCollider;
    Collider2D terrainDetector;
    Rigidbody2D rb;
    Health health;

    //anim
    [SerializeField] Animator animator;

    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerMovement>().transform;
        
        revealedChild = GetComponentInChildren<RevealedMushroom>().gameObject;
        hiddenChild = GetComponentInChildren<HiddenMushroom>().gameObject;
        damageCollider = GetComponentInChildren<Damage>().gameObject;
        revealedChild.SetActive(false);
        damageCollider.SetActive(false);
        feetCollider = GetComponentInChildren<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        terrainDetector = GetComponentInChildren<TerrainDetectionCollider>().gameObject.GetComponent<Collider2D>();
        health = GetComponent<Health>();
    }
    private void Update()
    {
        if (playerTransform != null)
        {
            distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            //if player is close enough and mushroom is hiding or if he gets hit he stops hiding
            if (!revealed && distanceToPlayer < distanceToReveal || !revealed && health.hitThisFrame)
            {
                revealed = true;
                revealedChild.SetActive(true);
                damageCollider.SetActive(true);
                hiddenChild.SetActive(false);
            }
            // if player is close enough then the mushroom starts chasing the player
            else if (distanceToPlayer < distanceToHide && revealed)
            {
                ChasePlayer();
            }
            //hide if player is far away
            else if (distanceToPlayer < distanceToDespawn)
            {
                Hide();
            }
            //despawn
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //slows down if there is no player
            float currentDeacceleration;
            //sets acceleration values depending on if you are on the ground or not
            if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                currentDeacceleration = deacceleration;
            }
            else
            {
                currentDeacceleration = deaccelerationAir;
            }
            rb.velocity = new Vector2(Mathf.Clamp(Mathf.Abs(rb.velocity.x) - currentDeacceleration * Time.deltaTime, 0, moveSpeed) * Mathf.Sign(rb.velocity.x), rb.velocity.y);
        }
        
    }
    void ChasePlayer()
    {
        float moveDirection = Mathf.Sign(playerTransform.position.x - transform.position.x);
        float newVelocity = 0;
        float currentAcceleration;
        float currentDeacceleration;

        //added anims
        animator.SetBool("Chase", true);
        //sets acceleration values depending on if you are on the ground or not
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            currentAcceleration = acceleration;
            currentDeacceleration = deacceleration;
        }
        else
        {
            currentAcceleration = accelerationAir;
            currentDeacceleration = deaccelerationAir;
        }
        //checks if there is vertical movement input
        if (Mathf.Abs(moveDirection) > 0)
        {
            //accelerates if input is in the same direction as current velocity, deaccelerates if its in the opposite direction
            if (moveDirection == Mathf.Sign(rb.velocity.x) || rb.velocity.x == 0)
            {
                newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) + currentAcceleration * Time.deltaTime, 0, moveSpeed) * moveDirection;
            }
            else if (moveDirection == -Mathf.Sign(rb.velocity.x))
            {
                newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) - currentDeacceleration * Time.deltaTime, 0, moveSpeed) * Mathf.Sign(rb.velocity.x);
            }
        }
        else
        {
            //deaccelerates
            newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) - currentDeacceleration * Time.deltaTime, 0, moveSpeed) * Mathf.Sign(rb.velocity.x);
        }
        rb.velocity = new Vector2(newVelocity, rb.velocity.y);
        //flip sprite the right way

        GetComponent<SpriteRenderer>().flipX = rb.velocity.x < 0;
        //this part makes him jump
        terrainDetector.offset = new Vector2(Mathf.Abs(terrainDetector.offset.x) * moveDirection, terrainDetector.offset.y);
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && terrainDetector.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
     
    }

    void Hide()
    {
        animator.SetBool("Chase", false);
        transform.position = new Vector2(Mathf.Round((float)(transform.position.x - 0.5)) + (float)0.5, transform.position.y);
        revealed = false;
        hiddenChild.SetActive(true);
        revealedChild.SetActive(false);
        damageCollider.SetActive(false);
    }
}
