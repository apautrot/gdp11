using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor ( typeof ( GatesSetup ) )]
class GatesSetupInspector : Editor
{
	protected SerializedProperty spawnLists;

	protected virtual void OnEnable ()
	{
		spawnLists = serializedObject.FindProperty ( "SpawnLists" );
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		serializedObject.Update ();

		int spawnListCount = spawnLists.arraySize;
		for ( int i = 0; i < spawnListCount; i++ )
		{
			SerializedProperty spawnList = spawnLists.GetArrayElementAtIndex ( i );
			{
				SerializedProperty name = spawnList.FindPropertyRelative ( "Name" );
				SerializedProperty items = spawnList.FindPropertyRelative ( "Items" );
				
				EditorGUILayout.BeginVertical ( EditorStyles.helpBox );
				// GUILayout.Space ( 10 );
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if ( GUILayout.Button ( "x", GUILayout.Width ( 20 ), GUILayout.Height ( 16 ) ) )
				{
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.PropertyField ( name );
	 			int itemCount = items.arraySize;
 				for ( int j = 0; j < itemCount; j++ )
	 			{
	 				SerializedProperty spawnItem = items.GetArrayElementAtIndex ( j );
	 				SerializedProperty chance = spawnItem.FindPropertyRelative ( "Chance" );
	 				SerializedProperty objectType = spawnItem.FindPropertyRelative ( "ObjectType" );

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space ( 120 );
					EditorGUILayout.PropertyField ( objectType, GUIContent.none );
					EditorGUILayout.PropertyField ( chance, GUIContent.none, GUILayout.Width ( 40 ) );
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUILayout.BeginHorizontal ();
				// GUILayout.FlexibleSpace ();
				if ( GUILayout.Button ( "Add item", GUILayout.Width ( 100 ) ) )
				{
					items.InsertArrayElementAtIndex ( items.arraySize );
				}
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space ( 10 );
				EditorGUILayout.EndVertical ();
			}
		}

		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if ( GUILayout.Button ( "Add gate", GUILayout.Width ( 150 ) ) )
		{
			spawnLists.InsertArrayElementAtIndex ( spawnLists.arraySize );
		}
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();

		if ( GUI.changed )
			serializedObject.ApplyModifiedProperties ();
	}

}
