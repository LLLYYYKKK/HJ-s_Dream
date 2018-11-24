using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
	public enum Direction {North, South, East, West};

	public GameObject[] canSpawnEnemies;
	public BoxCollider2D[] spawnAreas;
	public Vector2[] spawnPoints;
	public AudioClip doorOpenSound;
	public Door[] doors;
	public GameObject clearReward;
	bool isCleared;

	public Dictionary<Direction, RoomManager> nextRooms;

	List<GameObject> instantiatedEnemies;

	AudioSource audioSource;

	void Awake() {
		nextRooms = new Dictionary<Direction, RoomManager> ();
		instantiatedEnemies = new List<GameObject> ();
		audioSource = GetComponent<AudioSource> ();
		isCleared = false;
		spawnAreas = transform.Find("SpawnArea").GetComponents<BoxCollider2D> ();
	}

	public void SpawnEnemies() {
		doors = GetComponentsInChildren<Door> ();

		if (!isCleared) {
			if (instantiatedEnemies.Count == 0) {
				foreach (var point in spawnPoints) {
					GameObject enemy = Instantiate (canSpawnEnemies [0], transform);
					enemy.transform.localPosition = point;
					enemy.GetComponent<EnemyMovement> ().SetDirectionTo (enemy.transform.position);
					instantiatedEnemies.Add (enemy);
				}

				if (instantiatedEnemies.Count == 0) {
					RoomClear ();
				}
			}
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
		foreach (var door in doors) {
			door.Open ();
		}

		if (clearReward != null) {
			Instantiate (clearReward, transform);
		}
	}
}
