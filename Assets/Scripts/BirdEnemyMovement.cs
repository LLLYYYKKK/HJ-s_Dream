using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemyMovement : EnemyMovement {
	float hitInterval = 1f;
	float hitTimer = 0f;
	float circleMoveSpeed = 0f;
	public float circleSpeed = 5f;
	bool isHit = false;
	public float minRadius = 0.3f;
	public float maxRadius = 1f;
	float radius;

	Vector2 circleCenter;

	protected override void Awake ()
	{
		base.Awake ();
		radius = Random.Range (minRadius, maxRadius);
		circleMoveSpeed = Mathf.Deg2Rad * Random.Range (0, 360);
		circleCenter = transform.position;
	}

	protected override void Update ()
	{
		base.Update ();
		if (isAlive) {
			if (isHit) {
				hitTimer += Time.deltaTime;
				if (hitTimer >= hitInterval) {
					hitTimer = 0f;
					isHit = false;
				}
			}	
		}
	}
		
	protected override void FixedUpdateCharacter ()
	{
		if (canMove) {
			circleMoveSpeed += Time.fixedDeltaTime * circleSpeed;
			if (circleMoveSpeed >= Mathf.Deg2Rad * 360F) {
				circleMoveSpeed = 0F;
			}
			float x = radius * Mathf.Cos (circleMoveSpeed) + circleCenter.x;
			float y = radius * Mathf.Sin (circleMoveSpeed) + circleCenter.y;
			transform.position = new Vector3 (x, y, 0f);

			float step = GetSpeed() * Time.fixedDeltaTime;
			Vector2 moveTowards = Vector2.MoveTowards (circleCenter, destination, step);
			circleCenter = moveTowards;
		}
	}


	protected override void StartNewAttack ()
	{
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (hitTimer == 0f && isAlive) {
			if (other.tag == "Player") {
				HitByBody (other.GetComponent<CharacterMovement>());
			}
		}
	}

	protected virtual void HitByBody(CharacterMovement characterMovement) {
		characterMovement.Hit (GetAttackPower(), this);
		isHit = true;
	}
}
