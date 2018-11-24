using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	public Door linkedDoor;
	Vector2 enterPoint;

	bool isOpened;

	BoxCollider2D doorCollider;
	Animator animator;

	void Awake() {
		doorCollider = GetComponent<BoxCollider2D> ();
		animator = GetComponent<Animator> ();
		enterPoint = transform.Find ("EnterPoint").position;
		isOpened = false;
	}

	void Start() {
		animator.SetTrigger ("Close");
	}

	public void Open ()
	{
		Destroy (doorCollider);
		animator.SetTrigger ("Open");
		isOpened = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (isOpened) {
			if (other.tag == "Player") {
				CharacterMovement playerMovement = other.GetComponent<CharacterMovement> ();
				linkedDoor.Enter (playerMovement);
				transform.parent.gameObject.SetActive (false);
			}
		}
	}

	public void Enter (CharacterMovement charMovement)
	{
		transform.parent.gameObject.SetActive (true);
		transform.parent.GetComponent<RoomManager> ().SpawnEnemies ();
		charMovement.Stop ();
		charMovement.transform.position = enterPoint;
		Camera.main.transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, Camera.main.transform.position.z);
	}
}
