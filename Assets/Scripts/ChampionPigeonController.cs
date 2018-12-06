using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionPigeonController : MonoBehaviour {
	GameObject player;
	ChampionPigeon pigeon;
	BoxCollider2D[] changePositionTrigger;
	TreasureBox trapBox;
	RoomManager roomManager;
	bool playerTrapped;

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
		pigeon = GetComponentInChildren<ChampionPigeon> ();
		pigeon.attackTarget = player;
		changePositionTrigger = GetComponents<BoxCollider2D> ();
		pigeon.transform.localPosition = new Vector2(changePositionTrigger [Random.Range (0, changePositionTrigger.Length)].offset.x, 0f);
		trapBox = GetComponentInChildren<TreasureBox> ();
		playerTrapped = false;
	}

	void Start() {
		roomManager = GetComponent<RoomManager> ();
		roomManager.SetEnemy (pigeon);
	}
		

	void Update() {
		if (!playerTrapped) {
			if (trapBox.IsOpened ()) {
				playerTrapped = true;
				StartFly ();
			}
		}
	}

	void StartFly ()
	{
		pigeon.transform.position = new Vector2 (pigeon.transform.position.x, player.transform.position.y);
		pigeon.transform.position -= pigeon.center.localPosition;
		pigeon.Fly ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (playerTrapped) {
			if (other.gameObject == pigeon.gameObject) {
				StartFly ();
			}
		}
	}
}
