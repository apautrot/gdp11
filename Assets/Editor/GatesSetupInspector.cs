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
		serializedObject.Update ();

		int spawnListIndexToDelete = -1;
		int spawnListIndexToDuplicate = -1;
		int spawnListCount = spawnLists.arraySize;
		for ( int i = 0; i < spawnListCount; i++ )
		{
			SerializedProperty spawnList = spawnLists.GetArrayElementAtIndex ( i );
			{
				SerializedProperty name = spawnList.FindPropertyRelative ( "Name" );
				SerializedProperty items = spawnList.FindPropertyRelative ( "Items" );
				
				EditorGUILayout.BeginVertical ( EditorStyles.helpBox );
				GUILayout.Space ( 10 );
				EditorGUILayout.BeginHorizontal ();
				string labelName = name.stringValue; if ( labelName.Length == 0 ) labelName = "?";
				EditorGUILayout.PropertyField ( name, new GUIContent ( labelName ) );
				if ( GUILayout.Button ( "D", GUILayout.Width ( 20 ), GUILayout.Height ( 16 ) ) )
					spawnListIndexToDuplicate = i;
				if ( GUILayout.Button ( "x", GUILayout.Width ( 20 ), GUILayout.Height ( 16 ) ) )
					spawnListIndexToDelete = i;
				EditorGUILayout.EndHorizontal ();

				int itemIndexToDelete = -1;
	 			int itemCount = items.arraySize;
 				for ( int j = 0; j < itemCount; j++ )
	 			{
	 				SerializedProperty spawnItem = items.GetArrayElementAtIndex ( j );
	 				SerializedProperty chance = spawnItem.FindPropertyRelative ( "Chance" );
	 				SerializedProperty objectType = spawnItem.FindPropertyRelative ( "ObjectType" );

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space ( EditorGUIUtility.labelWidth );
					EditorGUILayout.PropertyField ( objectType, GUIContent.none );
					EditorGUILayout.PropertyField ( chance, GUIContent.none, GUILayout.Width ( 40 ) );
					if ( GUILayout.Button ( "x", GUILayout.Width ( 20 ), GUILayout.Height ( 16 ) ) )
					{
						itemIndexToDelete = j;
					}
					EditorGUILayout.EndHorizontal ();
				}
				if ( itemIndexToDelete != -1 )
				{
					items.DeleteArrayElementAtIndex ( itemIndexToDelete );
					serializedObject.ApplyModifiedProperties ();
				}

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space ( EditorGUIUtility.labelWidth );
				GUILayout.FlexibleSpace ();
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
		if ( spawnListIndexToDuplicate != -1 )
		{
			spawnLists.InsertArrayElementAtIndex ( spawnListIndexToDuplicate );
			serializedObject.ApplyModifiedProperties ();
		}
		if ( spawnListIndexToDelete != -1 )
		{
			spawnLists.DeleteArrayElementAtIndex ( spawnListIndexToDelete );
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if ( GUILayout.Button ( "Add spawn list", GUILayout.Width ( 150 ) ) )
		{
			spawnLists.InsertArrayElementAtIndex ( spawnLists.arraySize );
		}
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();

		if ( GUI.changed )
			serializedObject.ApplyModifiedProperties ();
	}

}
