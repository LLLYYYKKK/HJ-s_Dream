using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerEnemyMovement : EnemyMovement {
	public float hitObjectSpeed = 6f;
	protected override void Update ()
	{
		base.Update ();
		Debug.Log (attackTarget);
		Debug.Log (canMove);
	}

	protected override void HitTargetAction ()
	{
		audioSource.PlayOneShot (attackSound);

		LinearMoveHitObject instantiatedHitObject = Instantiate (hitObject).GetComponent<LinearMoveHitObject> ();
		instantiatedHitObject.transform.position = hitObjectSpawnPoint.transform.position;
		instantiatedHitObject.damage = GetAttackPower ();
		instantiatedHitObject.range = GetAttackRange ();
		instantiatedHitObject.speed = hitObjectSpeed;
		instantiatedHitObject.Hit (this, attackTarget.transform.Find ("Center").position, attackTarget.tag);
	}
}
