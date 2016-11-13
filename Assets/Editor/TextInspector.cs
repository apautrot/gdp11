using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor ( typeof ( TextMeshEx ) ), CanEditMultipleObjects]
public class TextInspector : Editor
{
	[MenuItem ( "GameObject/Create Other/3D Text (extended)", false, 5 )]
	public static void CreateTextMeshEx ()
	{
		GameObject go = new GameObject();
		go.name = "Text";

		TextMeshEx textMesh = go.GetOrCreateComponent<TextMeshEx> ();
		textMesh.Font = Resources.GetBuiltinResource ( typeof ( Font ), "Arial.ttf" ) as Font;

		if ( Selection.activeGameObject != null )
		{
			go.transform.parent = Selection.activeGameObject.transform;
			go.transform.position = Selection.activeGameObject.transform.position;
		}
		Selection.activeObject = go;
	}

	public override void OnInspectorGUI ()
	{
		this.DrawDefaultInspector ();

		UnityToolbag.SortingLayerExposedEditor.DrawSortingLayerGUI ( target, targets );

		// http://twistedoakstudios.com/blog/Post373_unity-font-extraction
	}
}