using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionPigeon : BirdEnemyMovement {
	public enum AttackState {Wait, Fly, BasicAttack};
	enum FlyDirection {Right, Left};
	public AttackState attackState;
	FlyDirection flyDirection;
	float flipSpeed;
	public float flySpeed;

	protected override void Awake ()
	{
		base.Awake ();
		attackState = AttackState.Wait;
		flyDirection = FlyDirection.Right;
		flipSpeed = 1f;
		isRemoveColliderWhenDead = false;
	}

	protected override void Update ()
	{
		base.Update ();
		if (isAlive) {
			switch (attackState) {
			case AttackState.Fly:
				if (canHitTarget && attackTarget != null) {
					if (Vector2.Distance (attackTarget.GetComponent<CharacterMovement> ().center.transform.position, center.transform.position) < 4f) {
						HitTargetAction ();
						canHitTarget = false;
					}
				}
				break;
			}
		} else {
			GetComponent<Collider2D> ().isTrigger = false;
		}
	}

	protected override void FixedUpdateCharacter ()
	{
		switch (attackState) {
		case AttackState.Fly:
			rbody2D.velocity = new Vector2 (flipSpeed * GetSpeed () * flySpeed, 0);
			break;
		}
	}

	public void Fly ()
	{
		canHitTarget = true;
		Transform spriteTransform = spriteRenderer.transform;
		Vector3 localScale = spriteTransform.localScale;
		attackState = AttackState.Fly;
		switch (CalculateFlyDirection ()) {
		case FlyDirection.Left:
			spriteTransform.localScale = new Vector3 (localScale.x, Mathf.Abs (localScale.y) * -1f, localScale.z);
			flipSpeed = -1f;
			break;
		case FlyDirection.Right:
			spriteTransform.localScale = new Vector3 (localScale.x, Mathf.Abs (localScale.y), localScale.z);
			flipSpeed = 1f;
			break;
		}
	}

	FlyDirection CalculateFlyDirection ()
	{
		if (transform.localPosition.x > 0f) {
			return FlyDirection.Left;
		} else {
			return FlyDirection.Right;
		}
	}

	protected override void HitByBody (CharacterMovement characterMovement)
	{
		base.HitByBody (characterMovement);
		StartCoroutine (Knockback());
	}

	IEnumerator Knockback() {
		Collider2D collider = GetComponent<Collider2D> ();
		collider.isTrigger = false;
		yield return new WaitForSeconds(0.05f);
		collider.isTrigger = true;
	}

	protected override void StartDead ()
	{
		base.StartDead ();
		rbody2D.bodyType = RigidbodyType2D.Dynamic;
	}
}
