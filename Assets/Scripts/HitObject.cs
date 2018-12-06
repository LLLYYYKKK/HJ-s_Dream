﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject : MonoBehaviour {
	public float liveTime;
	public float speed;
	public bool isItTargeting;
	public bool isSetAttackTargetIfNotExist = true;
	public int maximumTargetCount;
	public GameObject hitEffect;

	protected Rigidbody2D rbody2D;

	protected List<GameObject> attackTargetsBeHit;

	[System.NonSerialized] public string attackTargetTag;
	[System.NonSerialized] public float damage;
	[System.NonSerialized] public CharacterMovement attacker;

	float timer;
	Vector2 destination;

	CharacterMovement attackTarget;

	void Awake() {
		timer = 0.0f;
		destination = transform.position;
		rbody2D = GetComponent<Rigidbody2D> ();
		attackTargetTag = "Enemy";
		attackTargetsBeHit = new List<GameObject> ();
	}

	// Update is called once per frame
	protected virtual void Update () {
		timer += Time.deltaTime;

		if (timer >= liveTime) {
			Destroy (gameObject);
		}

		if (isItTargeting && attackTarget != null) {
			destination = attackTarget.center.position;
			float angle = Mathf.Atan2 (destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

			if (!attackTarget.isAlive) {
				Destroy (gameObject);
			}
		}
	}

	protected virtual void FixedUpdate() {
		rbody2D.velocity = speed * transform.right;
	}
		
	public virtual void Hit(CharacterMovement attacker, Vector2 destination, string attackTargetTag) {
		this.attacker = attacker;
		this.attackTargetTag = attackTargetTag;
		if (attacker.attackTarget != null) {
			attackTarget = attacker.attackTarget.GetComponent<CharacterMovement> ();
		}
		this.destination = destination;
		float angle = Mathf.Atan2 (destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	protected virtual void CheckHit (GameObject target)
	{
		if (!attackTargetsBeHit.Contains(target)) {
			if (attackTargetsBeHit.Count != maximumTargetCount) {
				attackTargetsBeHit.Add (target);
				CharacterMovement targetCharacterMovement = target.GetComponent<CharacterMovement> ();
				// attacker.attackTarget = target;
				HitAction(targetCharacterMovement);

				if (isSetAttackTargetIfNotExist) {
					if (attacker.attackTarget == null) {
						attacker.attackTarget = target;
					}
				}

				if (attackTargetsBeHit.Count == maximumTargetCount) {
					Destroy (gameObject);
				}
			}
		}
	}

	protected void CreateHitEffect ()
	{
		if (hitEffect != null) {
			GameObject InstantiatedhitEffect = Instantiate (hitEffect);
			InstantiatedhitEffect.transform.position = transform.position;
			InstantiatedhitEffect.transform.rotation = transform.rotation;
		}
	}

	protected void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == attackTargetTag) {
			if (isItTargeting) {
				if (other.gameObject == attackTarget.gameObject) {
					CheckHit (other.gameObject);
				}
			} else {
				CheckHit (other.gameObject);
			}
		}
	}

	protected virtual void HitAction (CharacterMovement targetCharacterMovement)
	{
		targetCharacterMovement.Hit (damage, attacker);
		CreateHitEffect ();
	}
}
