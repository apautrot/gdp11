#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

public static class GameObjectEditorExtensions
{
	public static void SetAssetLabel ( this GameObject self, string label )
	{
		if ( UnityEditor.PrefabUtility.GetPrefabType ( self ) != UnityEditor.PrefabType.Prefab )
			return;

		string[] labels = UnityEditor.AssetDatabase.GetLabels ( self );
		if ( !labels.Contains ( label ) )
		{
			List<string> labelList = new List<string> ( labels );
			labelList.Add ( label );
			UnityEditor.AssetDatabase.SetLabels ( self, labelList.ToArray () );
			UnityEditor.EditorUtility.SetDirty ( self );
			UnityEngine.Debug.LogWarning ( "Forcing Label On Object: " + label + " - " + self );
		}
	}

	public static void SetAssetBundle ( this GameObject self, string label )
	{
		if ( UnityEditor.PrefabUtility.GetPrefabType ( self ) != UnityEditor.PrefabType.Prefab )
			return;

		var path = UnityEditor.AssetDatabase.GetAssetPath ( self );
		var importer = UnityEditor.AssetImporter.GetAtPath ( path );
		if ( importer && importer.assetBundleName != label )
			importer.assetBundleName = label;
	}

	public static bool IsPrefab ( this GameObject self )
	{
		return ( PrefabUtility.GetPrefabType ( self ) == PrefabType.Prefab );
	}

	public static bool IsPrefabInstance ( this GameObject self )
	{
		return ( PrefabUtility.GetPrefabType ( self ) == PrefabType.PrefabInstance );
	}

	public static void SetDirty ( this GameObject self )
	{
		EditorUtility.SetDirty ( self );
	}

	public static string GetTypeDescription ( this Object self )
	{
		GameObject targetAsGameObject = self as GameObject;
		if ( targetAsGameObject != null )
		{
			if ( targetAsGameObject.IsPrefab () )
				return "Prefab";
			else if ( targetAsGameObject.IsPrefabInstance() )
				return "Prefab instance";
			else
				return "Game object";
		}

		MonoScript targetAsMonoScript = self as MonoScript;
		if ( targetAsMonoScript != null )
		{
			return "Script";
		}

		Component targetAsComponent = self as Component;
		if ( targetAsComponent != null )
		{
			GameObject gameObject = targetAsComponent.gameObject;
			return "Component of " + gameObject.GetTypeDescription () + " " + gameObject.name;
		}

//		Debug.Log ( AssetDatabase.GetAssetOrScenePath ( self ) );

		// return self.GetType().Name;
		return self.GetType ().Name;
	}

	public static void BroadcastEditorMessageToScene ( string messageName, System.Object[] parameters = null )
	{
		MonoBehaviour[] all = Resources.FindObjectsOfTypeAll<MonoBehaviour> ();
		for ( int i = 0; i < all.Length; i++ )
		{
			MonoBehaviour mb = all[i];

			if ( mb.IsSceneObject () )
			{
				System.Reflection.MethodInfo method = mb.GetType ().GetMethod ( messageName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
				if ( method != null )
					method.Invoke ( mb, parameters );
			}
		}
	}

	public static List<GameObject> FindByPath ( string path )
	{
		List<GameObject> result = new List<GameObject> ();
		string[] names = path.Split ( new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries );

		if ( names.Length > 0 )
		{
			GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject> ();
			for ( int i = 0; i < allGameObjects.Length; i++ )
			{
				GameObject gameObject = allGameObjects[i];
				if ( gameObject.transform.parent == null )
					if ( gameObject.name == names[0] )
						if ( gameObject.IsSceneObject () )
							if ( names.Length == 1 )
								result.Add ( gameObject );
							else
								FindByPath ( gameObject, names, 1, result );
			}
		}

		return result;
	}

	public static void FindByPath ( GameObject gameObject, string[] names, int nameIndex, List<GameObject> result )
	{
		string name = names[nameIndex];
		List<GameObject> childsByName = gameObject.FindChildsByName ( name );

		if ( childsByName.Count > 0 )
		{
			if ( nameIndex == names.Length - 1 )
				result.AddRange ( childsByName );
			else
				for ( int i = 0; i < childsByName.Count; i++ )
					FindByPath ( childsByName[i], names, nameIndex + 1, result );
		}
	}

	public static bool IsSceneObject ( this Object target )
	{
		if ( target == null )
			return false;

		GameObject asGameObject = target as GameObject;
		if ( asGameObject != null )
			if ( !asGameObject.IsPrefab () )
				return true;

		Component asComponent = target as Component;
		if ( asComponent != null )
			if ( !asComponent.gameObject.IsPrefab () )
				return true;

		if ( AssetDatabase.Contains ( target ) )
			if ( AssetDatabase.GetAssetPath ( target ).Length == 0 )
				return true;

		return false;
	}

	public static string GetHierarchyPath ( this GameObject self )
	{
		StringBuilder path = new StringBuilder ();
		Transform transform = self.transform;
		while ( transform != null )
		{
			if ( path.Length > 0 )
			{
				path.Insert ( 0, "." );
				path.Insert ( 0, transform.name );
			}
			else
				path.Append ( transform.name );

			transform = transform.parent;
		}

		return path.ToString ();
	}
}


public static class TypeExtensions
{
	public static MethodInfo GetMethodRecursive ( this System.Type self, string name, BindingFlags bindingFlags )
	{
		MethodInfo method = self.GetMethod ( name, bindingFlags );
		if ( method == null )
		{
			System.Type baseType = self.BaseType;
			if ( baseType != null )
				return baseType.GetMethodRecursive ( name, bindingFlags );
		}

		return method;
	}
}


#endif