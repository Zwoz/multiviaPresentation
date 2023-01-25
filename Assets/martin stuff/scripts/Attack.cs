using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] int numberOfAttacks;
    [SerializeField] float attackSizeModifier;
    [SerializeField] RuntimeAnimatorController smallAttack;
    [SerializeField] RuntimeAnimatorController bigAttack;
    
    int currentAttack = 1;
    Animator attackAnimator;
    public bool useBigAttack;
    Vector2 baseColliderSize;
    BoxCollider2D attackCollider;
    float startPositionX;
    bool currentAttackSizeBig;
    private void Start()
    {
        attackAnimator = GetComponent<Animator>();
        if (!useBigAttack)
        {
            attackAnimator.runtimeAnimatorController = smallAttack;
        }
        else
        {
            attackAnimator.runtimeAnimatorController = bigAttack;
        }
        
        attackCollider = GetComponent<BoxCollider2D>();
        baseColliderSize = attackCollider.size;
        startPositionX = Mathf.Abs(transform.position.x);
    }
    private void Update()
    {
        //swaps between big and small attack if necessary
        if (!useBigAttack && currentAttackSizeBig)
        {
            attackAnimator.runtimeAnimatorController = smallAttack;
            attackCollider.size = baseColliderSize;
            transform.localPosition = new Vector2(startPositionX, transform.localPosition.y);
            currentAttackSizeBig = false;
        }
        else if (useBigAttack && !currentAttackSizeBig)
        {
            attackAnimator.runtimeAnimatorController = bigAttack;
            attackCollider.size = baseColliderSize * attackSizeModifier;
            transform.localPosition = new Vector2(startPositionX + (attackCollider.size.x - baseColliderSize.x) / 2, transform.localPosition.y);
            currentAttackSizeBig = true;
        }
    }
    public void SetActive(float direction, bool nextAttack)
    {
        //sets the object to active, checks what animation it should play
        gameObject.SetActive(true);
        if (nextAttack && currentAttack < numberOfAttacks)
        {
            currentAttack++;
        }
        else
        {
            currentAttack = 1;
        }
        attackAnimator.SetInteger("CurrentAttack", currentAttack);
        transform.localPosition = new Vector2(
            Mathf.Abs(transform.localPosition.x) * direction,
            transform.localPosition.y);
    }
   
}
