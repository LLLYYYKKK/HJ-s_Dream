using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// NOTE: Rotation/scale is only supported on the main game object, not children. If child rotation/scaling is necessary,
//       use individual 'OutlineObject' components on each game object with 'includeChildren' set to false.

[ExecuteInEditMode]
public class OutlineObject : MonoBehaviour {

	[Tooltip("Reference to the custom outline shader (Sprites-Outline).")]
	public Material outlineMaterial;

	[Tooltip("Size of the outline, in pixels.")]
	[Range(1, 10)]
	public int outlineSize = 1;

	[Tooltip("Increase to blur the outline by adding anti-aliasing to the outer edges.")]
	[Range(0, 9)]
	public int outlineBlur;

	[Tooltip("Color of the outline.")]
	public Color outlineColor = Color.white;

	[Tooltip("Include child sprites in the outline.")]
	public bool includeChildren;

	[Tooltip("Filter child sprites by layer (only sprites on checked layers will be included).")]
	public LayerMask childLayers = 1 << 0; // Use the 'Default' layer by default.

	[Tooltip("Prevent transparent pixels in child sprites from masking out opaque pixels in overlapping sprites.")]
	public bool childrenOverlap;

	[Tooltip("Generate the outline on game start; also disables real-time changes while in edit mode.")]
	public bool generatesOnStart;

	[Tooltip("Enable to regenerate the outline when the sprite frame changes.")]
	public bool isAnimated;

	[Tooltip("Enable to sort the outline using its Z-axis position instead of its sorting order.")]
	public bool isIsometric;

	SpriteRenderer sprite;
	GameObject     outline;
	SpriteRenderer outlineSprite;
	Texture2D      texture;

	SpriteRenderer _sortingSprite;
	float          _boundsMinX;
	float          _boundsMinY;
	float          _boundsMaxX;
	float          _boundsMaxY;
	Vector2        _anchor;
	Rect           _textureRect = Rect.zero;

	Dictionary<int, Sprite> _cachedOutlineSprites = new Dictionary<int, Sprite> ();
	int                     _lastSpriteFrameId;

	void Start() {
		if (!Application.isPlaying || !generatesOnStart) {
			TryGetOutline ();
			return;
		}

		Regenerate ();
	}

	void TryGetOutline() {
		if (!sprite) {
			sprite         = GetComponent<SpriteRenderer> ();
			_sortingSprite = sprite;
		}

		Transform outlineTransform = transform.Find ("Outline");

		if (outlineTransform) {
			outline       = outlineTransform.gameObject;
			outlineSprite = outline.GetComponent<SpriteRenderer> ();
		}
	}

	void LateUpdate() {
		SortOutline ();

		if (!Application.isPlaying || !isAnimated)
			return;

		int spriteFrameId = sprite.sprite.GetInstanceID ();

		if (spriteFrameId == _lastSpriteFrameId)
			return;

		if (_cachedOutlineSprites.ContainsKey (spriteFrameId)) {
			outlineSprite.sprite = _cachedOutlineSprites [spriteFrameId];
		} else {
			Regenerate ();
		}

		_lastSpriteFrameId = spriteFrameId;
	}

	public void Regenerate() {
		if (!outlineMaterial)
			return;

		Vector3    cachedPosition = transform.position;
		Quaternion cachedRotation = transform.rotation;
		Vector3    cachedScale    = transform.localScale;

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		transform.localScale = new Vector3 (
			transform.localScale.x / transform.lossyScale.x,
			transform.localScale.y / transform.lossyScale.y,
			transform.localScale.z / transform.lossyScale.z
		);

		if (!outline) {
			TryGetOutline ();

			if (!outline) {
				outline = new GameObject ("Outline");

				outlineSprite                = outline.AddComponent<SpriteRenderer> ();
				outlineSprite.material       = new Material (outlineMaterial);
				outlineSprite.sortingLayerID = sprite.sortingLayerID;

				outline.transform.localScale = transform.lossyScale;
				outline.transform.parent     = transform;
			}
		}

		SetupTexture ();
		ClearTexture ();
		FillTexture  (sprite);

		texture.Apply ();

		_textureRect.width  = texture.width;
		_textureRect.height = texture.height;

		_anchor.x = (sprite.sprite.pivot.x + GetOffsetX (sprite)) / texture.width;
		_anchor.y = (sprite.sprite.pivot.y + GetOffsetY (sprite)) / texture.height;

		outlineSprite.sprite = Sprite.Create (texture, _textureRect, _anchor, sprite.sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);

		if (Application.isPlaying && isAnimated) {
			Texture2D textureClone       = Texture2D.Instantiate (texture);
			Sprite    outlineSpriteClone = Sprite   .Create      (textureClone, _textureRect, _anchor, sprite.sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);

			int spriteFrameId = sprite.sprite.GetInstanceID ();

			_cachedOutlineSprites.Add (spriteFrameId, outlineSpriteClone);
		}

		SortOutline ();

		outlineSprite.sharedMaterial.SetInt   ("_OutlineSize",  outlineSize);
		outlineSprite.sharedMaterial.SetColor ("_OutlineColor", outlineColor);
		outlineSprite.sharedMaterial.SetInt   ("_OutlineBlur",  outlineBlur);

		transform.position   = cachedPosition;
		transform.rotation   = cachedRotation;
		transform.localScale = cachedScale;
	}

	void SetupTexture() {
		_boundsMinX = float.MaxValue;
		_boundsMinY = float.MaxValue;
		_boundsMaxX = float.MinValue;
		_boundsMaxY = float.MinValue;

		SpriteRendererExt.GetActiveBounds (sprite, ref _boundsMinX, ref _boundsMinY, ref _boundsMaxX, ref _boundsMaxY, includeChildren, ShouldIgnoreSprite);

		int padding = outlineSize * 2;
		int width   = Mathf.CeilToInt ((_boundsMaxX - _boundsMinX) * sprite.sprite.pixelsPerUnit) + padding;
		int height  = Mathf.CeilToInt ((_boundsMaxY - _boundsMinY) * sprite.sprite.pixelsPerUnit) + padding;

		if (!texture) {
			texture            = new Texture2D (width, height, TextureFormat.RGBA32, false);
			texture.filterMode = sprite.sprite.texture.filterMode;
			texture.wrapMode   = sprite.sprite.texture.wrapMode;
		} else {
			texture.Resize (width, height);
		}
	}

	void ClearTexture() {
		Color32[] pixels      = texture.GetPixels32 ();
		int       pixelsCount = pixels.Length;

		for (int i = 0; i < pixelsCount; i++) {
			pixels [i].a = 0;
		}

		texture.SetPixels32 (pixels);
	}

	void FillTexture(SpriteRenderer sprite) {
		if (!ShouldIgnoreSprite (sprite)) {
			if (!childrenOverlap) {
				texture.SetPixels32 (GetOffsetX (sprite), GetOffsetY (sprite), sprite.sprite.texture.width, sprite.sprite.texture.height, sprite.sprite.texture.GetPixels32 ());
			} else {
				int width  = (int)sprite.sprite.rect.width;
				int height = (int)sprite.sprite.rect.height;

				Color[] pixels     = sprite.sprite.texture.GetPixels ((int)sprite.sprite.rect.x, (int)sprite.sprite.rect.y, width, height);
				int     pixelCount = pixels.Length;

				int offsetX = GetOffsetX (sprite);
				int offsetY = GetOffsetY (sprite);

				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						int index = width * y + x;

						if (pixels [index].a > 0) {
							texture.SetPixel (x + offsetX, y + offsetY, pixels [index]);
						}
					}
				}
			}

			if (!isIsometric && sprite.sortingOrder < _sortingSprite.sortingOrder) {
				_sortingSprite = sprite;
			}
		}

		if (!includeChildren)
			return;

		SpriteRendererExt.ForEachChild (sprite, childSprite => {
			FillTexture (childSprite);
			return true;
		});
	}

	int GetOffsetX(SpriteRenderer sprite) {
		return outlineSize + Mathf.RoundToInt ((sprite.bounds.min.x - _boundsMinX) * sprite.sprite.pixelsPerUnit);
	}

	int GetOffsetY(SpriteRenderer sprite) {
		return outlineSize + Mathf.RoundToInt ((sprite.bounds.min.y - _boundsMinY) * sprite.sprite.pixelsPerUnit);
	}

	void SortOutline() {
		if (!outlineSprite)
			return;

		outlineSprite.flipX = sprite.flipX;
		outlineSprite.flipY = sprite.flipY;

		SetSortOrderOffset (-1);
	}

	public void SetSortOrder(int sortOrder) {
		if (!outlineSprite)
			return;

		if (!isIsometric) {
			outlineSprite.sortingOrder            = sortOrder;
			outlineSprite.transform.localPosition = Vector3.zero;
		} else {
			outlineSprite.sortingOrder            = _sortingSprite.sortingOrder;
			outlineSprite.transform.localPosition = Vector3.forward * sortOrder;
		}
	}

	public void SetSortOrderOffset(int zOffset) {
		SetSortOrder (!isIsometric ? _sortingSprite.sortingOrder + zOffset : -zOffset);
	}

	public void Show() {
		if (outline) {
			outline.gameObject.SetActive (true);
		}
	}

	public void Hide() {
		if (outline) {
			outline.gameObject.SetActive (false);
		}
	}

	public void Clear() {
		if (!outline) {
			TryGetOutline ();

			if (!outline)
				return;
		}

		if (Application.isPlaying) {
			Destroy (outline);
		} else {
			DestroyImmediate (outline);
		}
	}

	public virtual bool ShouldIgnoreSprite(SpriteRenderer sprite) {
		return (!sprite.enabled || sprite == outlineSprite || (sprite.gameObject != gameObject && !LayerMaskExt.ContainsLayer (childLayers, sprite.gameObject.layer)));
	}

	#if UNITY_EDITOR
	bool _shouldRegenerate;
	bool _shouldClear;

	void OnValidate() {
		if (Application.isPlaying)
			return;

		if (!generatesOnStart) {
			_shouldRegenerate = true;
		} else {
			_shouldClear = true;
		}
	}

	void Update() {
		if (_shouldRegenerate) {
			Regenerate ();
			_shouldRegenerate = false;
		}

		if (_shouldClear) {
			Clear ();
			_shouldClear = false;
		}
	}
	#endif

}
