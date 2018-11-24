using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
	public int numberOfRooms = 5;
	public int minNumberOfEnemyRange = 3;
	public int maxNumberOfEnemyRange = 10;

	public GameObject[] roomPrefabs;
	public GameObject[] canDropItems;
	public GameObject northDoor;
	public GameObject southDoor;
	public GameObject westDoor;
	public GameObject eastDoor;

	PlayerMovement playerMovement;

	void Awake() {
		playerMovement = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();

		RoomManager baseRoom = Instantiate (roomPrefabs [0], transform).GetComponent<RoomManager> ();
		baseRoom.spawnPoints = new Vector2[0];
		baseRoom.clearReward = null;

		RoomManager currentRoom = baseRoom;

		int roomCount = 1;
		while (true) {
			GameObject roomPrefab = roomPrefabs [Random.Range (0, roomPrefabs.Length)];

			int directionRoll = Random.Range (0, 4);

			switch (directionRoll) {
			case (int) RoomManager.Direction.North:
				if (!currentRoom.nextRooms.ContainsKey (RoomManager.Direction.North)) {
					RoomManager nextRoom = Instantiate (roomPrefab, transform).GetComponent<RoomManager> ();
					nextRoom.transform.position = currentRoom.transform.position + new Vector3 (0f, 20f, 0f);
					SettingRoom (nextRoom);

					currentRoom.nextRooms.Add (RoomManager.Direction.North, nextRoom);
					nextRoom.nextRooms.Add (RoomManager.Direction.South, currentRoom);

					Door northDoor = Instantiate (this.northDoor, currentRoom.transform).GetComponent<Door> ();
					Door southDoor = Instantiate (this.southDoor, nextRoom.transform).GetComponent<Door> ();
					northDoor.linkedDoor = southDoor;
					southDoor.linkedDoor = northDoor;

					nextRoom.gameObject.SetActive (false);

					roomCount++;
				} else {
					currentRoom = currentRoom.nextRooms [RoomManager.Direction.North];
				}
				break;

			case (int) RoomManager.Direction.South:
				if (!currentRoom.nextRooms.ContainsKey (RoomManager.Direction.South)) {
					RoomManager nextRoom = Instantiate (roomPrefab, transform).GetComponent<RoomManager> ();
					nextRoom.transform.position = currentRoom.transform.position + new Vector3 (0f, -20f, 0f);
					SettingRoom (nextRoom);

					currentRoom.nextRooms.Add (RoomManager.Direction.South, nextRoom);
					nextRoom.nextRooms.Add (RoomManager.Direction.North, currentRoom);

					Door northDoor = Instantiate (this.northDoor, nextRoom.transform).GetComponent<Door> ();
					Door southDoor = Instantiate (this.southDoor, currentRoom.transform).GetComponent<Door> ();
					northDoor.linkedDoor = southDoor;
					southDoor.linkedDoor = northDoor;

					nextRoom.gameObject.SetActive (false);

					roomCount++;
				} else {
					currentRoom = currentRoom.nextRooms [RoomManager.Direction.South];
				}
				break;

			case (int) RoomManager.Direction.West:
				if (!currentRoom.nextRooms.ContainsKey (RoomManager.Direction.West)) {
					RoomManager nextRoom = Instantiate (roomPrefab, transform).GetComponent<RoomManager> ();
					nextRoom.transform.position = currentRoom.transform.position + new Vector3 (-30f, 0f, 0f);
					SettingRoom (nextRoom);

					currentRoom.nextRooms.Add (RoomManager.Direction.West, nextRoom);
					nextRoom.nextRooms.Add (RoomManager.Direction.East, currentRoom);

					Door westDoor = Instantiate (this.westDoor, currentRoom.transform).GetComponent<Door> ();
					Door eastDoor = Instantiate (this.eastDoor, nextRoom.transform).GetComponent<Door> ();
					westDoor.linkedDoor = eastDoor;
					eastDoor.linkedDoor = westDoor;

					nextRoom.gameObject.SetActive (false);

					roomCount++;
				} else {
					currentRoom = currentRoom.nextRooms [RoomManager.Direction.West];
				}
				break;

			case (int) RoomManager.Direction.East:
				if (!currentRoom.nextRooms.ContainsKey (RoomManager.Direction.East)) {
					RoomManager nextRoom = Instantiate (roomPrefab, transform).GetComponent<RoomManager> ();
					nextRoom.transform.position = currentRoom.transform.position + new Vector3 (30f, 0f, 0f);
					SettingRoom (nextRoom);

					currentRoom.nextRooms.Add (RoomManager.Direction.East, nextRoom);
					nextRoom.nextRooms.Add (RoomManager.Direction.West, currentRoom);

					Door westDoor = Instantiate (this.westDoor, nextRoom.transform).GetComponent<Door> ();
					Door eastDoor = Instantiate (this.eastDoor, currentRoom.transform).GetComponent<Door> ();
					westDoor.linkedDoor = eastDoor;
					eastDoor.linkedDoor = westDoor;

					nextRoom.gameObject.SetActive (false);

					roomCount++;
				} else {
					currentRoom = currentRoom.nextRooms [RoomManager.Direction.East];
				}
				break;
			}
			if (roomCount == numberOfRooms) {
				break;
			}
		}

		baseRoom.SpawnEnemies ();
	}

	void SettingRoom (RoomManager room)
	{
		int minNumberOfEnemy = Random.Range (0, minNumberOfEnemyRange);
		int maxNumberOfEnemy = Random.Range (minNumberOfEnemyRange, maxNumberOfEnemyRange);
		int numberOfEnemy = Random.Range (minNumberOfEnemy, maxNumberOfEnemy);

		GameObject clearReward = null;
		float itemRoll = Random.Range (0f, 1f);
		if (itemRoll <= playerMovement.luck + (numberOfEnemy / maxNumberOfEnemyRange) * 0.3) {
			clearReward = canDropItems [Random.Range (0, canDropItems.Length)];
		}

		Vector2[] spawnPoints = new Vector2[numberOfEnemy];
		for (int i = 0; i < numberOfEnemy; i++) {
			spawnPoints [i] = new Vector2 (Random.Range (room.minSpawnRange.x, room.maxSpawnRange.x), Random.Range (room.minSpawnRange.y, room.maxSpawnRange.y));
		}

		room.spawnPoints = spawnPoints;
		room.clearReward = clearReward;
	}
}
