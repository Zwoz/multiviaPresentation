using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] float knockbackMultiplier = 1;
    Vector2 knockbackVector = Vector2.zero;

    public void ApplyKnockback(Vector2 knockbackForce)
    {
        knockbackVector = knockbackForce * knockbackMultiplier;
    }
    //adds knockback in late update so nothing just overwrites the knockback
    private void LateUpdate()
    {
        if (knockbackVector != Vector2.zero)
        {
            GetComponent<Rigidbody2D>().velocity = knockbackVector;
            knockbackVector = Vector2.zero;
        }
    }
}
