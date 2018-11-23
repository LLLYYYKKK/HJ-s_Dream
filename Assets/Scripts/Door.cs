using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	public Door linkedDoor;
	Vector2 enterPoint;

	BoxCollider2D doorCollider;
	CapsuleCollider2D doorEnterTrigger;

	void Awake() {
		doorCollider = GetComponent<BoxCollider2D> ();
		doorEnterTrigger = GetComponent<CapsuleCollider2D> ();
		enterPoint = transform.Find ("EnterPoint").position;
	}

	public void Open ()
	{
		Destroy (doorCollider);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			CharacterMovement playerMovement = other.GetComponent<CharacterMovement> ();
			linkedDoor.Enter (playerMovement);
			transform.parent.gameObject.SetActive (false);
		}
	}

	public void Enter (CharacterMovement charMovement)
	{
		transform.parent.gameObject.SetActive (true);
		charMovement.Stop ();
		charMovement.transform.position = enterPoint;
		Camera.main.transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, Camera.main.transform.position.z);
	}
}
