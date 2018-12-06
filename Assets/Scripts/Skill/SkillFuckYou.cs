using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFuckYou : Skill {
	public GameObject hitObject;

	Vector2 hitObjectSpawnPosition;
	Vector2 destination;
	GameObject instantiatedHitObject;

	public override void Update ()
	{
		base.Update ();
		if (isSkillActivate && caster != null) {
			caster.canMove = false;
		}
	}

	public override void UseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		caster.Stop ();
		caster.AttackDone ();
		base.UseSkill (caster, mousePosition);
		destination = mousePosition;
	}

	public override void SkillAction ()
	{
		base.SkillAction ();
		hitObjectSpawnPosition = caster.hitObjectSpawnPoint.transform.position;
		instantiatedHitObject = Instantiate (hitObject);
		instantiatedHitObject.transform.position = hitObjectSpawnPosition;
		LinearMoveHitObject hitObjectScript = instantiatedHitObject.GetComponent<LinearMoveHitObject> ();
		hitObjectScript.range = skillRange;
		hitObjectScript.Hit (caster, destination, targetTag);
		hitObjectScript.damage = CalculateDamage (caster);
	}

	public override float CalculateDamage (CharacterMovement caster)
	{
		float damage = skillBasicDamage + skillDamageCoefficient * caster.GetAttackPower();
		return damage;
	}

	public override void EndSkill ()
	{
		base.EndSkill ();
		caster.canMove = true;
	}

	public override string GetDescription (CharacterMovement caster)
	{
		return GetSkillDescriptionTitle(caster) + "<color=" + DESCRIPTION_COLOR + "><b>" + caster.charcterName + "</b>" + "(이)가 " + skillBasicDamage.ToString() + BuildSkillCoefficientDescription(caster, skillDamageCoefficient) + "의 데미지를 주는 엿을 날립니다.\n엿에 닿은 적은 0.1초에 한 번씩 데미지를 입습니다." + "</color>";
	}
}
