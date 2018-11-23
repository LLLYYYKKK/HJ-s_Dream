using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour {
	[System.NonSerialized] public List<GameObject> obtainedSkills;

	int canUseSkillSize = 4;
	GameObject[] canUseSkills;
	PlayerMovement playerMovement;
	PlayerController playerController;
	Transform hitObjectSpawnPoint;
	Transform obtainedSkillsTransform;
	AudioSource audioSource;

	Skill currentSkill;
	GameObject currentAttackTarget;
	UICanvas uiCanvas;

	public AudioClip skillCoolTimeSound;

	public AudioClip obtainSkillSound;

	void Awake () {
		obtainedSkills = new List<GameObject> ();
		playerController = GetComponentInParent<PlayerController> ();
		playerMovement = GetComponentInParent<PlayerMovement> ();
		hitObjectSpawnPoint = transform.Find ("HitObjectSpawnPoint");
		obtainedSkillsTransform = transform.Find ("ObtainedSkills");

		audioSource = GetComponent<AudioSource> ();

		uiCanvas = GameObject.FindGameObjectWithTag ("UICanvas").GetComponent<UICanvas> ();
		Transform skillUI = uiCanvas.transform.Find("StatusUI").Find("SkillUI");

		canUseSkills = new GameObject[canUseSkillSize];
	}

	// Update is called once per frame
	void Update () {
		for (int i = 0; i < canUseSkillSize; i++) {
			if (canUseSkills [i] != null) {
				Skill skill = canUseSkills [i].GetComponent<Skill> ();
				uiCanvas.ShowSkillCoolTime (i, skill.skillCoolTimer);
			}
		}
	}

	public void UseSkill (int skillIndex)
	{
		if (IsCanUseSkillExist(skillIndex)) {
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			if (currentSkill != null) {
				if (currentSkill.isSkillActivate) {
					Debug.Log ("다른 스킬을 사용중");
					return;
				}
			}

			Skill selectedSkill = canUseSkills [skillIndex].GetComponent<Skill> ();
			if (selectedSkill.skillCoolTimer != 0.0f) {
				audioSource.PlayOneShot (skillCoolTimeSound);
				return;
			}
				
			playerController.controlState = PlayerController.ATTACK_STATE;

			currentSkill = selectedSkill;
			currentSkill.UseSkill (playerMovement, mousePosition);

			/*
			if (!isUsingSkill) {
				if (skillTimers [skillIndex] == 0.0f) {
					currentSkill = canUseSkills[skillIndex].GetComponent<Skill> ();
					currentSkill.UseSkill (playerMovement);


					playerMovement.canMove = false;
					playerMovement.Animator.SetTrigger (currentSkill.skillAnimationTrigger);
					playerMovement.Animator.speed = 1.0f / currentSkill.skillCastTime;
					skillCastTimer = 0.0f;
					skillTimers [skillIndex] = currentSkill.skillCoolTime - currentSkill.skillCoolTime * playerMovement.coolTimeReductionRate;
					isUsingSkill = true;
				} else {
					audioSource.PlayOneShot (skillCoolTimeSound);
				}
			} else {
				Debug.Log ("스킬을 사용중");
			}
			*/
		}
	}

	/*
	void EndUseSkill ()
	{
		playerMovement.canAttack = true;
		playerMovement.canMove = true;
		playerMovement.Animator.speed = 1.0f;
		playerController.controlState = PlayerController.ATTACK_STATE;
	}
	*/
	public void ObtainSkill (GameObject skill) {
		if (skill.GetComponent<Skill> () != null) {
			Skill skillScript = skill.GetComponent<Skill> ();
			if (skillScript != null) {
				foreach (GameObject obtainedSkill in obtainedSkills) {
					Skill obtainedSkillScript = obtainedSkill.GetComponent<Skill> ();
					if (obtainedSkillScript.skillName == skillScript.skillName) {
						obtainedSkillScript.skillLevel += 1;
						return;
					}
				}
			}

			obtainedSkills.Add (Instantiate (skill, obtainedSkillsTransform));
			if (canUseSkills [0] == null) {
				SetCanUseSkill (0, 0);	
			}
		}
	}

	public void ObtainSkill(GameObject skill, bool withSound) {
		ObtainSkill (skill);
		if (withSound) {
			audioSource.PlayOneShot (obtainSkillSound);
		}
	}

	public void SetCanUseSkill (int skillIndex, int obtainedSkillIndex)
	{
		for (int i = 0; i < canUseSkillSize; i++) {
			if (canUseSkills [i] == obtainedSkills [obtainedSkillIndex]) {
				SetCanUseSkillNull (i);
			}
		}

		canUseSkills [skillIndex] = obtainedSkills [obtainedSkillIndex];
		uiCanvas.SetCanUseSkill (skillIndex, canUseSkills [skillIndex].GetComponent<Skill> ());
	}

	public void SetCanUseSkillNull(int skillIndex) {
		canUseSkills [skillIndex] = null;
		uiCanvas.SetCanUseSkillNull(skillIndex);
	}

	public void ShowSkillDescription(int skillIndex) {
		if (IsCanUseSkillExist(skillIndex)) {
			Skill skill = canUseSkills [skillIndex].GetComponent<Skill> ();
			uiCanvas.ShowSkillDescription (skillIndex, skill);
		}
	}

	public void UnshowSkillDescription(int skillIndex) {
		if (IsCanUseSkillExist(skillIndex)) {
			uiCanvas.UnshowSkillDescription (skillIndex);
		}
	}

	public void ShowObtainedSkills(int atCanUseSkillIndex) {
		uiCanvas.ShowObtainedSkills (obtainedSkills, atCanUseSkillIndex);
	}

	public bool IsCanUseSkillExist(int skillIndex) {
		if (canUseSkills [skillIndex] != null) {
			return true;
		}
		return false;
	}
}
