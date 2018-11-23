using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement {
	public bool isRanger;
	public GameObject basicSkill;
	SkillManager skillManager;

	protected override void Awake ()
	{
		base.Awake ();
		skillManager = GetComponent<SkillManager> ();
	}

	void Start() {
		if (basicSkill != null) {
			skillManager.ObtainSkill (basicSkill);
			skillManager.SetCanUseSkill (0, 0);
		}
	}

	protected override void HitTargetAction ()
	{
		if (!isRanger) {
			CharacterMovement targetCharacterMovement = attackTarget.GetComponent<CharacterMovement> ();
			targetCharacterMovement.Hit (attackPower, this);
		} else {
			base.HitTargetAction ();
		}
	}

	public override void Hit (float damage, CharacterMovement attacker)
	{
		base.Hit (damage, attacker);
	}
}
