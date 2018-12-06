using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour {
	[Range(1, 5)] public int skillLevel = 1;
	public SkillManager skillManager;

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
	[System.NonSerialized] public bool canSkillAction;
	[System.NonSerialized] public static string SKILL_NAME_COLOR = "white";
	[System.NonSerialized] public static string DESCRIPTION_COLOR = "lightblue";
	[System.NonSerialized] public static string DAMAGE_COEFFICIENT_COLOR = "lime";

	float skillCastTimer;

	public virtual void Awake() {skillCoolTimer = 0.0f;
		skillCastTimer = 0.0f;
		isSkillActivate = false;
		audioSource = GetComponent<AudioSource> ();
		canSkillAction = false;
	}

	public virtual void Update() {
		if (isSkillActivate) {
			skillCastTimer += Time.deltaTime;
			if (skillCastTimer >= skillActionTime && canSkillAction) {
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

	public virtual void UseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		isSkillActivate = true;
		canSkillAction = true;
		caster.UseSkill (skillAnimationTrigger, skillCastTime);
		skillCoolTimer = CalculateCoolTime(caster);

		skillManager.SkillUsed (this);
	}

	public virtual void SkillAction () {
		audioSource.PlayOneShot (skillSound);
		canSkillAction = false;
	}

	public virtual void EndSkill () {
		isSkillActivate = false;
		skillCastTimer = 0.0f;
		caster.EndSkill ();
	}

	public virtual string GetDescription(CharacterMovement caster) {
		return "";
	}

	public virtual float CalculateDamage(CharacterMovement caster) {
		return skillBasicDamage + skillDamageCoefficient * caster.GetAttackPower();
	}

	public virtual void CoolTimeEnd ()
	{
		skillCoolTimer = 0.0f;	
	}

	protected string GetSkillDescriptionTitle(CharacterMovement caster) {
		return "<color=#ffffff><size=15><b>" + skillName + "</b></size> Lv." + skillLevel.ToString() + "\t\t" + "<size=12>쿨타임 : <b>" + CalculateCoolTime(caster).ToString() + "초</b></size></color>\n\n";
	}

	float CalculateCoolTime (CharacterMovement caster)
	{
		return skillCoolTime - skillCoolTime * caster.GetSkillCoolTimeReductionRate();
	}

	public virtual void TryUseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		if (skillCoolTimer > 0.0f) {
			skillManager.SkillIsInCooTime ();
		} else {
			this.caster = caster;
			UseSkill (caster, mousePosition);
		}
	}
		
	protected string BuildSkillCoefficientDescription (CharacterMovement caster, float skillDamageCoefficient)
	{
		return "(<color=" + DAMAGE_COEFFICIENT_COLOR + ">+" + (skillDamageCoefficient * caster.GetAttackPower ()).ToString () + "</color>)";
	}
}
