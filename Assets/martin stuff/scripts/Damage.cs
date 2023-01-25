using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] bool destroyOnDealingDamage;
    [SerializeField] string targetTag;
    [SerializeField] [Tooltip("if true, this will only deal damage on trigger enter, else it will always deal damage when target is in trigger and not invincible")] 
    bool dealDamageOnEnter = false;
    [SerializeField] Vector2 knockback;
    [SerializeField] [Tooltip("if true, apply knockback from parent transform, else apply knockback from damage transform")]
    bool knockbackFromParent;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!dealDamageOnEnter)
        {
            DealDamage(collision);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (dealDamageOnEnter)
        {
            DealDamage(collision);
        }
    }
    void DealDamage(Collider2D collision)
    {
        Knockback k = collision.gameObject.GetComponent<Knockback>();
        Health h = collision.gameObject.GetComponent<Health>();
        float knockbackDirection;
        //if the game object itself had no knockback or health this tries to get those components from the parent
        if (k == null)
        {
            k = collision.gameObject.GetComponentInParent<Knockback>();
        }
        if (h == null)
        {
            h = collision.gameObject.GetComponentInParent<Health>();
        }
        //knocks back whatever the attack hit. bool to check if the knockback direction should be away from the attack gameobject or the parent of the attack.
        //this can be helpful if the player is doing a melee attack when standing inside an enemy so you always push him away from you
        //checks if the thing you hit was on the correct layer and if the target is not invincible
        if (collision.tag == targetTag && k != null && !h.invincible)
        {
            if (!knockbackFromParent)
            {
                knockbackDirection = Mathf.Sign(collision.gameObject.transform.position.x - transform.position.x);
            }
            else
            {
                knockbackDirection = Mathf.Sign(collision.gameObject.transform.position.x - GetComponentInParent<Rigidbody2D>().gameObject.transform.position.x);
            }
            k.ApplyKnockback(new Vector2(knockback.x * knockbackDirection, knockback.y));
        }
        //checks if the thing you hit was on the correct layer and if the target is not invincible
        if (collision.tag == targetTag && h != null && !h.invincible)
        {
            h.TakeDamage(damage);
            if (destroyOnDealingDamage)
            {
                Destroy(gameObject);
            }
        }
    }
}
