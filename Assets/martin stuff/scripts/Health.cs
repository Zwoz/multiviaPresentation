using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp;
    public float currentHP;
    public Slider slider;
    [SerializeField] float InvincibilityTime;
    [SerializeField] TextMeshProUGUI healthDisplay;

    [HideInInspector]public bool invincible = false;
    bool invincibleRecentlyHit = false;
    [HideInInspector]public bool invincibleOther = false;
    public bool hitThisFrame;
    public PlayerMovement pm;
    bool isPlayer = false;

    private void Update()
    {
        //if this script is on the player it updates the health display
        if (gameObject.tag == "Player")
        {
            //healthDisplay.text = currentHP.ToString();
            slider.value = currentHP;
            isPlayer = true;
        }
        if (invincibleRecentlyHit || invincibleOther)
        {
            invincible = true;
        }
        else
        {
            invincible = false;
        }
        if (currentHP == 0)
        {
            Die();
        }
        

    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, maxHp);
        invincibleRecentlyHit = true;
        hitThisFrame = true;
        Invoke("SetInvincibleRecentlyHitFalse",InvincibilityTime);
        if (isPlayer)
        {
            pm.TakeDmg();
        }
        StartCoroutine(SetHitThisFrameFalse());
    }
    void SetInvincibleRecentlyHitFalse()
    {
        invincibleRecentlyHit = false;
    }
    IEnumerator SetHitThisFrameFalse()
    {
        hitThisFrame = true;
        yield return new WaitForEndOfFrame();
        hitThisFrame = false;
    }
    void Die()
    {

        if(!isPlayer)
        {
            Destroy(gameObject);
        }
        else
        {
            invincibleOther = true;
            pm.DeathAnim();
        }
       
    }
}
