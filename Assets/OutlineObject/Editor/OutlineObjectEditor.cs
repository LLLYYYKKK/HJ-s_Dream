using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OutlineObject))]
public class OutlineObjectEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		EditorGUILayout.Space ();

		GUIStyle hr = new GUIStyle (GUI.skin.box);

		hr.border.top    = 0;
		hr.border.bottom = 0;
		hr.margin.top    = 0;
		hr.margin.bottom = 8;
		hr.stretchWidth  = true;
		hr.fixedHeight   = 1;

		Color originalColor = GUI.color;

		GUI.color = Color.black;
		GUILayout.Box ("", hr);
		GUI.color = originalColor;

		GUILayout.Label ("Outline Actions:");

		if (GUILayout.Button ("Regenerate")) {
			OutlineObject outline = (OutlineObject)target;
			outline.Regenerate ();
		}

		if (GUILayout.Button ("Clear")) {
			OutlineObject outline = (OutlineObject)target;
			outline.Clear ();
		}

		EditorGUILayout.Space ();
	}

}
