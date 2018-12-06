using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalKeyCardItem : NormalItem {
	public int count = 1;

	protected override bool IsCanObtain (PlayerMovement playerMovement)
	{
		return playerMovement.keyCardCount < 100;
	}

	protected override void ObtainAction (PlayerMovement playerMovement)
	{
		base.ObtainAction (playerMovement);
		playerMovement.keyCardCount += count;
	}
}
