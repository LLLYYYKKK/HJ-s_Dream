using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPlate : MonoBehaviour {
	public float damage = 2f;
	public float explosionTime = 0.2f;
	public float remainTime = 2f;
	public float canDamagedByPlateTime = 0.5f;
	public CharacterMovement creater;
	SpriteRenderer spriteRenderer;
	Collider2D collider;
	List<CharacterMovement> damagedCharacters;
	float timer;

	bool isExplosion;

	void Awake() {
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		this.collider = GetComponent<Collider2D> ();
		timer = 0f;
		transform.localScale = new Vector3 (0, 0, 1);
		damagedCharacters = new List<CharacterMovement> ();
		isExplosion = true;
	}

	void Update() {
		timer += Time.deltaTime;
		if (timer < explosionTime) {
			transform.localScale += new Vector3 (Time.deltaTime / explosionTime, Time.deltaTime / explosionTime, 0);
		} else {
			if (isExplosion) {
				this.collider.isTrigger = true;
				damagedCharacters.Clear ();
				isExplosion = false;
			}
			transform.localScale -= new Vector3 (Time.deltaTime / remainTime * 0.2f, Time.deltaTime / remainTime * 0.2f, 0);
			if (timer - explosionTime >= remainTime / 2f) {
				Color spriteColor = spriteRenderer.color;
				spriteColor.a -= Time.deltaTime / remainTime * 2f;
				spriteRenderer.color = spriteColor;
			}
		}
		if (timer >= explosionTime + remainTime) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (timer < explosionTime) {
			try {
				CharacterMovement characterMovement = other.gameObject.GetComponent<CharacterMovement> ();
				if (!damagedCharacters.Contains(characterMovement)) {
					characterMovement.Hit(damage, creater);
					damagedCharacters.Add(characterMovement);
				}
			}
			catch {
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (timer >= explosionTime) {
			try {
				if (!other.isTrigger) {
					CharacterMovement characterMovement = other.GetComponent<CharacterMovement>();
					StartCoroutine(HitCharacterByPlate(characterMovement));
				}
			}
			catch{
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		OnTriggerEnter2D (other);
	}

	IEnumerator HitCharacterByPlate (CharacterMovement characterMovement) {
		if (!damagedCharacters.Contains (characterMovement)) {
			characterMovement.Hit (damage, creater);
			damagedCharacters.Add (characterMovement);

			yield return new WaitForSeconds (canDamagedByPlateTime);
			damagedCharacters.Remove (characterMovement);
		}
	}
}
