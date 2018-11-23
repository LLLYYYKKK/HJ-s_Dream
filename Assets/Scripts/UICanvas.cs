using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICanvas : MonoBehaviour {
	public Sprite skillDescriptionSprite;
	public GameObject obtainedSkillScrollView;
	public Sprite defaultSkillButtonSprite;
	public GameObject characterHpBar;
	public AudioClip enterSound;
	public AudioClip closeSound;

	AudioSource audioSource;

	GameObject player;
	Transform skillUI;
	Transform hpUI;
	Button[] skillButtons;
	Dictionary<CharacterMovement, GameObject> characterHpBarDictionary;
	Dictionary<CharacterMovement, float> characterHpBarTimerDictionary;

	GameObject currentScrollView;

	void Awake() {
		audioSource = GetComponent<AudioSource> ();

		player = GameObject.FindGameObjectWithTag ("Player");
		Transform statusUI = transform.Find ("StatusUI");
		skillUI = statusUI.Find ("SkillUI");
		hpUI = statusUI.Find ("HpUI");
		skillButtons = skillUI.GetComponentsInChildren<Button> ();
		int i = 0;
		foreach (var button in skillButtons) {
			int buttonIndex = i;
			button.onClick.AddListener (() => {
				audioSource.PlayOneShot (enterSound);
				UnshowSkillDescription(buttonIndex);
			});
			i++;
		}
		ShowPlayerHp ();
		characterHpBarDictionary = new Dictionary<CharacterMovement, GameObject> ();
		characterHpBarTimerDictionary = new Dictionary<CharacterMovement, float> ();
	}

	void Update() {
		UpdateCharacterHpBar ();
	}

	void UpdateCharacterHpBar ()
	{
		foreach (var keyValuePair in characterHpBarDictionary) {
			GameObject hpBarObject = keyValuePair.Value;
			CharacterMovement target = keyValuePair.Key;

			hpBarObject.transform.position = CalculateHpBarPosition (target);

			characterHpBarTimerDictionary [keyValuePair.Key] += Time.deltaTime;
		}

		foreach (var keyValuePair in characterHpBarTimerDictionary) {
			if (keyValuePair.Value >= 3f) {
				RemoveCharacterHpBar (keyValuePair.Key);
				return;
			}
		}
	}

	public void Hit(CharacterMovement target, float damage) {
		ShowDamage (target.gameObject, damage);

		if (!characterHpBarDictionary.ContainsKey (target)) {
			CreateCharacterHpBar (target);
		}
		else {
			characterHpBarTimerDictionary [target] = 0f;
			Slider slider = characterHpBarDictionary[target].GetComponent<Slider> ();
			slider.value = target.hp / target.maxHp;
		}

		if (target.tag == "Player") {
			ShowPlayerHp ();
		}
	}

	void CreateCharacterHpBar (CharacterMovement target)
	{
		GameObject instantiatedHpBar = Instantiate (characterHpBar, transform);
		characterHpBarDictionary.Add (target, instantiatedHpBar);
		characterHpBarTimerDictionary.Add (target, 0.0f);
		instantiatedHpBar.transform.position = CalculateHpBarPosition (target);
		if (target.tag == "Enemy") {
			Transform fillArea = instantiatedHpBar.transform.Find ("Fill Area");
			fillArea.GetComponentInChildren<Image> ().color = new Color (1f, 0f, 0f);
		}
		Slider slider = instantiatedHpBar.GetComponent<Slider> ();
		slider.value = target.hp / target.maxHp;
	}

	public void Dead (CharacterMovement characterMovement)
	{
		RemoveCharacterHpBar (characterMovement);
	}

	void RemoveCharacterHpBar (CharacterMovement target)
	{
		if (characterHpBarDictionary.ContainsKey (target)) {
			Destroy (characterHpBarDictionary [target]);
			characterHpBarDictionary.Remove (target);
			characterHpBarTimerDictionary.Remove (target);
		}
	}

	Vector3 CalculateHpBarPosition (CharacterMovement target)
	{
		return Camera.main.WorldToScreenPoint (target.transform.position - new Vector3(0, 0.2f, 0));
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
		Image skillDeiscriptionImage = skillDescriptionObject.AddComponent<Image> ();
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
		Vector2 localPosition = new Vector2(localX, localY);
		SetAnchoredPosition (skillDescriptionRectTransform, localPosition);
	}

	void SetAnchoredPosition (RectTransform rectTransform, Vector2 position)
	{
		rectTransform.anchoredPosition = position;

		Vector2 areaOutOfCanvas = CalculateAreaOutOfCanvas (rectTransform);

		rectTransform.anchoredPosition = position - areaOutOfCanvas;
	}

	Vector2 CalculateAreaOutOfCanvas(RectTransform rectTransform) {
		RectTransform canvasRectTransform = GetComponent<RectTransform> ();
		Vector2 canvasMax = CalculateMax (canvasRectTransform);
		Vector2 rectMax = CalculateMax (rectTransform);

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

	Vector2 CalculateMax (RectTransform rectTransform)
	{
		RectTransform canvasRectTransform = GetComponent<RectTransform> ();
		float deltaX = canvasRectTransform.position.x / (canvasRectTransform.sizeDelta.x * 0.5f);
		float deltaY = canvasRectTransform.position.y / (canvasRectTransform.sizeDelta.y * 0.5f);

		return new Vector2 (rectTransform.position.x / deltaX + rectTransform.sizeDelta.x * 0.5f, rectTransform.position.y / deltaY + rectTransform.sizeDelta.y * 0.5f);
	}

	public void UnshowSkillDescription(int skillIndex) {
		Transform skillDescriptionTranform = skillButtons [skillIndex].transform.Find ("SkillDescription");
		if (skillDescriptionTranform != null) {
			Destroy (skillDescriptionTranform.gameObject);
		}
	}

	void ShowPlayerHp ()
	{
		Slider hpSlider = hpUI.GetComponentInChildren<Slider> ();
		PlayerMovement playerMovement = player.GetComponent<PlayerMovement> ();
		hpSlider.value = playerMovement.hp / playerMovement.maxHp;
		hpSlider.fillRect.GetComponent<Image> ().color = new Color (1f, hpSlider.value, hpSlider.value);
		hpSlider.GetComponentInChildren<Text> ().text = Mathf.CeilToInt (playerMovement.hp).ToString();
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
		SetAnchoredPosition (rectTransform, position);

		currentScrollView.transform.SetParent (transform);
		currentScrollView.transform.SetSiblingIndex (0);

		EventTrigger unshowTrigger = currentScrollView.AddComponent<EventTrigger> ();
		EventTrigger.Entry unshowTriggerEntry = new EventTrigger.Entry ();
		unshowTriggerEntry.eventID = EventTriggerType.PointerExit;
		unshowTriggerEntry.callback.AddListener ((data) => {
			audioSource.PlayOneShot(closeSound);
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
			obtainedSkillButton.onClick.AddListener(() => {
				skillManager.SetCanUseSkill(atCanUseSkillIndex, obtainedSkillIndex);
				audioSource.PlayOneShot(enterSound);
				Destroy(currentScrollView);
				UnshowSkillDescription(atCanUseSkillIndex);
			});

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
