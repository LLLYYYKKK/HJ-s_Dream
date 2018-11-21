using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFuckYou : Skill {
	public GameObject hitObject;

	Vector2 hitObjectSpawnPosition;
	Vector2 destination;
	GameObject instantiatedHitObject;

	void FixedUpdate() {
		if (instantiatedHitObject != null) {
			float distance = Vector2.Distance (instantiatedHitObject.transform.position, hitObjectSpawnPosition);
			if (distance >= skillRange) {
				Destroy (instantiatedHitObject);
			}
		}
	}

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
		base.UseSkill (caster, mousePosition);
		destination = mousePosition;
	}

	public override void SkillAction ()
	{
		base.SkillAction ();
		instantiatedHitObject = Instantiate (hitObject, caster.hitObjectSpawnPoint);
		hitObjectSpawnPosition = caster.hitObjectSpawnPoint.transform.position;
		HitObject hitObjectScript = instantiatedHitObject.GetComponent<HitObject> ();
		hitObjectScript.Hit (caster, destination, targetTag);
		hitObjectScript.damage = CalculateDamage (caster);
	}

	public override float CalculateDamage (CharacterMovement caster)
	{
		float damage = skillBasicDamage + skillDamageCoefficient * caster.attackPower;
		return damage;
	}

	public override void EndSkill ()
	{
		base.EndSkill ();
		caster.canMove = true;
	}

	public override string GetDescription (CharacterMovement caster)
	{
		return GetSkillDescriptionTitle(caster) + "<color=" + DESCRIPTION_COLOR + "><b>" + caster.name + "</b>" + "(이)가 " + skillBasicDamage.ToString() + "(<color=" + DAMAGE_COEFFICIENT_COLOR + ">+" + (skillDamageCoefficient * caster.attackPower).ToString() + "</color>)" + "의 데미지를 주는 엿을 날립니다.\n엿에 닿은 적은 0.1초에 한 번씩 데미지를 입습니다." + "</color>";
	}
}
