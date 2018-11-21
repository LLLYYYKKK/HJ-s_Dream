using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICanvas : MonoBehaviour {
	public Sprite skillDescriptionSprite;
	public GameObject obtainedSkillScrollView;
	public Sprite defaultSkillButtonSprite;

	GameObject player;
	Transform skillUI;
	Transform hpUI;
	Button[] skillButtons;

	GameObject currentScrollView;

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
		Transform statusUI = transform.Find ("StatusUI");
		skillUI = statusUI.Find ("SkillUI");
		hpUI = statusUI.Find ("HpUI");
		skillButtons = skillUI.GetComponentsInChildren<Button> ();
		ShowPlayerHp ();
	}

	public void Hit(CharacterMovement target, float damage) {
		ShowDamage (target.gameObject, damage);

		if (target.tag == "Player") {
			ShowPlayerHp ();
		}
	}

	void ShowDamage(GameObject target, float damage) {
		GameObject damageShowerObject = new GameObject ("DamageShower");
		damageShowerObject.transform.SetParent (transform);
		DamageShower damageShower = damageShowerObject.AddComponent<DamageShower> ();
		damageShower.ShowDamage (target, damage);
	}

	public void ShowSkillDescription(int skillIndex, Skill skill) {
		GameObject skillDescriptionObject = new GameObject ("SkillDescription");
		skillDescriptionObject.transform.SetParent (skillButtons [skillIndex].transform, false);
		RectTransform skillDescriptionRectTransform = skillDescriptionObject.AddComponent<RectTransform> ();
		GameObject skillDescriptionImageObject = new GameObject ("Image");
		skillDescriptionImageObject.transform.SetParent (skillDescriptionObject.transform, false);
		Image skillDeiscriptionImage = skillDescriptionImageObject.AddComponent<Image> ();
		skillDeiscriptionImage.sprite = skillDescriptionSprite;
		skillDeiscriptionImage.type = Image.Type.Sliced;
		skillDeiscriptionImage.color = new Color (0f, 0f, 0f, 0.5f);
		GameObject skillDescriptionTextObject = new GameObject ("Text");
		skillDescriptionTextObject.transform.SetParent (skillDescriptionObject.transform, false);

		Text skillDescriptionText = skillDescriptionTextObject.AddComponent<Text> ();
		Font font = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
		skillDescriptionText.font = font;
		skillDescriptionText.material = font.material;
		skillDescriptionText.text = skill.GetDescription (player.GetComponent<PlayerMovement>());
		ContentSizeFitter sizeFitter = skillDescriptionTextObject.AddComponent<ContentSizeFitter> ();
		sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		skillDeiscriptionImage.rectTransform.sizeDelta = new Vector2 (skillDescriptionText.preferredWidth + 20, skillDescriptionText.preferredHeight - 10);

		RectTransform imageRectTransform = skillDeiscriptionImage.rectTransform;

		float localX = 0f;
		float localY = 25 + skillDeiscriptionImage.rectTransform.sizeDelta.y * 0.5f;

		Vector2 areaOutOfCanvas = CalculateAreaOutOfCanvas (imageRectTransform);

		skillDescriptionRectTransform.anchoredPosition = new Vector2 (localX, localY) - areaOutOfCanvas;
	}


	Vector2 CalculateAreaOutOfCanvas(RectTransform rectTransform) {
		RectTransform canvasRectTransform = GetComponent<RectTransform> ();

		float delta = canvasRectTransform.position.x / (canvasRectTransform.sizeDelta.x * 0.5f);

		Vector2 canvasMax = CalculateMax (canvasRectTransform, delta);
		Vector2 rectMax = CalculateMax (rectTransform, delta);

		float x = 0.0f;
		float y = 0.0f;

		if (canvasMax.x < rectMax.x) {
			float diffrence = rectMax.x - canvasMax.x;
			x = diffrence;
		}

		if (canvasMax.y < rectMax.y) {
			float diffrence = rectMax.y - canvasMax.y;
			y = diffrence;
		}

		return new Vector2 (x, y);
	}

	Vector2 CalculateMax (RectTransform rectTransform, float delta)
	{
		return new Vector2 (rectTransform.position.x / delta + rectTransform.sizeDelta.x * 0.5f, rectTransform.position.y / delta + rectTransform.sizeDelta.y * 0.5f);
	}

	public void UnshowSkillDescription(int skillIndex) {
		Destroy (skillButtons [skillIndex].transform.Find ("SkillDescription").gameObject);
	}

	void ShowPlayerHp ()
	{
		Slider hpSlider = hpUI.GetComponentInChildren<Slider> ();
		PlayerMovement playerMovement = player.GetComponent<PlayerMovement> ();
		hpSlider.value = playerMovement.hp / playerMovement.maxHp;
		hpSlider.fillRect.GetComponent<Image> ().color = new Color (1f, hpSlider.value, hpSlider.value);
		hpUI.GetComponentInChildren<Text> ().text = Mathf.CeilToInt (playerMovement.hp).ToString();
	}

	public void SetCanUseSkill (int skillIndex, Skill skill)
	{
		skillButtons [skillIndex].GetComponent<Image> ().sprite = skill.skillIcon;
	}

	public void ShowSkillCoolTime (int i, float skillCoolTimer)
	{
		Text skillButtonText = skillButtons [i].transform.Find ("CoolTimeText").GetComponent<Text> ();
		if (skillCoolTimer != 0.0f) {
			int coolTime = Mathf.CeilToInt (skillCoolTimer);
			skillButtons [i].interactable = false;
			skillButtonText.text = coolTime.ToString ();
		} else {
			skillButtonText.text = "";
			skillButtons [i].interactable = true;
		}
	}

	public void ShowObtainedSkills (List<GameObject> obtainedSkills, int atCanUseSkillIndex)
	{
		Destroy (currentScrollView);
		currentScrollView = Instantiate (obtainedSkillScrollView, skillButtons[atCanUseSkillIndex].transform);
		RectTransform rectTransform = currentScrollView.GetComponent<RectTransform> ();

		int contentHeight = 55 * obtainedSkills.Count;
		Transform contentTransform = currentScrollView.transform.Find ("Viewport").Find ("Content");
		RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform> ();
		contentRectTransform.sizeDelta = new Vector2 (0, contentHeight);

		Vector2 position = new Vector2 (0, 180);
		Vector2 areaOutOfCanvas = CalculateAreaOutOfCanvas (rectTransform);
		rectTransform.anchoredPosition = new Vector2 (0, 180);

		rectTransform.anchoredPosition = position - areaOutOfCanvas;

		currentScrollView.transform.SetParent (transform);
		currentScrollView.transform.SetSiblingIndex (0);

		EventTrigger unshowTrigger = currentScrollView.AddComponent<EventTrigger> ();
		EventTrigger.Entry unshowTriggerEntry = new EventTrigger.Entry ();
		unshowTriggerEntry.eventID = EventTriggerType.PointerExit;
		unshowTriggerEntry.callback.AddListener ((data) => {
			Destroy(currentScrollView);
		});
		unshowTrigger.triggers.Add (unshowTriggerEntry);

		int index = 0;
		foreach (GameObject obtainedSkill in obtainedSkills) {
			Skill skill = obtainedSkill.GetComponent<Skill> ();

			GameObject obtainedSkillButtonObject = new GameObject ("ObtainedSkillButton");
			obtainedSkillButtonObject.transform.SetParent (contentTransform, false);
			RectTransform obtainedSkilButtonRectTransform = obtainedSkillButtonObject.AddComponent<RectTransform> ();
			Button obtainedSkillButton = obtainedSkillButtonObject.AddComponent<Button> ();
			Image buttonImage = obtainedSkillButtonObject.AddComponent<Image> ();

			obtainedSkillButton.targetGraphic = buttonImage;

			buttonImage.sprite = skill.skillIcon;
			obtainedSkilButtonRectTransform.sizeDelta = new Vector2 (50, 50);
			obtainedSkilButtonRectTransform.anchorMin = Vector2.zero;
			obtainedSkilButtonRectTransform.anchorMax = Vector2.zero;
			obtainedSkilButtonRectTransform.pivot = new Vector2 (-0.1f, 0.1f);
			obtainedSkilButtonRectTransform.anchoredPosition = new Vector3 (0, 50 * index, 0);

			EventTrigger trigger = obtainedSkillButtonObject.AddComponent<EventTrigger> ();

			EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry ();
			pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
			pointerEnterEntry.callback.AddListener ((data) => {
				ShowSkillDescription (atCanUseSkillIndex, skill);
			});
			trigger.triggers.Add (pointerEnterEntry);

			EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry ();
			pointerExitEntry.eventID = EventTriggerType.PointerExit;
			pointerExitEntry.callback.AddListener ((data) => {
				UnshowSkillDescription (atCanUseSkillIndex);
			});
			trigger.triggers.Add (pointerExitEntry);

			int obtainedSkillIndex = index;
			SkillManager skillManager = player.GetComponent<SkillManager> ();
			obtainedSkillButton.onClick.AddListener(() => skillManager.SetCanUseSkill(atCanUseSkillIndex, obtainedSkillIndex));

			index++;
		}
	}

	public void SetCanUseSkillNull (int skillIndex)
	{
		skillButtons [skillIndex].GetComponent<Image> ().sprite = defaultSkillButtonSprite;
		skillButtons[skillIndex].transform.Find ("CoolTimeText").GetComponent<Text> ().text = "";
		skillButtons [skillIndex].interactable = true;
	}
}
