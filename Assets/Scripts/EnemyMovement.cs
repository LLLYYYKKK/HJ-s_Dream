using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement: CharacterMovement {
	public enum Grade {Normal, Champion, Boss};
	public Grade enemyGrade = Grade.Normal;

	public float playerFinderRange = 3.0f;
	public float dropItemProbability = 0.1f;
	public GameObject[] canDropItems;
	PlayerMovement player;
	public RoomManager roomManager;
	RigidbodyType2D bodyType;

	protected override void Awake ()
	{
		base.Awake ();
		player = GameObject.FindWithTag ("Player").GetComponent<PlayerMovement> ();
		bodyType = rbody2D.bodyType;
	}

	protected override void Update() {
		base.Update ();
		spriteRenderer.sortingOrder = Mathf.RoundToInt (-transform.position.y * 100f + transform.position.x * 100f);
		EnemyUpdate ();
	}

	protected void EnemyUpdate() {
		if (attackTarget == null && isAlive) {
			FindPlayer ();
		}

		if (IsHighlighted ()) {
			Highlight ();
		}
		float distanceBetweenPlayer = Vector2.Distance (player.center.position, center.position);
		audioSource.volume = 1 - (distanceBetweenPlayer / 5f);
	}

	void FindPlayer() {
		Collider2D[] colliders2D = Physics2D.OverlapCircleAll (this.GetComponent<Collider2D>().transform.position, playerFinderRange);
		foreach (Collider2D otherCollider2D in colliders2D) {
			if (otherCollider2D.tag == "Player") {
				attackTarget = otherCollider2D.gameObject;
			}
		}
	}

	protected override void StartNewAttack ()
	{
		base.StartNewAttack ();
		rbody2D.bodyType = RigidbodyType2D.Static;
	}

	public override void AttackDone ()
	{
		base.AttackDone ();
		rbody2D.bodyType = bodyType;
	}

	public override void Hit (float damage, CharacterMovement attacker)
	{
		base.Hit (damage, attacker);
		attackTarget = attacker.gameObject;
	}

	protected override void DeadAction ()
	{
		base.DeadAction ();
		Debug.Log (roomManager == null);
		if (roomManager != null) {
			roomManager.EnemyDead (this);
		}
		DropItem ();
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

	void DropItem ()
	{
		float itemRoll = Random.Range (0f, 1f);
		if (itemRoll <= dropItemProbability + player.luck / 2f) {
			Debug.Log (canDropItems.Length);
			if (canDropItems.Length != 0 && canDropItems != null) {
				GameObject item = Instantiate (canDropItems [Random.Range (0, canDropItems.Length)]);
				item.transform.position = transform.position;
				if (roomManager != null) {
					item.transform.SetParent (roomManager.transform);
				}
			}
		}
	}

	public override void CancleAttack ()
	{
		base.CancleAttack ();
		SetDestinatioTo (transform.position);
	}
}
