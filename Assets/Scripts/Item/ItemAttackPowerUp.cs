using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttackPowerUp : Item {
	public float powerUpAmount = 0.5f;

	public override void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		base.ApplyObtainEffect (playerMovement);
		playerMovement.AttackPowerUp (powerUpAmount);
	}
}
