using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement: CharacterMovement {
	public float playerFinderRange = 3.0f;
	PlayerMovement player;
	RoomManager roomManager;

	protected override void Awake ()
	{
		base.Awake ();
		roomManager = GetComponentInParent<RoomManager> ();
		player = GameObject.FindWithTag ("Player").GetComponent<PlayerMovement> ();;
	}

	protected override void Update() {
		base.Update ();
		spriteRenderer.sortingOrder = Mathf.RoundToInt (-transform.position.y * 10f + transform.position.x);
		if (attackTarget == null && isActive) {
			FindPlayer ();
		}

		if (IsHighlighted ()) {
			Highlight ();
		}
		float distanceBetweenPlayer = Vector2.Distance (player.center.position, center.position);
		audioSource.volume = 1 - (distanceBetweenPlayer / 5f);
	}

	public override void SetDirectionTo(Vector2 destination) {
		this.destination = destination;
	}

	void FindPlayer() {
		Collider2D[] colliders2D = Physics2D.OverlapCircleAll (this.GetComponent<Collider2D>().transform.position, playerFinderRange);
		foreach (Collider2D otherCollider2D in colliders2D) {
			if (otherCollider2D.tag == "Player") {
				attackTarget = otherCollider2D.gameObject;
			}
		}
	}

	public override void Hit (float damage, CharacterMovement attacker)
	{
		base.Hit (damage, attacker);
		attackTarget = attacker.gameObject;
	}

	protected override void DeadAction ()
	{
		base.DeadAction ();
		roomManager.EnemyDead (this);
	}

	bool IsHighlighted ()
	{
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast (mousePosition, Vector2.zero);

		if (hit.collider != null) {
			if (hit.collider.gameObject == gameObject) {
				return true;
			}
		}
		return false;
	}

	void Highlight ()
	{
	}
}
