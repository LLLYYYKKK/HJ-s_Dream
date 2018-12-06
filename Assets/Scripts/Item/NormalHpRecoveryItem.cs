using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalHpRecoveryItem : NormalItem {
	public float recoveryAmount = 0.5f;

	protected override bool IsCanObtain (PlayerMovement playerMovement)
	{
		return playerMovement.hp != playerMovement.maxHp;
	}

	protected override void ObtainAction (PlayerMovement playerMovement)
	{
		base.ObtainAction (playerMovement);
		playerMovement.Hit (-recoveryAmount, null);
	}
}
