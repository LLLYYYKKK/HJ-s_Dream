using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoomManager : RoomManager {
	TreasureBox treasureBox;
	protected override void Awake ()
	{
		base.Awake ();
		treasureBox = GetComponentInChildren<TreasureBox> ();
	}

	public void SetTreasure (GameObject treasure)
	{
		treasureBox.dropItems.Add (treasure);
	}
}
