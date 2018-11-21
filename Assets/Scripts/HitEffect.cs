using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour {
	public float deadTime = 0.5f;
	float timer;
	SpriteRenderer spriteRenderer;
	Animator animator;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		animator.speed = 1f / deadTime;
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= deadTime) {
			Destroy (gameObject);
		}
	}
}
