using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public string itemName;
	public float canOptainTime = 0.5f;
	float timer;
	// Use this for initialization
	void Start () {
		timer = 0f;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		timer += Time.deltaTime;
	}

	protected virtual void ObtainItem (PlayerMovement playerMovement)
	{
		ItemManager itemManager = playerMovement.GetComponent<ItemManager> ();
		itemManager.ObtainItem (this);
	}

	public virtual void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		Destroy (gameObject);
	}

	public virtual string GetDescription() 
	{
		return "";
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			if (timer >= canOptainTime) {
				ObtainItem (other.gameObject.GetComponent<PlayerMovement>());
			}
		}
	}

	void OnCollisionStay2D(Collision2D other) {
		OnCollisionEnter2D (other);
	}
}
