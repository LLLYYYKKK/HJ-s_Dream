using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : Item {
	public GameObject skill;

	protected override void ObtainItem (PlayerMovement playerMovement)
	{
		SkillManager skillManager = playerMovement.GetComponent<SkillManager> ();
		skillManager.ObtainSkill (skill, true);
		Destroy (gameObject);
	}
}
