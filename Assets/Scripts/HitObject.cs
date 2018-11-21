using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObject : MonoBehaviour {
	public float liveTime;
	public float speed;
	public bool isItTargeting;
	public bool isSetAttackTargetIfNotExist = true;
	public int maximumTargetCount;
	public GameObject hitEffect;

	Rigidbody2D rbody2D;

	List<GameObject> attackTargetsBeHit;

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
	void Update () {
		timer += Time.deltaTime;

		if (timer >= liveTime) {
			Destroy (gameObject);
		}

		if (isItTargeting && attackTarget != null) {
			destination = attackTarget.center.position;
			float angle = Mathf.Atan2 (destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);

			if (!attackTarget.isActive) {
				Destroy (gameObject);
			}
		}
	}

	void FixedUpdate() {
		rbody2D.velocity = speed * transform.right;
	}
		
	public void Hit(CharacterMovement attacker, Vector2 destination, string attackTargetTag) {
		this.attacker = attacker;
		this.attackTargetTag = attackTargetTag;
		if (attacker.attackTarget != null) {
			attackTarget = attacker.attackTarget.GetComponent<CharacterMovement> ();
		}
		this.destination = destination;
		float angle = Mathf.Atan2 (destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	void CheckHit (GameObject target)
	{
		if (!attackTargetsBeHit.Contains(target)) {
			if (attackTargetsBeHit.Count != maximumTargetCount) {
				attackTargetsBeHit.Add (target);
				CharacterMovement targetCharacterMovement = target.GetComponent<CharacterMovement> ();
				// attacker.attackTarget = target;
				targetCharacterMovement.Hit (damage, attacker);
				CreateHitEffect ();

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

	void CreateHitEffect ()
	{
		if (hitEffect != null) {
			GameObject InstantiatedhitEffect = Instantiate (hitEffect);
			InstantiatedhitEffect.transform.position = transform.position;
			InstantiatedhitEffect.transform.rotation = transform.rotation;
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
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

	void HitAction ()
	{
	}
}
