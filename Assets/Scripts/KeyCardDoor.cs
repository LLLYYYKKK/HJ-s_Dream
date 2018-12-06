using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardDoor : Door {
	[SerializeField] bool isLocked = true;
	[SerializeField] bool isRoomCleared = false;
	public AudioSource unlockSound;
	public AudioSource openSound;

	AudioSource audioSource;

	protected override void Awake() {
		base.Awake();
		isLocked = true;
		isRoomCleared = false;
		audioSource = GetComponent<AudioSource> ();
	}

	public override void Open ()
	{
		if (!isLocked) {
			base.Open ();
		}
		isRoomCleared = true;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (isLocked) {
			if (other.gameObject.tag == "Player") {
				PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement> ();
				if (IsCanOpened (playerMovement)) {
					playerMovement.keyCardCount -= 1;
					UnLock ();

					if (isRoomCleared) {
						base.Open ();
					}
				}
			}
		}
	}

	bool IsCanOpened (PlayerMovement playerMovement)
	{
		if (playerMovement.keyCardCount > 0) {
			return true;
		}
		return false;
	}

	public void UnLock ()
	{
		isLocked = false;
		StartCoroutine (RemoveLock ());
	}

	IEnumerator RemoveLock() {
		yield return new WaitForSeconds (0.5f);
		Destroy (GetComponent<BoxCollider2D> ());
	}

	void OnEnable() {
		if (!isLocked) {
			Destroy (GetComponent<BoxCollider2D> ());
		}
	}
}
