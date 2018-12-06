using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
	Dictionary<RoomManager, Vector2Int> roomDictionary;

	public int stageSize = 7;
	public int numberOfNormalRooms = 5;
	public int minNumberOfEnemyRange = 3;
	public int maxNumberOfEnemyRange = 10;
	int[,] roomMatrix;

	public GameObject[] normalRoomPrefabs;
	public GameObject[] itemRoomPrefabs;
	public GameObject[] championRoomPrefabs;

	public List<GameObject> clearRewards;
	public GameObject[] normalEnemyDropItems;
	public List<GameObject> itemRoomTreasures;

	PlayerMovement playerMovement;
	UICanvas uiCanvas;

	Vector2Int baseRoomPoint;
	RoomManager baseRoom;

	void Awake() {
		playerMovement = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();
		uiCanvas = GameObject.FindGameObjectWithTag ("UICanvas").GetComponent<UICanvas> ();

		roomDictionary = new Dictionary<RoomManager, Vector2Int> ();
		roomMatrix = new int[stageSize, stageSize];
		CreateNormalRooms ();
		CreateItemRoom ();
		CreateChampionRoom ();
		CreateDoors ();

		uiCanvas.CreateMap (roomMatrix, stageSize);

		baseRoom.SpawnEnemies ();
		TurnOffOtherRooms ();

		string debug = "";
		for (int i = 0; i < stageSize; i++) {
			for (int j = 0; j < stageSize; j++) {
				debug += roomMatrix [i, j].ToString () + " ";
			}
			debug += "\n";
		}

		Debug.Log (debug);
	}

	void CreateNormalRooms ()
	{
		baseRoomPoint = new Vector2Int (Random.Range (0, stageSize), Random.Range (0, stageSize));
		SettingBaseRoom (baseRoomPoint);
		Vector2Int currentPoint = baseRoomPoint;
		int normalRoomCount = 0;

		while (true) {
			if (normalRoomCount == numberOfNormalRooms) {
				break;
			}

			int roll = Random.Range (0, 4);
			Vector2Int nextRoomPoint = currentPoint;

			switch (roll) {
			case (int) RoomManager.Direction.North:
				nextRoomPoint = currentPoint + new Vector2Int (-1, 0);
				break;
			case (int) RoomManager.Direction.South:
				nextRoomPoint = currentPoint + new Vector2Int (1, 0);
				break;
			case (int) RoomManager.Direction.East:
				nextRoomPoint = currentPoint + new Vector2Int (0, 1);
				break;
			case (int) RoomManager.Direction.West:
				nextRoomPoint = currentPoint + new Vector2Int (0, -1);
				break;
			}

			if (IsNotOverIndex (nextRoomPoint)) {
				currentPoint = nextRoomPoint;
				if (IsAnotherRoomExist (nextRoomPoint)) {
				} else {
					SettingNormalRoom (nextRoomPoint);
					normalRoomCount++;
				}
			}
		}
	}

	List<Vector2Int> FindCanHasSpecialRoomPoints ()
	{
		List<Vector2Int> pointsCanHasSpecialRoom = new List<Vector2Int> ();
		for (int i = 0; i < stageSize; i++) {
			for (int j = 0; j < stageSize; j++) {
				if (roomMatrix [i, j] == (int)RoomManager.RoomType.Null) {
					int nullRoomCount = 0;
					Vector2Int northPoint = new Vector2Int (i - 1, j);
					if (northPoint.x == -1) {
						nullRoomCount++;
					} else if (roomMatrix [northPoint.x, northPoint.y] == (int)RoomManager.RoomType.Null) {
						nullRoomCount++;
					} else if (roomMatrix [northPoint.x, northPoint.y] > (int)RoomManager.RoomType.Normal) {
						continue;
					}
					Vector2Int southPoint = new Vector2Int (i + 1, j);
					if (southPoint.x == stageSize) {
						nullRoomCount++;
					} else if (roomMatrix [southPoint.x, southPoint.y] == (int)RoomManager.RoomType.Null) {
						nullRoomCount++;
					} else if (roomMatrix [southPoint.x, southPoint.y] > (int)RoomManager.RoomType.Normal) {
						continue;
					}
					Vector2Int eastPoint = new Vector2Int (i, j + 1);
					if (eastPoint.y == stageSize) {
						nullRoomCount++;
					} else if (roomMatrix [eastPoint.x, eastPoint.y] == (int)RoomManager.RoomType.Null) {
						nullRoomCount++;
					} else if (roomMatrix [eastPoint.x, eastPoint.y] > (int)RoomManager.RoomType.Normal) {
						continue;
					}
					Vector2Int westPoint = new Vector2Int (i, j - 1);
					if (westPoint.y == -1) {
						nullRoomCount++;
					} else if (roomMatrix [westPoint.x, westPoint.y] == (int)RoomManager.RoomType.Null) {
						nullRoomCount++;
					} else if (roomMatrix [westPoint.x, westPoint.y] > (int)RoomManager.RoomType.Normal) {
						continue;
					}

					if (nullRoomCount == 3) {
						Vector2Int candidiatePoint = new Vector2Int (i, j);
						pointsCanHasSpecialRoom.Add (candidiatePoint);
					}
				}
			}
		}

		return pointsCanHasSpecialRoom;
	}

	void CreateItemRoom ()
	{
		List<Vector2Int> pointsCanHasItemRoom = FindCanHasSpecialRoomPoints ();
		Vector2Int itemRoomPoint = pointsCanHasItemRoom [Random.Range (0, pointsCanHasItemRoom.Count)];
		SettingItemRoom (itemRoomPoint);
		pointsCanHasItemRoom.Clear ();
	}

	void CreateChampionRoom() {
		List<Vector2Int> pointsCanHasChampionRoom = FindCanHasSpecialRoomPoints ();
		Vector2Int championRoomPoint = pointsCanHasChampionRoom [Random.Range (0, pointsCanHasChampionRoom.Count)];
		SettingChampionRoom (championRoomPoint);
		pointsCanHasChampionRoom.Clear ();
	}

	void CreateDoors (){
		foreach (var currentRoomPair in roomDictionary) {
			foreach (var roomPairToCompare in roomDictionary) {
				Vector2 pointDiffrence = roomPairToCompare.Value - currentRoomPair.Value;
				RoomManager.Direction doorDirection = RoomManager.Direction.None;
				if (pointDiffrence.x == -1 && pointDiffrence.y == 0) {
					//North
					doorDirection = RoomManager.Direction.North;

				} else if (pointDiffrence.x == 1 && pointDiffrence.y == 0) {
					//South
					doorDirection = RoomManager.Direction.South;
				} else if (pointDiffrence.x == 0 && pointDiffrence.y == 1) {
					//East
					doorDirection = RoomManager.Direction.East;
				}else if (pointDiffrence.x == 0 && pointDiffrence.y == -1) {
					//West
					doorDirection = RoomManager.Direction.West;
				}

				if (doorDirection != RoomManager.Direction.None) {
					currentRoomPair.Key.CreateDoor (doorDirection, roomPairToCompare.Key); 
				}
			}
		}
	}

	void TurnOffOtherRooms ()
	{
		foreach (var keyValuePair in roomDictionary) {
			if (keyValuePair.Key != baseRoom) {
				keyValuePair.Key.gameObject.SetActive (false);
			}
		}
	}

	bool IsNotOverIndex (Vector2Int nextRoomPoint)
	{
		if (0 <= nextRoomPoint.x && nextRoomPoint.x < stageSize) {
			if (0 <= nextRoomPoint.y && nextRoomPoint.y < stageSize) {
				return true;
			}
		}

		return false; 
	}

	bool IsAnotherRoomExist (Vector2Int nextRoomPoint)
	{
		if (roomMatrix [nextRoomPoint.x, nextRoomPoint.y] != (int)RoomManager.RoomType.Null) {
			return true;
		}
		return false;
	}
	
	protected virtual void SettingBaseRoom (Vector2Int basePoint)
	{
		roomMatrix [basePoint.x, basePoint.y] = (int) RoomManager.RoomType.Base;
		baseRoom = Instantiate (normalRoomPrefabs [0], transform).GetComponent<RoomManager>();
		baseRoom.spawnPoints = new Vector2[0];
		baseRoom.clearReward = null;
		baseRoom.roomType = RoomManager.RoomType.Base;
		roomDictionary.Add (baseRoom, basePoint);
	}

	protected virtual void SettingNormalRoom (Vector2Int nextRoomPoint)
	{
		roomMatrix [nextRoomPoint.x, nextRoomPoint.y] = (int) RoomManager.RoomType.Normal;
		RoomManager normalRoom = Instantiate (normalRoomPrefabs [Random.Range (0, normalRoomPrefabs.Length)], transform).GetComponent<RoomManager> ();
		roomDictionary.Add (normalRoom, nextRoomPoint);
		Vector2 localPosition = CalculateLocalPosition (nextRoomPoint);
		normalRoom.transform.localPosition = localPosition;
		normalRoom.roomType = RoomManager.RoomType.Normal;

		int minNumberOfEnemy = Random.Range (0, minNumberOfEnemyRange);
		int maxNumberOfEnemy = Random.Range (minNumberOfEnemyRange, maxNumberOfEnemyRange);
		int numberOfEnemy = Random.Range (minNumberOfEnemy, maxNumberOfEnemy);

		GameObject clearReward = null;
		float itemDice = Random.Range (0f, 1f);
		if (itemDice <= playerMovement.luck + (numberOfEnemy / maxNumberOfEnemyRange) * 0.3) {
			if (clearRewards.Count != 0) {
				clearReward = clearRewards [Random.Range (0, clearRewards.Count)];
				clearRewards.Remove (clearReward);
			}
		}
			
		normalRoom.spawnPoints = GetSpawnPoints(normalRoom.spawnAreas, numberOfEnemy);
		normalRoom.normalEnemyDropItems = normalEnemyDropItems;
		normalRoom.clearReward = clearReward;
	}

	void SettingItemRoom (Vector2Int roomPoint)
	{
		roomMatrix [roomPoint.x, roomPoint.y] = (int)RoomManager.RoomType.Item;
		ItemRoomManager itemRoom = Instantiate (itemRoomPrefabs [Random.Range (0, itemRoomPrefabs.Length)], transform).GetComponent<ItemRoomManager> ();
		roomDictionary.Add (itemRoom, roomPoint);
		Vector2 localPosition = CalculateLocalPosition (roomPoint);
		itemRoom.transform.localPosition = localPosition;
		itemRoom.roomType = RoomManager.RoomType.Item;
		GameObject treasure = itemRoomTreasures[Random.Range(0, itemRoomTreasures.Count)];
		itemRoomTreasures.Remove (treasure);

		itemRoom.SetTreasure (treasure);
	}

	void SettingChampionRoom (Vector2Int roomPoint)
	{
		roomMatrix [roomPoint.x, roomPoint.y] = (int)RoomManager.RoomType.Champion;
		RoomManager championRoom = Instantiate (championRoomPrefabs [Random.Range (0, championRoomPrefabs.Length)], transform).GetComponent<RoomManager> ();
		roomDictionary.Add (championRoom, roomPoint);
		Vector2 localPosition = CalculateLocalPosition (roomPoint);
		championRoom.transform.localPosition = localPosition;
		championRoom.roomType = RoomManager.RoomType.Champion;
		GameObject clearReward = itemRoomTreasures [Random.Range (0, itemRoomTreasures.Count)];
		championRoom.clearReward = clearReward;
		itemRoomTreasures.Remove (clearReward);

		int numberOfNormalEnemy = Random.Range (1, minNumberOfEnemyRange);
		championRoom.spawnPoints = GetSpawnPoints (championRoom.spawnAreas, numberOfNormalEnemy);
		championRoom.normalEnemyDropItems = normalEnemyDropItems;
	}

	Vector2 CalculateLocalPosition (Vector2Int roomPoint)
	{
		Vector2 localPosition = (baseRoomPoint - roomPoint);
		localPosition = new Vector2 (-localPosition.y * 40f, localPosition.x * 30f);

		return localPosition;
	}

	Vector2[] GetSpawnPoints (BoxCollider2D[] spawnAreas, int numberOfEnemy)
	{
		Vector2[] spawnPoints = new Vector2[numberOfEnemy];
		for (int i = 0; i < numberOfEnemy; i++) {
			BoxCollider2D spawnArea = spawnAreas[Random.Range(0, spawnAreas.Length)];
			spawnPoints [i] = spawnArea.offset + new Vector2(Random.Range(-spawnArea.size.x, spawnArea.size.x) / 2f, Random.Range(-spawnArea.size.y, spawnArea.size.y) / 2f);
		}

		return spawnPoints;
	}
}
