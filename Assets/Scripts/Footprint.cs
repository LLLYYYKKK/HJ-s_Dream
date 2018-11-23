using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour {
	public float deadTime;
	SpriteRenderer spriteRenderer;
	float timer;

	void Awake() {
		timer = 0.0f;
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
	}

	void Update() {
		timer += Time.deltaTime;
		Color color = spriteRenderer.color;
		color.a -= Time.deltaTime / deadTime;
		spriteRenderer.color = color;
		if (timer >= deadTime) {
			Destroy (gameObject);
		}
	}
}
