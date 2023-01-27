using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;


public class Magnet : MonoBehaviour
{
    [SerializeField]
    List<Rigidbody2D> affectedBodies = new();


    private void OnTriggerEnter2D(Collider2D collision)
    {if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<Rigidbody2D>() is Rigidbody2D rigidbody)
            {
                affectedBodies.Add(rigidbody);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() is Rigidbody2D rigidbody)
        {
            affectedBodies.Remove(rigidbody);
        }
    }
    private void FixedUpdate()
    {
        foreach (var body in affectedBodies)
        {//Vi gör om transform position från en Vector 3 till en Vector 2 eftersom den inte kommer behövas.
            body.velocity = ((Vector2)transform.position - body.position).normalized;
        }
    }
}
