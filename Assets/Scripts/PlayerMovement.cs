using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement {
	[Range(0f, 1f)] public float luck = 0.2f; 
	public int keyCardCount = 0;
	public int coinCount = 0;
	public bool isRanger;
	public GameObject[] basicSkills;
	SkillManager skillManager;

	protected override void Awake ()
	{
		base.Awake ();
		skillManager = GetComponent<SkillManager> ();
	}

	void Start() {
		if (basicSkills.Length != 0) {
			foreach (var basicSkill in basicSkills) {
				skillManager.ObtainSkill (basicSkill);
				skillManager.SetCanUseSkill (0, 0);
			}
		}
	}

	protected override void HitTargetAction ()
	{
		if (!isRanger) {
			CharacterMovement targetCharacterMovement = attackTarget.GetComponent<CharacterMovement> ();
			targetCharacterMovement.Hit (GetAttackPower(), this);
		} else {
			base.HitTargetAction ();
		}
	}

	public override void Hit (float damage, CharacterMovement attacker)
	{
		base.Hit (damage, attacker);
	}

	protected override void DeadAction ()
	{
		uiCanvas.Dead (this);
	}

	public override void AttackPowerUp (float powerUpAmount)
	{
		base.AttackPowerUp (powerUpAmount);
		uiCanvas.UpdateAttackPower (initialAttackPower);
	}

	public override void AttackTimeReduction (float reductionRate)
	{
		base.AttackTimeReduction (reductionRate);
		uiCanvas.UpdateAttackTime (initialAttackTime);
	}

	public override void SpeedUp (float speedUpAmount)
	{
		base.SpeedUp (speedUpAmount);
		uiCanvas.UpdateSpeed (initialSpeed);
	}

	public override void SkillCoomTimeReductionRateUp (float rate)
	{
		base.SkillCoomTimeReductionRateUp (rate);
		uiCanvas.UpdateCoolTimeReduction (InitialSkillCoolTimeReductionRate);
	}
}
