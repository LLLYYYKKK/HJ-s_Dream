using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour {
	bool isOpened = false;
	AudioSource audioSource;
	Animator animaotr;
	Rigidbody2D rbody2D;
	public AudioClip removeLockSound;
	public AudioClip openSound;
	public List<GameObject> dropItems;
	Transform itemDropPoint;

	void Awake() {
		isOpened = false;
		audioSource = GetComponent<AudioSource> ();
		animaotr = GetComponent<Animator> ();
		rbody2D = GetComponent<Rigidbody2D> ();
		itemDropPoint = transform.Find ("ItemDropPoint");
		// dropItems = new List<GameObject> ();
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (!isOpened) {
			if (other.gameObject.tag == "Player") {
				Open ();
				PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement> ();
				playerMovement.SetDestinatioTo (playerMovement.transform.position);
			}
		}
	}

	void Open() {
		isOpened = true;
		animaotr.SetTrigger ("RemoveLock");
		audioSource.PlayOneShot (removeLockSound);
	}

	public void OpenAction() {
		audioSource.PlayOneShot (openSound);
		rbody2D.drag = 1;
		rbody2D.mass = 5;

		if (dropItems.Count != 0) {
			foreach (var dropItem in dropItems) {
				GameObject InstantiatedItem = Instantiate (dropItem);
				InstantiatedItem.transform.position = itemDropPoint.position;
				InstantiatedItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.Range(-100, 100), Random.Range(100, 200)));
			}
		}
	}

	public bool IsOpened() {
		return isOpened;
	}
}
