using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float findEnemyRange = 10.0f;
	public Texture2D[] idleCursor;
	public GameObject footprint;

	CharacterMovement characterMovement;
	GameObject attackRangeShower;
	SkillManager skillManager;
	Transform center;

	int cursorCount;

	[System.NonSerialized] public int controlState;
	public const int IDLE_STATE = 0;
	public const int WAIT_ATTACK_STATE = 1;
	public const int ATTACK_STATE = 2;

	void Awake() {
		characterMovement = GetComponent<CharacterMovement> ();
		skillManager = GetComponent<SkillManager> ();
		center = transform.Find ("Center");
		attackRangeShower = center.GetChild(0).gameObject;
		characterMovement.canAttack = false;
		controlState = IDLE_STATE;

		cursorCount = 0;

		StartCoroutine (ChangeCursor ());
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (controlState);

		attackRangeShower.SetActive (false);

		if (Input.GetMouseButtonDown (1)) {
			GameObject clickedEnemy = CheckEnemyClicked ();
			if (clickedEnemy != null) {
				AttackEnemy (clickedEnemy);
			} else {
				Move ();
			}
		}

		if (Input.GetButtonDown("Stop")) {;
			characterMovement.Stop ();
			controlState = IDLE_STATE;
		}

		if (Input.GetButtonDown ("Attack")) {
			controlState = WAIT_ATTACK_STATE;
		}

		if (Input.GetButtonDown("Skill1")) {
			skillManager.UseSkill (0);
			// controlState = ATTACK_STATE;
		}

		if (Input.GetButtonDown("Skill2")) {
			skillManager.UseSkill (1);
			// controlState = ATTACK_STATE;
		}

		if (Input.GetButtonDown("Skill3")) {
			skillManager.UseSkill (2);
			// controlState = ATTACK_STATE;
		}

		if (Input.GetButtonDown("Skill4")) {
			skillManager.UseSkill (3);
			// controlState = ATTACK_STATE;
		}

		switch (controlState) {
		case IDLE_STATE:
			characterMovement.canAttack = false;
			characterMovement.attackTarget = null;
			characterMovement.isAlwaysTracingTarget = false;
			break;

		case WAIT_ATTACK_STATE:
			attackRangeShower.SetActive (true);
			attackRangeShower.transform.localScale = new Vector3 (characterMovement.attackRange, characterMovement.attackRange);

			if (Input.GetMouseButtonDown (0)) {
				GameObject clickedEnemy = CheckEnemyClicked ();
				AttackEnemy (clickedEnemy);
				SetDestinationToMousePosition ();
			}
			break;
		case ATTACK_STATE:
			GameObject nearstEnemy = FindNearstEnemy ();
			characterMovement.isAlwaysTracingTarget = true;

			if (characterMovement.attackTarget == null) {
				characterMovement.attackTarget = nearstEnemy;
			}

			break;
		}
	}

	void Move ()
	{
		controlState = IDLE_STATE;
		SetDestinationToMousePosition ();
		characterMovement.CancleAttack ();
	}

	void SetDestinationToMousePosition ()
	{
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		characterMovement.SetDirectionTo(mousePosition);

		switch (controlState) {
		case IDLE_STATE:
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
				return hit.collider.gameObject;
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
				if (enemy.GetComponent<CharacterMovement> ().isActive) {
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
		controlState = ATTACK_STATE;
		characterMovement.canAttack = true;
		characterMovement.attackTarget = clickedEnemy;
	}

	IEnumerator ChangeCursor ()
	{
		while(true) {
			switch (controlState) {
			case IDLE_STATE:
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
