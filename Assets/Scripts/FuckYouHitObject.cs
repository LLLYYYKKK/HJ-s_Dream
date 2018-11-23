using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckYouHitObject : HitObject {
	protected override void Update ()
	{
		
	}

	protected override void FixedUpdate ()
	{
		
	}

	public override void Hit (CharacterMovement attacker, Vector2 destination, string attackTargetTag)
	{
		base.Hit (attacker, destination, attackTargetTag);
		transform.rotation = Quaternion.identity;
		Vector2 velocity = destination - new Vector2(transform.position.x, transform.position.y);
		velocity = velocity.normalized * speed;
		rbody2D.velocity = velocity;
	}


	protected override void HitAction (CharacterMovement targetCharacterMovement)
	{
		StartCoroutine (MultipleHit (targetCharacterMovement));
	}

	void OnTriggerStay2D(Collider2D other) {
		OnTriggerEnter2D (other);
	}

	IEnumerator MultipleHit(CharacterMovement target) {
		if (target.isActive) {
			target.Hit (damage, attacker);
			yield return new WaitForSeconds (0.1f);
			attackTargetsBeHit.Remove (target.gameObject);
		}
	}
}
