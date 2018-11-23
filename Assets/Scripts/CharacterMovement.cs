using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour {
	[Range(0.0f, 5f)] public float speed = 2f;
	[Range(0.5f, 10f)] public float attackRange = 0.5f;
	[Range(0.2f, 5f)] public float attackTime = 1.0f;
	[Range(0f, 0.4f)] public float skillCoolTimeReductionRate;
	public string name;

	public float attackPower = 2.0f;

	public float hp = 10.0f;
	public float maxHp = 10.0f;
	public float deadTime;

	public bool isAlwaysTracingTarget = true;

	public GameObject hitObject;
	public AudioClip attackSound;
	public AudioClip hitSound;
	public AudioClip deadSound;
	public AudioClip moveSound;

	[System.NonSerialized] public bool isActive;
	[System.NonSerialized] public bool canMove;
	[System.NonSerialized] public bool canAttack;
	[System.NonSerialized] public GameObject attackTarget;

	[System.NonSerialized] public Vector2 destination;
	[System.NonSerialized] public Transform hitObjectSpawnPoint;

	protected Rigidbody2D rbody2D;
	protected Animator animator;
	public Animator Animator {
		get{
			return animator;
		}
	}
	protected SpriteRenderer spriteRenderer;
	protected GameObject shadow;
	protected AudioSource audioSource;
	protected UICanvas uiCanvas;

	public Transform center;

	float velocity;
	float totalAttackTimer;
	float deadTimer;

	bool isPreAttackDone;
	bool onAttack;
	bool canHitTarget;

	Vector2 prevPosition;

	[Range(0, 3)] int direction;
	public int Direction {
		get {
			return direction;
			}
	}

	protected virtual void Awake() {
		rbody2D = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		audioSource = GetComponent<AudioSource> ();
		center = transform.Find ("Center");
		hitObjectSpawnPoint = transform.Find ("HitObjectSpawnPoint");
		uiCanvas = GameObject.FindGameObjectWithTag ("UICanvas").GetComponent<UICanvas> ();
		CreateShadow ();

		isActive = true;
		canMove = true;
		canAttack = true;
		isPreAttackDone = true;
		canHitTarget = true;

		direction = 1;

		destination = transform.position;
		velocity = 0.0f;
		totalAttackTimer = 0.0f;

		prevPosition = transform.position;

		deadTimer = 0.0f;
	}

	protected virtual void Update() {
		HitRecover ();
		CheckDead ();

		if (isAlwaysTracingTarget) {
			if (attackTarget != null) {
				if (!attackTarget.GetComponent<CharacterMovement>().isActive) {
					CancleAttack ();
					canAttack = true;
				} else {
					SetDirectionTo (attackTarget.transform.position);
				}
			}
		}

		if (!isPreAttackDone) {
			totalAttackTimer += Time.deltaTime;

			if (totalAttackTimer >= attackTime) {
				AttackDone ();
			}
		}

		if (isActive) {
			if (canAttack) {
				if (IsAttackTargetInRange ()) {
					if (isPreAttackDone) {
						StartNewAttack ();
					} else {
						WaitPreAttack ();
					}
				} else {
				}
			}
				
		}

		animator.SetFloat ("Velocity", velocity);
		animator.SetInteger ("Direction", direction);
		animator.SetBool ("OnAttack", onAttack);
		animator.SetBool ("CanMove", canMove);

		UpdateShadow ();
	}

	protected virtual void FixedUpdate () {
		FixedUpdateCharacter ();
	}

	protected void FixedUpdateCharacter ()
	{
		if (isActive) {
			if (canMove) {
				float step = speed * Time.fixedDeltaTime;
				Vector2 moveTowards = Vector2.MoveTowards (transform.position, destination, step);
				MovePosition (moveTowards);
			}
			else {
				MovePosition (transform.position);
			}
		
			velocity = Vector2.Distance (prevPosition, transform.position);
			prevPosition = transform.position;
		}
	}

	void CreateShadow ()
	{
		shadow = new GameObject ("Shadow");
		shadow.transform.SetParent (transform, false);
		shadow.transform.localPosition = new Vector2 (0f, -0.1f);
		SpriteRenderer shadowSpriteRenderer = shadow.AddComponent<SpriteRenderer> ();
		shadowSpriteRenderer.sortingLayerName = "Shadow";
		Color shadowColor = new Color (0f, 0f, 0f, 0.5f);
		shadowSpriteRenderer.color = shadowColor;
	}

	void UpdateShadow() {
		shadow.GetComponent<SpriteRenderer> ().sprite = spriteRenderer.sprite;
		Vector2 spriteLocalScale = spriteRenderer.transform.localScale;
		shadow.transform.localScale = new Vector2 (spriteLocalScale.x * 0.7f, spriteLocalScale.y * 0.3f);
	}

	protected virtual void MovePosition(Vector2 position) {
		if (rbody2D.bodyType != RigidbodyType2D.Static) {
			rbody2D.MovePosition (position);
		}
	}

	public virtual void SetDirectionTo(Vector2 destination) {
		this.destination = destination;
	
		direction = CalculateDirection (destination);
	}

	public void Stop() {
		destination = transform.position;
		CancleAttack ();
		canMove = true;
	}

	int CalculateDirection(Vector2 target) {
		float distanceX = target.x - transform.position.x;
		float distanceY = target.y - transform.position.y;

		if (Mathf.Abs (distanceX) < Mathf.Abs (distanceY)) {
			if (distanceY > 0.0f) {
				return 0;
			} else {
				return 1;
			}
		} else {
			if (distanceX < 0.0f) {
				return 2;
			} else {
				return 3;
			}
		}		
	}

	bool IsAttackTargetInRange ()
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll (center.transform.position, attackRange);
		foreach (Collider2D collider2D in colliders) {
			if (collider2D.gameObject == attackTarget) {
				return true;
			}
		}

		return false;
	}

	void StartNewAttack ()
	{
		totalAttackTimer = 0.0f;
		destination = transform.position;
		canMove = false;
		isPreAttackDone = false;
		onAttack = true;
		canHitTarget = true;
		direction = CalculateDirection (new Vector2 (attackTarget.transform.position.x, attackTarget.transform.position.y));
		animator.speed = 1.0f / attackTime;
	}

	public void AttackDone ()
	{
		animator.speed = 1.0f;
		isPreAttackDone = true;
		onAttack = false;
		canMove = true;
	}

	public void CancleAttack ()
	{
		attackTarget = null;
		canAttack = false;
		canMove = true;
		animator.speed = 1.0f;
		onAttack = false;

		if (canHitTarget) {
			AttackDone ();
		}
	}

	void WaitPreAttack ()
	{
		canMove = false;
	}

	void HitTarget ()
	{
		if (attackTarget != null && canHitTarget) {
			HitTargetAction ();
			canHitTarget = false;
		}
	}

	protected virtual void HitTargetAction() {
		audioSource.PlayOneShot (attackSound);

		HitObject instantiatedHitObject = Instantiate (hitObject).GetComponent<HitObject>();
		instantiatedHitObject.transform.position = hitObjectSpawnPoint.transform.position;
		instantiatedHitObject.damage = attackPower;
		instantiatedHitObject.Hit (this, attackTarget.transform.Find ("Center").position, attackTarget.tag);
	}

	public virtual void Hit(float damage, CharacterMovement attacker) {
		hp -= damage;
		audioSource.PlayOneShot (hitSound);
		uiCanvas.Hit (this, damage);

		spriteRenderer.color = new Color (1, 0, 0);

		if (hp <= 0) {
			isActive = false;
			velocity = 0;
			onAttack = false;

			animator.SetTrigger ("Dead");
			audioSource.PlayOneShot (deadSound);
			foreach (Collider2D collider2D in GetComponents<Collider2D> ()) {
				Destroy(collider2D);
			}
		}
	}

	protected virtual void HitRecover ()
	{
		Color spriteColor = spriteRenderer.color;
		if (spriteColor.g < 1.0f || spriteColor.b < 1.0f) {
			spriteColor.g += 0.1F;
			spriteColor.b += 0.1f;

			if (spriteColor.g > 1.0f) {
				spriteColor.g = 1.0f;
			}

			if (spriteColor.b > 1.0f) {
				spriteColor.b = 1.0f;
			}

			spriteRenderer.color = spriteColor;
		}
	}

	void CheckDead ()
	{
		if (hp <= 0) {
			Color spriteColor = spriteRenderer.color;
			spriteColor.a -= Time.deltaTime / deadTime;
			spriteRenderer.color = spriteColor;

			deadTimer += Time.deltaTime;
			if (deadTimer >= deadTime) {
				DeadAction ();
			}
		}
	}

	protected virtual void DeadAction ()
	{
		uiCanvas.Dead (this);
		Destroy (gameObject);
	}

	void PlayMoveSound() {
		audioSource.PlayOneShot (moveSound);
	}

	public void See (Vector2 position)
	{
		direction = CalculateDirection (position);
		animator.SetInteger ("Direction", direction);
	}
}
