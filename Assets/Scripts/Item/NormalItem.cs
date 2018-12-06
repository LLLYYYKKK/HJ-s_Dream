using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : Item {
	public AudioClip obtainSound;
	AudioSource audioSource;
	bool isObtained;
	float deadTimer;

	void Awake() {
		audioSource = GetComponent<AudioSource> ();
		isObtained = false;
		deadTimer = 0.0f;
	}

	protected override void Update() {
		base.Update ();
		if (isObtained) {
			deadTimer += Time.deltaTime;
			if (deadTimer >= 1f) {
				Destroy (gameObject);
			}
		}
	}

	protected override void ObtainItem (PlayerMovement playerMovement)
	{
		if (IsCanObtain(playerMovement)) {
			ObtainAction (playerMovement);
		}
	}

	protected virtual bool IsCanObtain (PlayerMovement playerMovement)
	{
		return true;
	}

	protected virtual void ObtainAction (PlayerMovement playerMovement)
	{
		isObtained = true;
		audioSource.PlayOneShot (obtainSound);
		Destroy (GetComponent<Collider2D> ());
		Destroy (GetComponent<SpriteRenderer> ());
	}
}
