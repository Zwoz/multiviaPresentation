using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    GameObject Player;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D collider1;
    [SerializeField] private float moveSpeed;
    [SerializeField] CircleCollider2D collider2;
    [SerializeField] GameObject sprite;
    bool pickup;
    private Item item;

    private void Start()
    {
        Player = FindObjectOfType<PlayerMovement>().gameObject;
        collider2 = GetComponent<CircleCollider2D>();
        collider1 = GetComponentInChildren<BoxCollider2D>();
        Physics2D.IgnoreCollision(collider1, Player.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(collider1, Player.GetComponentInChildren<Collider2D>());

    }
    public void Initialize(Item item)
    {
        
        this.item = item;
        sr.sprite = item.image;
    }
    private void Update()
    {
        if (!pickup)
        {
           collider2.transform.position = collider1.transform.position;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {pickup = true;
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
        while (transform.position != target.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return 0;
        }
        pickup = false;
        Destroy(gameObject);
    }
}
