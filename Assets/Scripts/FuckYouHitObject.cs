using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckYouHitObject : HitObject {
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
