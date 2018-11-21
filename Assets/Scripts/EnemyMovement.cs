using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement: CharacterMovement {
	public float playerFinderRange = 3.0f;

	protected override void Update() {
		base.Update ();
		if (attackTarget == null) {
			FindPlayer ();
		}
	}

	public override void SetDirectionTo(Vector2 destination) {
		this.destination = destination;
	}

	void FindPlayer() {
		Collider2D[] colliders2D = Physics2D.OverlapCircleAll (this.GetComponent<Collider2D>().transform.position, playerFinderRange);
		foreach (Collider2D otherCollider2D in colliders2D) {
			if (otherCollider2D.tag == "Player") {
				attackTarget = otherCollider2D.gameObject;
			}
		}
	}

	public override void Hit (float damage, CharacterMovement attacker)
	{
		base.Hit (damage, attacker);
		attackTarget = attacker.gameObject;
	}
}
