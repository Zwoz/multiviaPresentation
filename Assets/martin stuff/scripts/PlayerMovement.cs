using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("movement")]
	[SerializeField] float moveSpeed;
	[SerializeField] float acceleration;
	[SerializeField] float accelerationAir;
	[SerializeField] float deacceleration;
	[SerializeField] float deaccelerationAir;
	[Header("jumping")]
	[SerializeField] float jumpVelocity;
	[SerializeField] float gravityScale;
	[SerializeField] float buttonPressTimeMax;
	[SerializeField] float buttonPressTimeMin;
	[SerializeField] float jumpBufferTime;
	[Header("Dashing")]
	[SerializeField] float dashSpeed;
	[SerializeField] float dashDuration;
	[SerializeField] float dashCooldown;
	[Header("Attacking")]
	[SerializeField] float attackSpeed;
	//if you attack before combo time is over the next attack of the combo is done, else the first attack is done again
	[SerializeField] float comboTime;
	[SerializeField] bool canBufferAttacks = true;

	float moveInputX;
	[HideInInspector]public float facingDirection = 1f;
	bool jumpButtonPressed;
	Coroutine dashCoroutine;
	float timeSinceLastAttack = 100;
	float timeSinceLastDash = 100;
	float timeSinceLastJump = 100;
	bool attackBuffered = false;

	Rigidbody2D rb;
	Collider2D mainCollider;
	public Collider2D feetCollider;
	[SerializeField] Collider2D fallCollider;
	Health playerHealth;
	[SerializeField] GameObject Attack;

	[SerializeField] Animator animator;
	GameObject fallobject;
	bool canFall;
	[SerializeField] LayerMask ground;
	[SerializeField] SpriteRenderer spriteRenderer;
	public bool attacking;
	bool dashIsOn;
	bool isGrounded;
	public Transform groundCheck;
	public float checkRadius;
	private Vector3 previousPosition;
	[SerializeField] private string Player;
	[SerializeField] private string UI;
	Item item;
	BuildingSystem buildingSystem;


	private void Start()
	{
		buildingSystem = FindObjectOfType<BuildingSystem>();
		rb = GetComponent<Rigidbody2D>();
		mainCollider = GetComponent<BoxCollider2D>();
		feetCollider = GetComponentInChildren<CapsuleCollider2D>();
		//fallCollider = fallobject.GetComponent<CapsuleCollider2D>();
		canFall = false;
		rb.gravityScale = gravityScale;
		playerHealth = GetComponent<Health>();
		
		previousPosition = transform.position;
		dashIsOn = false;
		attacking = false;
	}

	private void Update()
	{
		Move();
		if (moveInputX != 0)
		{
			facingDirection = moveInputX;
			animator.SetBool("Run", true); // Set the IsRunning animation parameter to true when moving
		}
		else if (rb.velocity.x != 0)
		{
			
			facingDirection = Mathf.Sign(rb.velocity.x);
		}
		else
		{
			
			animator.SetBool("Run", false); // Set the IsRunning animation parameter to false when not moving
		}
		
		animator.SetBool("Idle", moveInputX == 0);
		
		

		animator.SetFloat("Speed", Mathf.Abs(moveInputX));
		/*else if(moveInputX == 0 && attacking == true)
		{
			animator.SetBool("Idle", false);
		}*/

		if (previousPosition != transform.position && moveInputX != 0)
		{
			if (rb.velocity.x > 0)
			{
				spriteRenderer.flipX = false;
			}
			else if (rb.velocity.x < 0)
			{
				spriteRenderer.flipX = true;
			}
			previousPosition = transform.position;
		}

		
	  /*  if (attacking)
		{
			animator.SetBool("Idle", false);
		}*/
		else if (attacking && moveInputX != 0)
		{
			animator.SetBool("Run", true);
		}

		isGrounded = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && timeSinceLastJump > 0.2;

		if (isGrounded)
		{
			animator.SetBool("Land", true);
		}
		else
		{
			animator.SetBool("Land", false);
		}

		

		timeSinceLastAttack += Time.deltaTime;
		timeSinceLastDash += Time.deltaTime;
		timeSinceLastJump += Time.deltaTime;
	}


	void OnMove(InputValue value)
	{
		moveInputX = Mathf.Round(value.Get<Vector2>().x);
		animator.SetBool("Horizontalinput", moveInputX != 0);
	}

	

	void Move()
	{
		float newVelocity = 0;
		float currentAcceleration;
		float currentDeacceleration;
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
		if (Mathf.Abs(moveInputX) > 0)
		{
			//accelerates if input is in the same direction as current velocity, deaccelerates if its in the opposite direction
			if (moveInputX == Mathf.Sign(rb.velocity.x)||rb.velocity.x == 0)
			{
				newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) + currentAcceleration * Time.deltaTime, 0, moveSpeed) * moveInputX;
			}
			else if (moveInputX == -Mathf.Sign(rb.velocity.x))
			{
				newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) - currentDeacceleration * Time.deltaTime, 0, moveSpeed) * Mathf.Sign(rb.velocity.x);
			}
			
		}
		else
		{
			//deaccelerates
			newVelocity = Mathf.Clamp(Mathf.Abs(rb.velocity.x) - currentDeacceleration * Time.deltaTime, 0, moveSpeed) * Mathf.Sign(rb.velocity.x);
		}
		rb.velocity = new Vector2 (newVelocity, rb.velocity.y);
	}

	void OnJump(InputValue value)
	{

		jumpButtonPressed = value.isPressed;
		if (jumpButtonPressed)
        {
			StartCoroutine(Jump());
		}
	}

	IEnumerator Jump()
	{

		float currentBufferTimer = 0;
		bool hasJumped = false;
		//input buffer so you could jump if you start pressing the button a bit before hitting the ground
		while (currentBufferTimer <= jumpBufferTime && !hasJumped)
		{
			if (Physics2D.Raycast(transform.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground")))
			{
				animator.SetTrigger("Jump");
				float currentTimer = 0;
				//while the timer is below max and the jump button is pressed the velocity is set to jump velocity
				while (currentTimer < buttonPressTimeMax && jumpButtonPressed || currentTimer < buttonPressTimeMin)
				{
					timeSinceLastJump = 0;
					hasJumped = true;
					rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
					currentTimer += Time.deltaTime;
					yield return new WaitForEndOfFrame();


				}


			}
			currentBufferTimer += Time.deltaTime;
			yield return new WaitForEndOfFrame();

		}


	}

	void OnDash(InputValue Value)
	{
		if (dashCoroutine == null && timeSinceLastDash > dashCooldown)
		{
			dashCoroutine = StartCoroutine(Dash());
		}
	}

	IEnumerator Dash()
	{
		//dashing turns gravity off during the dash, sets the player speed to dash speed, then turns gravity back on at the end.
		//also makes invincible for the dash duration
		rb.gravityScale = 0;
		float dashTimer = 0;
		Vector2 dashVector = new Vector2(dashSpeed * facingDirection, 0);
		animator.SetBool("Dash", true);
		while (dashTimer <= dashDuration)
		{
			dashIsOn = true;
			playerHealth.invincibleOther = true;
			rb.velocity = dashVector;
			dashTimer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		animator.SetBool("Dash", false);
		rb.gravityScale = gravityScale;
		dashCoroutine = null;
		playerHealth.invincibleOther = false;
		
		
	}
	void OnAttack(InputValue value)
	//checks if the player currently has an attack object attached
	{
		if (GetComponentInChildren<Attack>(true) != null && buildingSystem.item.type == ItemType.Weapon)
		{
			//checks if enough time has passed since the last attack. if not, you can buffer one attack
			if (timeSinceLastAttack > attackSpeed)
			{
				AttackMethod();
			}
			else if (!attackBuffered && canBufferAttacks)
			{
				Invoke("AttackMethod", attackSpeed - timeSinceLastAttack);
				attackBuffered = true;
			}
		}
		else
			return;
		
		
	}
	void AttackMethod()
	{
		//checks if the cursor is left or right to the player to determine in which direction it should attack.
		//if the last attack was less than combo time, perform next attack
		Vector2 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
		Attack.transform.localPosition = new Vector2(Mathf.Abs(Attack.transform.localPosition.x) * Mathf.Sign(Input.mousePosition.x - playerScreenPoint.x), 
													Attack.transform.localPosition.y);
		spriteRenderer.flipX = (Input.mousePosition.x - playerScreenPoint.x < 0);
		attacking = true;
		animator.SetTrigger("Attack");
		StartCoroutine(Attackcancel());
		timeSinceLastAttack = 0;
		attackBuffered = false;
	}

	public void SetAttackActive()
	{
		Attack.SetActive(true);
		Invoke("SetAttackInactive", 0.1f);
	}
	public void SetAttackInactive()
	{
		Attack.SetActive(false);
	}

   public void DeathAnim()
	{
		animator.SetTrigger("Death");
		InputSystem.DisableAllEnabledActions();
	}

	public void TakeDmg()
	{
		Debug.Log("TOOK DAMAGE");
		animator.SetTrigger("TakeDMG");
	}

	private void ReturnToLife()
	{
	   
	}

	IEnumerator Attackcancel()
	{
		yield return new WaitForSeconds(1);
		animator.ResetTrigger("Attack");
	}

	void Land()
	{
		canFall = true;
		//anim.SetBool("Land", true);
	}

	void DisableLand()
	{
		canFall = false;
		//anim.SetBool("Land", false);
	}

	void TriggerJump()
	{
		//anim.SetBool("Jump", true);
	}

	void DisableJump()
	{
		//anim.SetBool("Jump", false);
	}

	void disableAttack()
	{
		attacking = false;
	}
}
