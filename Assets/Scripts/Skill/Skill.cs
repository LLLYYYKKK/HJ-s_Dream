using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour {
	[Range(1, 5)] public int skillLevel = 1;

	public float skillCastTime = 0.5f;
	public float skillActionTime = 0.3f;
	public float[] skillBasicDamageByLevel;
	public float skillBasicDamage {
		get {
			return skillBasicDamageByLevel [skillLevel - 1];
		}
	}
	public float skillDamageCoefficient = 0.6f;
	public float[] skillCoolTimeByLevel;
	public float skillCoolTime {
		get {
			return skillCoolTimeByLevel [skillLevel - 1];
		}
	}
	public float skillRange = 5.0f;
	public Sprite skillIcon;
	public string targetTag = "Enemy";
	public string skillAnimationTrigger;
	public string skillName;
	public AudioClip skillSound;

	protected CharacterMovement caster;
	AudioSource audioSource;

	[System.NonSerialized] public float skillCoolTimer;

	[System.NonSerialized] public bool isSkillActivate;
	[System.NonSerialized] public static string SKILL_NAME_COLOR = "white";
	[System.NonSerialized] public static string DESCRIPTION_COLOR = "lightblue";
	[System.NonSerialized] public static string DAMAGE_COEFFICIENT_COLOR = "lime";

	float skillCastTimer;

	public virtual void Update() {
		if (isSkillActivate) {
			skillCastTimer += Time.deltaTime;
			if (skillCastTimer >= skillActionTime) {
				SkillAction ();
			}

			if (skillCastTimer >= skillCastTime) {
				EndSkill ();
			}
		}

		if (skillCoolTimer >= 0.0f) {
			skillCoolTimer -= Time.deltaTime;
			if (skillCoolTimer <= 0.0f) {
				CoolTimeEnd ();
			}
		}
	}

	public virtual void Awake() {
		skillCoolTimer = 0.0f;
		skillCastTimer = 0.0f;
		isSkillActivate = false;
		audioSource = GetComponent<AudioSource> ();
	}

	public virtual void UseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		isSkillActivate = true;
		this.caster = caster;
		caster.CancleAttack ();
		caster.AttackDone ();
		caster.See (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		skillCoolTimer = CalculateCoolTime(caster);
		caster.Animator.SetTrigger (skillAnimationTrigger);
		caster.Animator.speed = 1.0f / skillCastTime;
	}

	public virtual void SkillAction () {
		audioSource.PlayOneShot (skillSound);
	}

	public virtual void EndSkill () {
		isSkillActivate = false;
		skillCastTimer = 0.0f;
		caster.Animator.speed = 1.0f;
		caster.canAttack = true;
		caster.canMove = true;
	}

	public virtual string GetDescription(CharacterMovement caster) {
		return "";
	}

	public virtual float CalculateDamage(CharacterMovement caster) {
		return skillBasicDamage + skillDamageCoefficient * caster.attackPower;
	}

	public virtual void CoolTimeEnd ()
	{
		skillCoolTimer = 0.0f;	
	}

	protected string GetSkillDescriptionTitle(CharacterMovement caster) {
		return "<color=#ffffff><size=15><b>" + skillName + "</b></size>\t\t" + "<size=12>쿨타임 : <b>" + CalculateCoolTime(caster).ToString() + "초</b></size></color>\n\n";
	}

	float CalculateCoolTime (CharacterMovement caster)
	{
		return skillCoolTime - skillCoolTime * caster.skillCoolTimeReductionRate;
	}
}
