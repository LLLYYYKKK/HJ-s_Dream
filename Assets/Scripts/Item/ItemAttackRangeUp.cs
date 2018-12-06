using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttackRangeUp : Item {
	public float plusAttackRanage = 0.3f;

	public override void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		base.ApplyObtainEffect (playerMovement);
		playerMovement.AttackRangeUp (plusAttackRanage);
	}
}
