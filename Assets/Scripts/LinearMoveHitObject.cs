using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMoveHitObject : HitObject {
	public float range = 5f;
	Vector2 spawnPoint;

	protected override void Update ()
	{
		
	}

	protected override void FixedUpdate ()
	{
		if (spawnPoint != null) {
			float distance = Vector2.Distance (transform.position, spawnPoint);
			if (distance >= range) {
				ReachEnd ();
			}
		}
	}

	public override void Hit (CharacterMovement attacker, Vector2 destination, string attackTargetTag)
	{
		base.Hit (attacker, destination, attackTargetTag);
		spawnPoint = transform.position;
		Vector2 velocity = destination - new Vector2(transform.position.x, transform.position.y);
		velocity = velocity.normalized * speed;
		rbody2D.velocity = velocity;
	}

	protected virtual void ReachEnd ()
	{
		Destroy (gameObject);
	}
}
