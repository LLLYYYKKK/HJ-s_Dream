using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
	public GameObject[] canSpawnEnemies;
	public Vector2[] spawnPoints;
	public AudioClip doorOpenSound;
	public Door[] doors;
	public GameObject clearReward;

	List<GameObject> instantiatedEnemies;

	AudioSource audioSource;

	void Awake() {
		instantiatedEnemies = new List<GameObject> ();
		audioSource = GetComponent<AudioSource> ();
		doors = GetComponentsInChildren<Door> ();

		foreach (var point in spawnPoints) {
			GameObject enemy = Instantiate (canSpawnEnemies [0], transform);
			enemy.transform.localPosition = point;
			enemy.GetComponent<EnemyMovement> ().SetDirectionTo (enemy.transform.position);
			instantiatedEnemies.Add (enemy);
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
		audioSource.PlayOneShot (doorOpenSound);
		foreach (var door in doors) {
			door.Open ();
		}

		if (clearReward != null) {
			Instantiate (clearReward, transform);
		}
	}
}
