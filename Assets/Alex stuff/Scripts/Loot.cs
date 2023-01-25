using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    GameObject Player;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D collider1;
    [SerializeField] private BoxCollider2D collider3;
    [SerializeField] private float moveSpeed;
    [SerializeField] CircleCollider2D collider2;
    [SerializeField] GameObject sprite;
    
    private Item item;

    private void Start()
    {
       
        Player = FindObjectOfType<PlayerMovement>().gameObject;
        
        collider1 = GetComponentInChildren<BoxCollider2D>();
        Physics2D.IgnoreCollision(collider1, Player.GetComponent<Collider2D>());

    }
    public void Initialize(Item item)
    {
        
        this.item = item;
        sr.sprite = item.image;
    }
    private void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bool canAdd = InventoryManager.instance.AddItem(item);
            if (canAdd)
            {
                StartCoroutine(MoveAndCollect(other.transform));
            }
        }
    }

    private IEnumerator MoveAndCollect(Transform target)
    {
        Destroy(collider1);
        Destroy(collider3);
        while (transform.position != target.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return 0;
        }

        Destroy(gameObject);
    }
}
