using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttackTimeReduction : Item {
	public float reductionRate = 0.2f;

	public override void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		base.ApplyObtainEffect (playerMovement);
		playerMovement.AttackTimeReduction (reductionRate);
	}
}
