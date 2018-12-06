using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float findEnemyRange = 10.0f;
	public Texture2D[] idleCursor;
	public GameObject footprint;

	PlayerMovement playerMovement;
	GameObject attackRangeShower;
	SkillManager skillManager;
	Transform center;

	int cursorCount;

	public enum ControlState {Idle, WaitAttack, Attack};
	public ControlState controlState;

	void Awake() {
		playerMovement = GetComponent<PlayerMovement> ();
		skillManager = GetComponent<SkillManager> ();
		center = transform.Find ("Center");
		attackRangeShower = center.GetChild(0).gameObject;
		playerMovement.canAttack = false;
		controlState = ControlState.Idle;

		cursorCount = 0;

		StartCoroutine (ChangeCursor ());
	}
	
	// Update is called once per frame
	void Update () {
		if (playerMovement.isAlive) {
			attackRangeShower.SetActive (false);

			if (Input.GetMouseButtonDown (1)) {
				skillManager.CancleWaitCastSkill ();
				GameObject clickedEnemy = CheckEnemyClicked ();
				if (clickedEnemy != null) {
					AttackEnemy (clickedEnemy);
				} else {
					Move ();
				}
			}

			if (Input.GetButtonDown ("Stop")) {
				playerMovement.Stop ();
				controlState = ControlState.Idle;
			}

			if (Input.GetButtonDown ("Attack")) {
				controlState = ControlState.WaitAttack;
			}

			if (Input.GetButtonDown ("Skill1")) {
				skillManager.TryUseSkill (0);
				// controlState = ATTACK_STATE;
			}

			if (Input.GetButtonDown ("Skill2")) {
				skillManager.TryUseSkill (1);
				// controlState = ATTACK_STATE;
			}

			if (Input.GetButtonDown ("Skill3")) {
				skillManager.TryUseSkill (2);
				// controlState = ATTACK_STATE;
			}

			if (Input.GetButtonDown ("Skill4")) {
				skillManager.TryUseSkill (3);
				// controlState = ATTACK_STATE;
			}

			switch (controlState) {
			case ControlState.Idle:
				playerMovement.canAttack = false;
				playerMovement.attackTarget = null;
				playerMovement.isAlwaysTracingTarget = false;
				break;

			case ControlState.WaitAttack:
				attackRangeShower.SetActive (true);
				attackRangeShower.transform.localScale = new Vector3 (playerMovement.GetAttackRange (), playerMovement.GetAttackRange ());

				if (Input.GetMouseButtonDown (0)) {
					skillManager.CancleWaitCastSkill ();
					GameObject clickedEnemy = CheckEnemyClicked ();
					AttackEnemy (clickedEnemy);
					SetDestinationToMousePosition ();
				}
				break;
			case ControlState.Attack:
				GameObject nearstEnemy = FindNearstEnemy ();
				playerMovement.isAlwaysTracingTarget = true;

				if (playerMovement.attackTarget == null) {
					playerMovement.attackTarget = nearstEnemy;
				}

				break;
			}
		} else {
			skillManager.CancleWaitCastSkill ();
		}
	}

	void Move ()
	{
		controlState = ControlState.Idle;
		SetDestinationToMousePosition ();
		if (!skillManager.IsInSkillCasting()) {
			playerMovement.CancleAttack ();
		}
	}

	void SetDestinationToMousePosition ()
	{
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		playerMovement.SetDestinatioTo(mousePosition);

		switch (controlState) {
		case ControlState.Idle:
			cursorCount = 0;
			Instantiate (footprint, mousePosition, Quaternion.identity);
			break;
		}
	}

	GameObject CheckEnemyClicked() {
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast (mousePosition, Vector2.zero);

		if (hit.collider != null) {
			if (hit.collider.tag == "Enemy") {
				if (hit.collider.GetComponent<CharacterMovement> ().isAlive) {
					return hit.collider.gameObject;
				}
			}
		}

		return null;
	}

	GameObject FindNearstEnemy ()
	{
		Collider2D[] enemies = Physics2D.OverlapCircleAll (
			center.position, 
			findEnemyRange
		);
		Collider2D nearstEnemy = null;
		float distanceBetweenNearstEnemy = 0.0f;
		foreach (Collider2D enemy in enemies) {
			if (enemy.tag == "Enemy") {
				if (enemy.GetComponent<CharacterMovement> ().isAlive) {
					if (nearstEnemy == null) {
						nearstEnemy = enemy;
						distanceBetweenNearstEnemy = Vector2.Distance (transform.position, nearstEnemy.transform.position);
					} else {
						float distance = Vector2.Distance (transform.position, enemy.transform.position);
						if (distance < distanceBetweenNearstEnemy) {
							nearstEnemy = enemy;
							distanceBetweenNearstEnemy = distance;
						}
					}
				}
			}
		}

		if (nearstEnemy == null) {
			return null;
		}

		return nearstEnemy.gameObject;
	}

	void AttackEnemy (GameObject clickedEnemy)
	{
		controlState = ControlState.Attack;
		if (!skillManager.IsInSkillCasting ()) {
			playerMovement.canAttack = true;
		}
		playerMovement.attackTarget = clickedEnemy;
	}

	IEnumerator ChangeCursor ()
	{
		while(true) {
			switch (controlState) {
			case ControlState.Idle:
				if (cursorCount < idleCursor.Length) {
					Cursor.SetCursor (idleCursor [cursorCount], new Vector2(10, 10), CursorMode.Auto);
				} else {
					cursorCount = 0;
				}
				cursorCount++;
				break;
			}

			yield return new WaitForSeconds (0.1f);
		}
	}
}
