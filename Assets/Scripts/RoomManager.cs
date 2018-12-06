using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
	public enum Direction {North, South, East, West, None};
	public enum RoomType {Null, Base, Normal, Item, Champion, Boss, Hidden};
	public RoomType roomType;
	[SerializeField] Vector2 northDoorPoint;
	[SerializeField] Vector2 southDoorPoint;
	[SerializeField] Vector2 eastDoorPoint;
	[SerializeField] Vector2 westDoorPoint;
	Dictionary<Direction, Vector2> doorPointDictionary;
	DoorFactory doorFactory;

	public GameObject[] canSpawnEnemies;

	public GameObject[] normalEnemyDropItems {
		get;
		set;
	}

	public BoxCollider2D[] spawnAreas;
	public Vector2[] spawnPoints;
	public AudioClip doorOpenSound;
	public GameObject clearReward;
	[SerializeField] protected bool isCleared;

	public Dictionary<Direction, Door> doors;

	List<GameObject> instantiatedEnemies;

	AudioSource audioSource;
	UICanvas uiCanvas;

	protected virtual void Awake() {
		doorPointDictionary = new Dictionary<Direction, Vector2> ();
		doorPointDictionary.Add (Direction.North, northDoorPoint);
		doorPointDictionary.Add (Direction.South, southDoorPoint);
		doorPointDictionary.Add (Direction.East, eastDoorPoint);
		doorPointDictionary.Add (Direction.West, westDoorPoint);

		doorFactory = GetComponentInParent<DoorFactory> ();

		doors = new Dictionary<Direction, Door> ();
		instantiatedEnemies = new List<GameObject> ();
		audioSource = GetComponent<AudioSource> ();
		isCleared = false;
		spawnAreas = transform.Find("SpawnArea").GetComponents<BoxCollider2D> ();
		uiCanvas = GameObject.FindGameObjectWithTag ("UICanvas").GetComponent<UICanvas> ();
	}

	public void SpawnEnemies() {
		if (!isCleared) {
			if (instantiatedEnemies.Count == 0) {
				foreach (var point in spawnPoints) {
					Vector2 position = transform.position;
					position += point;
					GameObject enemy = Instantiate (canSpawnEnemies [Random.Range(0, canSpawnEnemies.Length)], position, Quaternion.identity, transform);
					if (enemy.tag == "EnemyAssociation") {
						EnemyMovement[] enemies = enemy.GetComponentsInChildren<EnemyMovement> ();
						foreach (var enemyMovement in enemies) {
							SetEnemy (enemyMovement);
							enemyMovement.transform.SetParent (transform);
						}
						Destroy (enemy);
					} else {
						EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement> ();
						SetEnemy (enemyMovement);
					}
				}

				if (instantiatedEnemies.Count == 0) {
					RoomClear ();
				}
			}
		}
	}

	public void SetEnemy (EnemyMovement enemyMovement)
	{
		instantiatedEnemies.Add (enemyMovement.gameObject);
		enemyMovement.roomManager = this;
		switch (enemyMovement.enemyGrade) {
		case EnemyMovement.Grade.Normal:
			enemyMovement.canDropItems = normalEnemyDropItems;
			break;
		}
	}

	public void EnemyDead (EnemyMovement enemyMovement)
	{
		instantiatedEnemies.Remove (enemyMovement.gameObject);

		if (instantiatedEnemies.Count == 0) {
			RoomClear ();
		}
	}

	void RoomClear ()
	{
		isCleared = true;
		audioSource.PlayOneShot (doorOpenSound);
		foreach (var keyValuePair in doors) {
			switch (keyValuePair.Key) {
			case Direction.North:
				transform.Find ("NorthCollider").gameObject.SetActive(false);
				break;
			case Direction.South:
				transform.Find("SouthCollider").gameObject.SetActive(false);
				break;
			case Direction.East:
				transform.Find("EastCollider").gameObject.SetActive(false);
				break;
			case Direction.West:
				transform.Find("WestCollider").gameObject.SetActive(false);
				break;
			}
			keyValuePair.Value.Open ();
		}

		if (clearReward != null) {
			Instantiate (clearReward, transform);
		}
	}

	public void Enter (CharacterMovement charMovement, Direction exitDoorDirection)
	{
		Door enterDoor = null;
		if (exitDoorDirection == Direction.North) {
			enterDoor = doors [Direction.South];
		} else if (exitDoorDirection == Direction.South) {
			enterDoor = doors [Direction.North];
		} else if (exitDoorDirection == Direction.East) {
			enterDoor = doors [Direction.West];
		} else if (exitDoorDirection == Direction.West) {
			enterDoor = doors [Direction.East];
		}

		gameObject.SetActive (true);
		SpawnEnemies ();
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);

		charMovement.transform.position = enterDoor.enterPoint;
		Debug.Log (enterDoor.enterPoint);
		charMovement.Stop ();

		uiCanvas.EnterRoom (enterDoor.direction);
	}

	public void Exit (Door door)
	{
		gameObject.SetActive (false);
		//uiCanvas.ExitRoom (this);
	}

	bool HasDoor (RoomManager.Direction direction)
	{
		return doors.ContainsKey (direction);
	}

	public void CreateDoor(Direction direction, RoomManager linkedRoom) {
		if (!HasDoor (direction)) {
			Door thisRoomDoor = null;
			switch(roomType) {
			case RoomType.Item:
				thisRoomDoor = doorFactory.CreateDoor (RoomType.Item, direction).GetComponent<Door> ();
				((KeyCardDoor)thisRoomDoor).UnLock ();
				break;
			default:
				thisRoomDoor = doorFactory.CreateDoor (linkedRoom.roomType, direction).GetComponent<Door> ();
				break;
			}
			thisRoomDoor.transform.SetParent (transform);
			thisRoomDoor.transform.localPosition = doorPointDictionary [direction];
			thisRoomDoor.roomManager = this;
			thisRoomDoor.linkedRoom = linkedRoom;
			doors.Add (direction, thisRoomDoor);
		}
	}
}
