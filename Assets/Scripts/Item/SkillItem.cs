using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : MonoBehaviour {
	public GameObject skill;

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			
			SkillManager skillManager = other.gameObject.GetComponent<SkillManager> ();
			skillManager.ObtainSkill (skill, true);

			Destroy (gameObject);
		}
	}
}
