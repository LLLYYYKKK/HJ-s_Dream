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
	int nextSkillIndex = -1;
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

			if (nextSkillIndex != -1) {
				TryUseSkill (nextSkillIndex);
			}
		}
	}

	public void TryUseSkill (int skillIndex)
	{
		if (IsCanUseSkillExist (skillIndex)) {
			if (currentSkill != null) {
				if (currentSkill.isSkillActivate) {
					Debug.Log ("다른 스킬을 사용중");
					nextSkillIndex = skillIndex;
					return;
				}
			}

			if (nextSkillIndex == skillIndex) {
				nextSkillIndex = -1;
			}
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Skill selectedSkill = canUseSkills [skillIndex].GetComponent<Skill> ();
			selectedSkill.TryUseSkill (playerMovement, mousePosition);
		}

	}

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

			GameObject newSkill = Instantiate(skill, obtainedSkillsTransform);
			newSkill.GetComponent<Skill> ().skillManager = this;
			obtainedSkills.Add (newSkill);
			for (int i = 0; i < canUseSkillSize; i++) {
				if (canUseSkills [i] == null) {
					SetCanUseSkill (i, obtainedSkills.Count - 1);
					break;
				}
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

	public void SkillUsed (Skill skill)
	{
		currentSkill = skill;
		playerController.controlState = PlayerController.ControlState.Attack;
	}

	public void SkillIsInCooTime ()
	{
		audioSource.PlayOneShot (skillCoolTimeSound);
	}

	public bool IsInSkillCasting ()
	{
		if (currentSkill != null) {
			return currentSkill.isSkillActivate;
		}
		return false;
	}

	public void CancleWaitCastSkill ()
	{
		nextSkillIndex = -1;
		foreach (GameObject canUseSkill in canUseSkills) {
			try {
				TargetingSkill targetingSkill = canUseSkill.GetComponent<TargetingSkill> ();
				targetingSkill.CancleTrace();
			} catch {
			}
		}
	}
}
