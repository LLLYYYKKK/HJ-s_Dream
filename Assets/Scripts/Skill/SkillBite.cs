using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBite : TargetingSkill {
	public float[] basicRecoveryAmountsByLevel;
	public float basicRecoveryAmount {
		get {
			return basicRecoveryAmountsByLevel [skillLevel - 1];
		}
	}
	public float recoveryCoeffiecient = 0.1f;

	public override void UseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		base.UseSkill (caster, mousePosition);
		caster.canMove = false;
	}

	public override void SkillAction ()
	{
		base.SkillAction ();
		target.Hit (CalculateDamage(caster), caster);
		caster.Hit (-CalculateRecoveryAmount(), caster);
	}

	public float CalculateRecoveryAmount() {
		return basicRecoveryAmount + caster.GetAttackPower() * recoveryCoeffiecient; 
	}

	public override string GetDescription (CharacterMovement caster)
	{
		return GetSkillDescriptionTitle (caster) +
		"<color=" + DESCRIPTION_COLOR + "><b>" + caster.charcterName + "</b>" + "(이)가 적을 물어뜯어 " +
			skillBasicDamage.ToString () + BuildSkillCoefficientDescription(caster, skillDamageCoefficient) +
		"의 데미지를 주고 체릭을 " +
			basicRecoveryAmount.ToString () + BuildSkillCoefficientDescription(caster, recoveryCoeffiecient) +
		"만큼 회복합니다.</color>";
	}
}
