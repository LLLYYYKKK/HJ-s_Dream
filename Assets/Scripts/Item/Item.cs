using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public string itemName;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void ApplyObtainEffect (PlayerMovement playerMovement)
	{
		
	}

	public virtual string GetDescription() 
	{
		return "";
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			ItemManager itemManager = other.gameObject.GetComponent<ItemManager> ();
			itemManager.ObtainItem (this);
			Destroy (gameObject);
		}
	}
}
