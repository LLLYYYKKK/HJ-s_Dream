using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitBombHitObject : AccelerationHitObject {
	public GameObject shitBombPlateObject;

	protected override void HitAction (CharacterMovement targetCharacterMovement)
	{
		ReachEnd ();
	}

	protected override void ReachEnd ()
	{
		base.ReachEnd ();
		ExplosionPlate shitBombPlate = Instantiate (shitBombPlateObject, transform.position, Quaternion.identity).GetComponent<ExplosionPlate> ();
		shitBombPlate.damage = damage;
		shitBombPlate.creater = attacker;
	}
}
