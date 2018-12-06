using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSkill : Skill {
	protected CharacterMovement target;
	bool isTracingTarget;

	public override void Update ()
	{
		base.Update ();
		if (isTracingTarget) {
			if (IsTargetInRange()) {
				UseSkill (caster, target.center.position);
			}
		}
	}

	public override void UseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		caster.SetDestinatioTo (caster.transform.position);
		base.UseSkill (caster, mousePosition);
		isTracingTarget = false;
	}

	public override void TryUseSkill (CharacterMovement caster, Vector2 mousePosition)
	{
		if (skillCoolTimer > 0.0f) {
			skillManager.SkillIsInCooTime ();
		} else {
			this.caster = caster;
			target = GetTarget (mousePosition);
			if (target != null) {
				caster.CancleAttack ();
				caster.attackTarget = target.gameObject;
				if (IsTargetInRange ()) {
					UseSkill (caster, mousePosition);
				} else {
					caster.SetDestinatioTo (target.transform.position);
					caster.canAttack = false;
					isTracingTarget = true;
				}
			}
		}
	}

	CharacterMovement GetTarget (Vector2 mousePosition)
	{
		RaycastHit2D hit = Physics2D.Raycast (mousePosition, Vector2.zero);

		if (hit.collider != null) {
			if (hit.collider.tag == targetTag) {
				CharacterMovement selectedTarget = hit.collider.GetComponent<CharacterMovement> ();
				if (selectedTarget.isAlive) {
					return selectedTarget;
				}
			}
		}

		return null;
	}

	bool IsTargetInRange ()
	{
		if (target != null) {
			Collider2D[] colliders = Physics2D.OverlapCircleAll (caster.center.transform.position, skillRange);
			foreach (Collider2D collider2D in colliders) {
				if (collider2D.gameObject == target.gameObject) {
					return true;
				}
			}
		}

		return false;
	}

	bool IsSkillCancled ()
	{
		return target.gameObject != caster.attackTarget;
	}

	public void CancleTrace ()
	{
		isTracingTarget = false;
	}
}
