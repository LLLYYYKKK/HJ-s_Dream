using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : NormalItem {
	protected override bool IsCanObtain (PlayerMovement playerMovement)
	{
		return playerMovement.coinCount < 100;
	}

	protected override void ObtainAction (PlayerMovement playerMovement)
	{
		base.ObtainAction (playerMovement);
		playerMovement.coinCount += 1;
	}
}
