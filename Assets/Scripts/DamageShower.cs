using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageShower : MonoBehaviour {
	float deadTime = 1.0f;
	float deadTimer;
	Text damageText;
	GameObject movementObject;

	void Awake() {
		damageText = gameObject.AddComponent<Text> ();
		movementObject = new GameObject ("DamageShowerMovementObject");
		movementObject.AddComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		deadTimer += Time.deltaTime;
		Color damageTextColor = damageText.color;
		damageTextColor.a -= Time.deltaTime / deadTime;
		damageText.color = damageTextColor;
		if (deadTimer >= deadTime) {
			Destroy (movementObject);
			Destroy (gameObject);
		}
		damageText.rectTransform.position = Camera.main.WorldToScreenPoint (movementObject.transform.position);
	}

	public void ShowDamage(GameObject target, float damage) {
		movementObject.transform.position = target.transform.Find ("HitObjectSpawnPoint").position;
		float moveForceX = Random.Range (50, 80);
		float moveForceY = Random.Range(80, 120);
		if (target.GetComponent<CharacterMovement> ().Direction == 3) {
			moveForceX *= -1;
		}
		movementObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (moveForceX, moveForceY));

		Font arialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
		damageText.font = arialFont;
		damageText.material = arialFont.material;
		if (damage < 0) {
			damageText.color = new Color (0, 1, 0);
			damageText.fontSize = 25;
			damageText.text = "+" + (-damage).ToString();
		} else {
			damageText.color = new Color (255, 255, 255);
			damageText.fontSize = 18;
			damageText.text = damage.ToString ();
		}
		damageText.alignment = TextAnchor.UpperCenter;
		gameObject.AddComponent<Outline> ();
	}
}
