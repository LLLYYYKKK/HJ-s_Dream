using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGlasses : Item {
	public override void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		playerMovement.attackRange += 1;
	}
}
