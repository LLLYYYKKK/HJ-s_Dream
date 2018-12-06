using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationHitObject : LinearMoveHitObject {
	public float acceleration = 0.2f;

	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
		rbody2D.velocity += rbody2D.velocity * acceleration * Time.fixedDeltaTime;;
	}
}
