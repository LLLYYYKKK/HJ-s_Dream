using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorFactory : MonoBehaviour {
	[SerializeField] GameObject normalNorthDoor;
	[SerializeField] GameObject normalSouthDoor;
	[SerializeField] GameObject normalEastDoor;
	[SerializeField] GameObject normalWestDoor;
	[SerializeField] GameObject itemNorthDoor;
	[SerializeField] GameObject itemSouthDoor;
	[SerializeField] GameObject itemEastDoor;
	[SerializeField] GameObject itemWestDoor;

	public GameObject CreateDoor(RoomManager.RoomType roomType, RoomManager.Direction direction) {
		switch (roomType) {
		case RoomManager.RoomType.Item:
			switch (direction) {
			case RoomManager.Direction.North:
				return Instantiate (itemNorthDoor);
			case RoomManager.Direction.South:
				return Instantiate (itemSouthDoor);
			case RoomManager.Direction.East:
				return Instantiate (itemEastDoor);
			case RoomManager.Direction.West:
				return Instantiate (itemWestDoor);

			}
			break;
		default:
			switch (direction) {
			case RoomManager.Direction.North:
				return Instantiate (normalNorthDoor);
			case RoomManager.Direction.South:
				return Instantiate (normalSouthDoor);
			case RoomManager.Direction.East:
				return Instantiate (normalEastDoor);
			case RoomManager.Direction.West:
				return Instantiate (normalWestDoor);

			}
			break;
		}
		return null;
	}
}
