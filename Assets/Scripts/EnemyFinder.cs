using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFinder : MonoBehaviour {
	CharacterMovement characterMovement;
	CircleCollider2D attackRangeTrigger;

	void Awake() {
		characterMovement = GetComponentInParent<CharacterMovement> ();
		attackRangeTrigger = GetComponent<CircleCollider2D> ();
	}

	void Update() {
		attackRangeTrigger.radius = characterMovement.attackRange;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Enemy") {
			
		}
	}
}
