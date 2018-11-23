using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
	public AudioClip obtainItemSound;

	PlayerMovement playerMovement;
	List<Item> obtainedItems;
	AudioSource audioSource;

	void Awake() {
		playerMovement = GetComponent<PlayerMovement> ();
		obtainedItems = new List<Item> ();
		audioSource = GetComponent<AudioSource> ();
	}

	public void ObtainItem(Item item) {
		obtainedItems.Add (item);
		item.ApplyObtainEffect (playerMovement);
		audioSource.PlayOneShot (obtainItemSound);
	}
}
