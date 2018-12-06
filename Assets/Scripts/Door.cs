using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	public RoomManager.Direction direction;

	public RoomManager linkedRoom;
	public Vector2 enterPoint {
		get {
			return transform.Find ("EnterPoint").position;
		}
	}

	[SerializeField] bool isOpened;

	protected Animator animator;
	public RoomManager roomManager;
	float recovertyTimer;

	protected virtual void Awake() {
		animator = GetComponent<Animator> ();
		isOpened = false;
	}

	protected virtual void Update() {
		animator.SetBool ("isOpened", isOpened);
	}

	public virtual void Open ()
	{
		animator.SetTrigger ("Open");
		isOpened = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (isOpened) {
			if (other.tag == "Player") {
				CharacterMovement playerMovement = other.GetComponent<CharacterMovement> ();
				linkedRoom.Enter (playerMovement, direction);
				roomManager.Exit (this);
			}
		}
	}
}
