using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

class ReorderableListEx : ReorderableList
{
	protected Color selectedColor = Color.Lerp ( Color.white, Color.yellow, 0.20f );

	public delegate void NewElement ( SerializedProperty newElement );
	public NewElement onNewElement = null;

	public delegate void AddElementFromSelection ( GameObject gameObject, SerializedProperty newElement );
	private AddElementFromSelection _onAddElementFromSelection;
	public AddElementFromSelection onAddElementFromSelection
	{
		get { return _onAddElementFromSelection; }
		set
		{
			_onAddElementFromSelection = value;

			if ( _onAddElementFromSelection != null )
			{
				onAddCallback = ( ReorderableList l ) =>
				{
					if ( Selection.gameObjects.Length == 0 )
					{
						ReorderableList.defaultBehaviours.DoAddButton ( l );
						if ( onNewElement != null )
						{
							SerializedProperty newElement = l.serializedProperty.GetArrayElementAtIndex ( l.serializedProperty.arraySize - 1 );
							onNewElement ( newElement );
						}
					}
					else
					{
						AddObjectArray ( Selection.gameObjects );
					}
				};
			}
			else
				onAddCallback = null;
		}
	}

	internal void AddObjectArray ( GameObject[] objects )
	{
		for ( int i = 0; i < objects.Length; i++ )
		{
			int newIndex = serializedProperty.arraySize;
			serializedProperty.arraySize++;
			index = newIndex;

			SerializedProperty newElement = serializedProperty.GetArrayElementAtIndex ( index );
			GameObject gameObject = objects[i];

			_onAddElementFromSelection ( gameObject, newElement );

			if ( onNewElement != null )
				onNewElement ( newElement );
		}
	}

	private void Initialize ( string displayName )
	{
		drawHeaderCallback = ( Rect rect ) =>
		{
			EditorGUI.LabelField ( rect, displayName != null ? displayName : serializedProperty.displayName );

			Rect clrRect = new Rect ( rect.x + rect.width - 50, rect.y, 50, rect.height );
			if ( GUI.Button ( clrRect, "Clear" ) )
				serializedProperty.ClearArray ();

			HandleDropOnHeader ( rect );
		};

		drawElementBackgroundCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
		{
			if ( Event.current.type == EventType.Repaint )
			{
 				GUIStyle elementBackground = new GUIStyle ( "RL Element" );
				// Rect backRect = new Rect ( rect.x + 2, rect.y + 2, 4, rect.height - 4 );
				Rect backRect = new Rect ( rect.x + 3, rect.y + 6, 14, rect.height - 12 );
				elementBackground.Draw ( backRect, false, isActive, isActive, isFocused );
				// EditorGUI.DrawRect ( rect, selectedColor );
			}
		};

		drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) =>
		{
 			rect.yMin += 3;
 			rect.yMax -= 3;

 	  		SerializedProperty element = serializedProperty.GetArrayElementAtIndex ( index );
 	   		EditorGUI.PropertyField ( rect, element, GUIContent.none, true );
		};

		onAddCallback = ( ReorderableList list ) =>
		{
			defaultBehaviours.DoAddButton ( list );

			if ( onNewElement != null )
			{
				SerializedProperty newElement = list.serializedProperty.GetArrayElementAtIndex ( list.serializedProperty.arraySize - 1 );
				onNewElement ( newElement );
			}
		};
	}

	public ReorderableListEx ( SerializedProperty serializedProperty, string displayName = null )
		: base ( serializedProperty.serializedObject, serializedProperty, true, true, true, true )
	{
		Initialize ( displayName );
	}

	public ReorderableListEx ( SerializedObject serializedObject, string propertyPath, string displayName = null )
		: base ( serializedObject, serializedObject.FindProperty ( propertyPath ) )
	{
		Initialize ( displayName );
	}

	public void HandleDropOnHeader ( Rect rect )
	{
		// from https://gist.github.com/bzgeb/3800350

		Event evt = Event.current;
		switch ( evt.type )
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if ( !rect.Contains ( evt.mousePosition ) )
					return;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if ( evt.type == EventType.DragPerform )
				{
					DragAndDrop.AcceptDrag ();

					List<GameObject> objects = new List<GameObject> ();
					foreach ( Object draggedObject in DragAndDrop.objectReferences )
					{
						GameObject go = draggedObject as GameObject;
						if ( go != null )
							objects.Add ( go );
					}

					if ( objects.Count > 0 )
						AddObjectArray ( objects.ToArray() );
				}
				break;
		}
	}

	/*
	example of added zone at top of list
	public void DropAreaGUI ()
	{
		Rect dropArea = GUILayoutUtility.GetRect ( 0.0f, 20, GUILayout.ExpandWidth ( true ) );
		GUI.Box ( dropArea, "Add Trigger" );

		HandleDropOnHeader ( dropArea );
	}
	*/
}
