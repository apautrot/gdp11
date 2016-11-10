using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum PositionSpace
{
	Local,
	World
}

public enum GetChildOption
{
	ChildOnly,
	FullHierarchy
}

public enum GetBoundsOption
{
	ChildOnly,
	FullHierarchy
}

public enum FadeOutEndAction
{
	DoNothing,
	Inactive,
	Destroy
}

public enum ScaleOutEndAction
{
	DoNothing,
	Inactive,
	Destroy
}



static class EnumUtils
{
	static internal T GetRandomEnumValue<T> ()
	{
		var v = System.Enum.GetValues ( typeof ( T ) );
		return (T)v.GetValue ( RandomInt.Range ( 0, v.Length - 1 ) );
	}
}



public static class GameObjectUtils
{
	public static void BroadcastMessageToScene ( string messageName, System.Object messageParameter = null )
	{
		GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType ( typeof ( GameObject ) );
		foreach ( GameObject go in gos )
		{
			if ( go && go.transform.parent == null )
			{
				go.gameObject.BroadcastMessage ( messageName, messageParameter, SendMessageOptions.DontRequireReceiver );
			}
		}
	}

	public static List<T> FindObjectsOfTypeAs<T> () where T : class
	{
		List<T> result = new List<T> ();
		GameObject[] all = Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) ) as GameObject[];
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i].GetComponentAs<T> ();
			if ( t != null )
				result.Add ( t );
		}
		return result;
	}

	public static T FindObjectOfTypeAs<T> () where T : class
	{
		GameObject[] all = Resources.FindObjectsOfTypeAll ( typeof ( GameObject ) ) as GameObject[];
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i].GetComponentAs<T> ();
			if ( t != null )
				return t;
		}

		return null;
	}

}



public enum GetColliderOption
{
	Trigger,
	NonTrigger,
	Any
}

public static class GameObjectExtensions
{
	public static Collider GetCollider ( this GameObject self, GetColliderOption option = GetColliderOption.Any )
	{
		Collider[] colliders = self.GetComponents<Collider> ();
		for ( int i = 0; i < colliders.Length; i++ )
		{
			Collider collider = colliders[i];
			switch ( option )
			{
				case GetColliderOption.Trigger:
					if ( collider.isTrigger )
						return collider;
					break;

				case GetColliderOption.NonTrigger:
					if ( ! collider.isTrigger )
						return collider;
					break;

				default:
					return collider;
			}
		}
		
		return null;
	}

	public static void MoveChildTo ( this GameObject self, GameObject to )
	{
		List<Transform> childs = new List<Transform> ();
		for ( int i = 0; i < self.transform.childCount; i++ )
			childs.Add ( self.transform.GetChild ( i ) );

		foreach ( Transform child in childs )
			child.parent = to.transform;
	}

	public static T GetOrCreateComponent<T> ( this GameObject gameObject ) where T : Component
	{
		T component = gameObject.GetComponent<T> ();
		if ( component == null )
			component = gameObject.AddComponent<T> ();

		return component;
	}

	public static T GetComponentInParentHierarchy<T> ( this GameObject self ) where T : MonoBehaviour
	{
		GameObject parent = self.transform.parent.gameObject;
		if ( parent == null )
			return null;

		T t = parent.GetComponent<T> ();
		if ( t != null )
			return t;

		return parent.GetComponentAsInParentHierarchy<T> ();
	}

	public static T GetComponentAsInParentHierarchy<T> ( this GameObject self ) where T : class
	{
		GameObject parent = self.transform.parent.gameObject;
		if ( parent == null )
			return null;

		T t = parent.GetComponentAs<T> ();
		if ( t != null )
			return t;

		return parent.GetComponentAsInParentHierarchy<T> ();
	}

	public static T GetComponentOfChildUnderPoint<T> ( this GameObject gameObject, Vector2 point ) where T : MonoBehaviour
	{
		GameObject go = gameObject.GetChildUnderPoint ( point );
		if ( go != null )
			return go.GetComponent<T> ();
		else
			return null;
	}

	static public T FindComponentInChildren<T> ( this GameObject self, bool recursive = false ) where T : Component
	{
		T component = null;

		Transform transform = self.transform;
		int count = transform.childCount;

		for ( int i = 0; i < count; i++ )
		{
			GameObject gameObject = transform.GetChild ( i ).gameObject;
			
			component = gameObject.GetComponent<T> ();
			
			if ( ( component == null ) && recursive )
				component = gameObject.GetComponentInChildren<T> ();

			if ( component != null )
				break;
		}

		return component;
	}

	public static T GetComponentAs<T> ( this GameObject self ) where T : class
	{
		Component[] all = self.GetComponents<Component>();
		for ( int i = 0 ; i < all.Length; i++ )
		{
			if ( all[i] is T )
				return all[i] as T;
		}

		return null;
	}

	public static T[] GetComponentsAs<T> ( this GameObject self ) where T : class
	{
		List<T> result = new List<T> ();
		Component[] all = self.GetComponents<Component> ();
		for ( int i = 0; i < all.Length; i++ )
		{
			T t = all[i] as T;
			if ( t != null )
				result.Add ( t );
		}

		return result.ToArray();
	}

	public static GameObject GetChildUnderPoint ( this GameObject gameObject, Vector2 point )
	{
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D> ();
			if ( boxCollider2D != null )
			{
				Rect r = RectExtensions.FromCollider ( boxCollider2D );
				// Debug.Log ( "Rect : " + r.ToString() + " - Point : " + point.ToString() );
				if ( r.Contains ( point ) )
					return child;
			}
		}

		return null;
	}

	public static bool IsUnderPoint ( this GameObject gameObject, Vector2 point )
	{
		BoxCollider2D boxCollider2D = gameObject.GetComponent<BoxCollider2D> ();
		if ( boxCollider2D != null )
		{
			Rect r = RectExtensions.FromCollider ( boxCollider2D );
			return r.Contains ( point );
		}

		CircleCollider2D circleCollider2D = gameObject.GetComponent<CircleCollider2D> ();
		if ( circleCollider2D != null )
		{
			Vector2 dist = ( (Vector2)circleCollider2D.transform.position + circleCollider2D.offset ) - point;
			return ( dist.magnitude < circleCollider2D.radius );
		}

		Debug.LogWarning ( "IsUnderPoint : No collider on object " + gameObject.name );
		return false;
	}

	public static Bounds GetBounds ( this GameObject gameObject, GetBoundsOption option = GetBoundsOption.FullHierarchy )
	{
		if ( option == GetBoundsOption.ChildOnly )
		{
			if ( gameObject.GetComponent<Renderer>() != null )
				return gameObject.GetComponent<Renderer>().bounds;
			else
				return new Bounds ();
		}
		else
		{
			Bounds b = gameObject.GetChildBounds ();
			if ( gameObject.GetComponent<Renderer> () != null )
			{
				Bounds rb = gameObject.GetComponent<Renderer> ().bounds;
				if ( rb.size != Vector3.zero )
					b.Encapsulate ( rb );
			}

			return b;
		}
	}

	public static Bounds GetChildBounds ( this GameObject gameObject )
	{
		Bounds bounds = new Bounds ();
		if ( gameObject.transform.childCount > 0 )
		foreach ( Transform t in gameObject.transform )
		{
			Bounds childBound = t.gameObject.GetBounds ( GetBoundsOption.FullHierarchy );
			if ( childBound.size != Vector3.zero )
				if ( bounds.size == Vector3.zero )
					bounds = childBound;
				else
					bounds.Encapsulate ( childBound );
		}

 		if ( bounds.size == Vector3.zero )
 			return new Bounds ( gameObject.transform.position, Vector3.zero );

		return bounds;
	}

// 	public static Vector3 GetChildBounds ( this GameObject gameObject )
// 	{
// 		Vector3 max = new Vector3 ( float.MinValue, float.MinValue, float.MinValue );
// 		Vector3 min = new Vector3 ( float.MaxValue, float.MaxValue, float.MaxValue );
// 
// 		int count = gameObject.transform.childCount;
// 		for ( int i = 0; i < count; i++ )
// 		{
// 			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
// 			BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D> ();
// 			if ( boxCollider2D != null )
// 			{
// 				Vector3 position = child.transform.localPosition;
// 				float left = position.x - ( boxCollider2D.size.x / 2 ) + boxCollider2D.center.x;
// 				float right = position.x + ( boxCollider2D.size.x / 2 ) + boxCollider2D.center.x;
// 				float bottom = position.y - ( boxCollider2D.size.y / 2 ) + boxCollider2D.center.y;
// 				float top = position.y + ( boxCollider2D.size.y / 2 ) + boxCollider2D.center.y;
// 
// 				min.x = System.Math.Min ( min.x, left );
// 				max.x = System.Math.Max ( max.x, right );
// 				min.y = System.Math.Min ( min.y, bottom );
// 				max.y = System.Math.Max ( max.y, top );
// 			}
// 		}
// 
// 		return max - min;
// 	}

	public static void ForEachChildDo ( this GameObject self, System.Action<GameObject> action )
	{
		for ( int i = 0; i < self.transform.childCount; i++ )
			action ( self.transform.GetChild ( i ).gameObject );
	}

	public static List<GameObject> GetChilds<T> ( this GameObject gameObject, GetChildOption option = GetChildOption.ChildOnly ) where T : MonoBehaviour
	{
		List<GameObject> childs = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.GetComponent<T>() != null )
			{
				childs.Add ( child );
				if ( option == GetChildOption.FullHierarchy )
					childs.AddRange ( child.GetChilds ( option ) );
			}
		}
		return childs;
	}

	public static List<GameObject> GetChilds ( this GameObject gameObject, GetChildOption option = GetChildOption.ChildOnly )
	{
		List<GameObject> childs = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			childs.Add ( child );
			if ( option == GetChildOption.FullHierarchy )
				childs.AddRange ( child.GetChilds ( option ) );
		}
		return childs;
	}

	public static GameObject FindChildByName ( this GameObject gameObject, string name, bool errorIfNotFound = true )
	{
// 		if ( gameObject == null )
// 			throw new System.Exception ( "FindChildByName(\"" + name + "\") executed on null reference" );

		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
				return child;
		}

		if ( errorIfNotFound )
		{
			Debug.LogError ( "Child " + name + " not found" );
			Debug.Break ();
		}

		return null;
	}

	public static T FindChildByName<T> ( this GameObject gameObject, string name, bool errorIfNotFound = true ) where T : Component
	{
		// 		if ( gameObject == null )
		// 			throw new System.Exception ( "FindChildByName(\"" + name + "\") executed on null reference" );

		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
			{
				T t = child.GetComponent<T> ();
				if ( t != null )
					return t;

				if ( errorIfNotFound )
				{
					Debug.LogError ( "Child " + name + " found, but no component with given type" );
					Debug.Break ();
				}
			}
		}

		if ( errorIfNotFound )
		{
			Debug.LogError ( "Child " + name + " not found" );
			Debug.Break ();
		}

		return null;
	}

	public static List<GameObject> FindChildsByName ( this GameObject gameObject, string name )
	{
		List<GameObject> list = new List<GameObject> ();
		int count = gameObject.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = gameObject.transform.GetChild ( i ).gameObject;
			if ( child.name.Equals ( name ) )
				list.Add ( child );
		}
		return list;
	}

	public static T FindChildByComponent<T> ( this GameObject self ) where T : UnityEngine.Component
	{
		return self.GetComponentInChildren<T> ();
	}

	public static T[] FindChildsByComponent<T> ( this GameObject self, bool recursive = false, bool includeInactive = false ) where T : UnityEngine.Component
	{
		if ( recursive )
			return self.GetComponentsInChildren<T> ( includeInactive );
		else
		{
			List<T> list = new List<T> ();
			for ( int i = 0 ; i < self.transform.childCount; i++ )
			{
				T t = self.transform.GetChild ( i ).GetComponent<T> ();
				if ( ( t != null ) && ( includeInactive || t.gameObject.activeInHierarchy ) )
					list.Add ( t );
			}
			return list.ToArray ();
		}
	}

	public static GameObject GetRootParent ( this GameObject self )
	{
		Transform parent = self.transform.parent;
		
		if ( parent == null )
			return null;
	
		while ( parent.transform.parent != null )
			parent = parent.transform.parent;

		return parent.gameObject;
	}

	public static AbstractGoTween FadeIn ( this GameObject self, float duration = 0.5f, float alpha = 1, float delay = 0, GoEaseType ease = GoEaseType.Linear, bool fadeFromCurrentAlpha = false, bool fadeToCurrentAlpha = false )
	{
		if ( duration == 0 )
			Debug.LogWarning ( "Invalid parameter duration on FadeIn call" );

		if ( fadeToCurrentAlpha && fadeFromCurrentAlpha )
			Debug.LogWarning ( "Invalid parameters combination fadeToCurrentAlpha, fadeFromCurrentAlpha on FadeIn call" );

		self.SetActive ( true );
		if ( !fadeToCurrentAlpha && !fadeFromCurrentAlpha )
			self.SetAlpha ( 0, true );
		AbstractGoTween tween = 
			fadeToCurrentAlpha ?
			self.alphaFrom ( duration, 0, ease, delay )
		:	self.alphaTo ( duration, alpha, ease, delay );
		return tween;
	}

	public static AbstractGoTween FadeOut ( this GameObject self, float duration = 0.5f, FadeOutEndAction action = FadeOutEndAction.Destroy, float alpha = 0, float delay = 0, GoEaseType ease = GoEaseType.Linear )
	{
		if ( duration == 0 )
			Debug.LogWarning ( "Invalid parameter duration on FadeIn call" );

		AbstractGoTween tween = self.alphaTo ( duration, alpha, ease, delay );
		if ( tween != null )
		{
			switch ( action )
			{
				case FadeOutEndAction.DoNothing: break;
				case FadeOutEndAction.Inactive:
					tween.setOnCompleteHandler ( c => self.SetActive ( false ) );
					break;
				case FadeOutEndAction.Destroy:
					tween.setOnCompleteHandler ( c => self.DestroySelf () );
					break;
			}
		}
		else switch ( action )
			{
				case FadeOutEndAction.DoNothing: break;
				case FadeOutEndAction.Inactive:
					self.SetActive ( false );
					break;
				case FadeOutEndAction.Destroy:
					self.DestroySelf ();
					break;
			}
		return tween;
	}

	public static AbstractGoTween ScaleIn ( this GameObject self, float duration = 0.5f )
	{
		if ( duration == 0 )
			Debug.LogWarning ( "Invalid parameter duration on FadeIn call" );

		self.SetActive ( true );
		return self.transform.scaleFrom ( duration, 0 );
	}

	public static GoTween ScaleOut ( this GameObject self, float duration = 0.5f, ScaleOutEndAction action = ScaleOutEndAction.Destroy, float scale = 0, bool isRelative = false, GoEaseType ease = GoEaseType.Linear )
	{
		if ( duration == 0 )
			Debug.LogWarning ( "Invalid parameter duration on ScaleOut call" );

		GoTween tween = self.transform.scaleTo ( duration, scale, isRelative );
		if ( tween != null )
		{
			switch ( action )
			{
				case ScaleOutEndAction.DoNothing: break;
				case ScaleOutEndAction.Inactive:
					tween.setOnCompleteHandler ( c => self.SetActive ( false ) );
					break;
				case ScaleOutEndAction.Destroy:
					tween.setOnCompleteHandler ( c => self.DestroySelf () );
					break;
			}
			tween.eases ( ease );
		}
		else switch ( action )
			{
				case ScaleOutEndAction.DoNothing: break;
				case ScaleOutEndAction.Inactive:
					self.SetActive ( false );
					break;
				case ScaleOutEndAction.Destroy:
					self.DestroySelf ();
					break;
			}
		return tween;
	}

	public static void SetScale ( this GameObject self, float scale )
	{
		self.transform.localScale = new Vector3 ( scale, scale, scale );
	}

	public static void SetAlpha ( this GameObject self, float alpha, bool recursiveOnChildren = true )
	{
		if ( self.GetComponent<Renderer>() != null )
			if ( self.GetComponent<Renderer>().material != null )
				self.GetComponent<Renderer>().material.SetAlpha ( alpha );

		if ( recursiveOnChildren )
			for ( int i = 0; i < self.transform.childCount; i++ )
				self.transform.GetChild ( i ).gameObject.SetAlpha ( alpha, recursiveOnChildren );
	}

	public static void SetColor ( this GameObject self, Color color, bool recursiveOnChildren = true )
	{
		if ( self.GetComponent<Renderer>() != null )
			if ( self.GetComponent<Renderer>().material != null )
				self.GetComponent<Renderer>().material.SetColor ( color );

		if ( recursiveOnChildren )
			for ( int i = 0; i < self.transform.childCount; i++ )
				self.transform.GetChild ( i ).gameObject.SetColor ( color, recursiveOnChildren );
	}

	public static void SetSpriteColor ( this GameObject self, Color color )
	{
		SpriteRenderer sr = self.GetComponent<SpriteRenderer> ();
		if ( sr != null )
			sr.color = color;
	}

	public static AbstractGoTween colorTo ( this GameObject self, float duration, Color endValue, GoEaseType easeType = GoEaseType.Linear )
	{
		List<GameObject> all = self.GetChilds ( GetChildOption.FullHierarchy );
		all.Add ( self );
		AbstractGoTween tween = null;
		GoTweenFlow tweens = null;

		foreach ( var v in all )
		{
			if ( v.GetComponent<Renderer>() != null )
				if ( v.GetComponent<Renderer>().material != null )
				{
					if ( tween != null )
						if ( tweens == null )
						{
							tweens = new GoTweenFlow ();
							tweens.insert ( 0, tween );
						}

					tween = v.GetComponent<Renderer>().material.colorTo ( duration, endValue );
					if ( tweens != null )
						tweens.insert ( 0, tween );
				}
		}

		if ( tweens != null )
		{
			Go.addTween ( tweens );
			return tweens;
		}
		else
			return tween;
	}

	static AbstractGoTween alphaTween ( this GameObject self, bool directionTo, float duration, float endValue, GoEaseType easeType, float delay )
	{
		List<GameObject> all = self.GetChilds ( GetChildOption.FullHierarchy );
		all.Add ( self );
		AbstractGoTween tween = null;
		GoTweenFlow tweens = null;

		foreach ( var v in all )
		{
			Renderer r = v.GetComponent<Renderer>();
			if ( r != null )
			{
				Material m = r.material;
				if ( m != null )
				{
					if ( tween != null )
						if ( tweens == null )
						{
							tweens = new GoTweenFlow ();
							tweens.insert ( 0, tween );
						}

					if ( directionTo )
						tween = m.alphaTo ( duration, endValue ).eases ( easeType );
					else
						tween = m.alphaFrom ( duration, endValue ).eases ( easeType );

					if ( tweens != null )
						tweens.insert ( 0, tween );
				}
			}
		}

		if ( tweens != null )
		{
			tweens.delays ( delay );
			Go.addTween ( tweens );
			return tweens;
		}
		else
		{
			if ( tween != null )
				tween.delays ( delay );
			return tween;
		}
	}

	public static AbstractGoTween alphaFrom ( this GameObject self, float duration, float endValue, GoEaseType easeType = GoEaseType.Linear, float delay = 0 )
	{
		return self.alphaTween ( false, duration, endValue, easeType, delay );
	}

	public static AbstractGoTween alphaTo ( this GameObject self, float duration, float endValue, GoEaseType easeType = GoEaseType.Linear, float delay = 0 )
	{
		return self.alphaTween ( true, duration, endValue, easeType, delay );
	}

	public static void DestroyAllChilds ( this GameObject self )
	{
		int count = self.transform.childCount;
		for ( int i = 0; i < count; i++ )
		{
			GameObject child = self.transform.GetChild ( i ).gameObject;
			GameObject.Destroy ( child );
		}
	}

	public static void DestroyAllChilds<T> ( this GameObject self ) where T : MonoBehaviour
	{
		List<GameObject> list = self.GetChilds<T>();
		for ( int i = 0; i < list.Count; i++ )
			GameObject.Destroy ( list[i] );
	}

	public static bool DestroyChilds ( this GameObject self, string name )
	{
		List<GameObject> childs = self.FindChildsByName ( name );
		foreach ( GameObject child in childs )
			GameObject.Destroy ( child );

		return ( childs.Count > 0 );
	}

	public static bool DestroyChild ( this GameObject self, string name )
	{
		GameObject child = self.FindChildByName ( name );
		if ( child != null )
		{
			GameObject.Destroy ( child );
			return true;
		}
		else
			return false;
	}

	public static GameObject Instantiate ( GameObject prefab, Vector3 position, string name = null )
	{
		GameObject go = GameObject.Instantiate ( prefab ) as GameObject;
		go.transform.position = position;
		if ( name != null )
			go.name = name;
		return go;
	}

	public static GameObject InstantiateAtLocalPosition ( this GameObject self, GameObject prefab, string name = null )
	{
		return self.InstantiateAtLocalPosition ( prefab, Vector3.zero, name );
	}

	public static GameObject InstantiateAtLocalPosition ( this GameObject self, GameObject prefab, Vector3 position, string name = null )
	{
		GameObject child = GameObject.Instantiate ( prefab ) as GameObject;
		child.transform.position = self.transform.position + position;
		if ( name != null )
			child.name = name;
		return child;
	}

	public static SpriteRenderer InstantiateSprite ( Sprite sprite, Vector3 position, GameObject parent = null, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		GameObject spriteGO = new GameObject ();
		if ( parent != null )
		{
			spriteGO.transform.parent = parent.transform;
			if ( positionSpace == PositionSpace.Local )
				spriteGO.transform.localPosition = position;
			else
				spriteGO.transform.position = position;
		}
		else
			spriteGO.transform.position = position;

		if ( name != null )
			spriteGO.name = name;

		SpriteRenderer sr = spriteGO.GetOrCreateComponent<SpriteRenderer> ();
		if ( sprite == null )
			Debug.LogWarning ( "InstantiateSprite : Sprite parameter is null" );
		sr.sprite = sprite;
		return sr;
	}

	public static SpriteRenderer InstantiateSprite ( this GameObject self, Sprite sprite, Vector3 position, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		return InstantiateSprite ( sprite, position, self, positionSpace, name );
	}

	public static GameObject InstantiateSibling ( this GameObject self, GameObject prefab, string name = null )
	{
		return self.InstantiateSibling ( prefab, Vector3.zero, PositionSpace.Local, name );
	}

	public static GameObject InstantiateSibling ( this GameObject self, GameObject prefab, Vector3 position, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		if ( positionSpace == PositionSpace.Local )
			position += self.transform.position;

		Transform parent = self.transform.parent;
		if ( parent != null )
			return parent.gameObject.InstantiateChild ( prefab, position, PositionSpace.World, name );
		else
			return Instantiate ( prefab, position, name );
	}

	public static GameObject InstantiateChild ( this GameObject self, GameObject prefab, string name = null )
	{
		return self.InstantiateChild ( prefab, Vector3.zero/* prefab.transform.localPosition */, PositionSpace.Local, name );
	}

	public static GameObject InstantiateChild ( this GameObject self, GameObject prefab, Vector3 position, PositionSpace positionSpace = PositionSpace.Local, string name = null )
	{
		GameObject child = GameObject.Instantiate ( prefab ) as GameObject;
		child.transform.parent = self.transform;
		if ( positionSpace == PositionSpace.Local )
			child.transform.localPosition = position;
		else
			child.transform.position = position;
		if ( name != null )
			child.name = name;
		return child;
	}

	public static GameObject InstantiateReplace ( this GameObject self, GameObject prefab, bool inheritPosition = true, bool inheritRotation = true, bool inheritScale = false, bool inheritName = false, bool destroySelf = true, bool immediateDestroy = false )
	{
		GameObject replacer = GameObject.Instantiate ( prefab ) as GameObject;

		replacer.transform.parent = self.transform.parent;

		if ( inheritPosition )
			replacer.transform.localPosition = self.transform.localPosition;

		if ( inheritRotation )
			replacer.transform.localRotation = self.transform.localRotation;

		if ( inheritScale )
			replacer.transform.localScale = self.transform.localScale;

		if ( inheritName )
			replacer.name = self.name;

		if ( destroySelf )
			if ( immediateDestroy )
				GameObject.DestroyImmediate ( self );
			else
				self.DestroySelf ();

		return replacer;
	}

	public static void DestroySelf ( this GameObject self )
	{
		GameObject.Destroy ( self );
	}

	public static bool SendMessageToChild ( this GameObject self, string childName, string message )
	{
		GameObject child = self.FindChildByName ( childName );
		if ( child != null )
		{
			child.SendMessage ( message );
			return true;
		}
		else return false;
	}

	public static double DistanceTo ( this GameObject self, GameObject other )
	{
		return ( self.transform.position - other.transform.position ).magnitude;
	}

	public static double DistanceTo ( this GameObject self, Vector3 position )
	{
		return ( self.transform.position - position ).magnitude;
	}
}


public static class FloatExtensions
{
	public static float RoundTo ( this float value, float roundTo )
	{
		if ( roundTo < 0 )
			roundTo = -roundTo;

		if ( value < 0 )
			return -( -value ).RoundTo ( roundTo );

		if ( ( value % roundTo ) > ( roundTo / 2 ) )
			return Mathf.Ceil ( value / roundTo ) * roundTo;
		else
			return Mathf.Floor ( value / roundTo ) * roundTo;
	}

	public static bool Between ( this float self, float a, float b, bool inclusive = false )
	{
		if ( a > b )
		{
			float acopy = a;
			a = b;
			b = acopy;
		}

		return inclusive
			? a <= self && self <= b
			: a < self && self < b;
	}
}


public static class MaterialExtensions
{
	public static void SetAlpha ( this Material self, float alpha, string propertyName = "_Color" )
	{
		Color c = self.GetColor ( propertyName );
		c.a = alpha;
		// self.color = c;
		self.SetColor ( propertyName, c );
	}
}



public static class SpriteRendererExtensions
{
	public static GoTween colorTo ( this SpriteRenderer self, float duration, Color endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().colorProp ( "color", endValue, isRelative ) );
	}
}



public static class Physics2DExtensions
{
	public static void AddForce ( this Rigidbody2D rigidbody2D, Vector2 force, ForceMode mode = ForceMode.Force )
	{
		switch ( mode )
		{
			case ForceMode.Force:
				rigidbody2D.AddForce ( force );
				break;
			case ForceMode.Impulse:
				rigidbody2D.AddForce ( force / Time.fixedDeltaTime );
				break;
			case ForceMode.Acceleration:
				rigidbody2D.AddForce ( force * rigidbody2D.mass );
				break;
			case ForceMode.VelocityChange:
				rigidbody2D.AddForce ( force * rigidbody2D.mass / Time.fixedDeltaTime );
				break;
		}
	}

	public static void AddForce ( this Rigidbody2D rigidbody2D, float x, float y, ForceMode mode = ForceMode.Force )
	{
		rigidbody2D.AddForce ( new Vector2 ( x, y ), mode );
	}
}


public static class MonoBehaviorExtensions
{
	public static Coroutine WaitAndDo ( this MonoBehaviour self, float duration, System.Action action )
	{
		if ( duration != 0.0f )
			return self.StartCoroutine ( WaitDoCoroutine ( duration, action ) );
		else
		{
			action ();
			return null;
		}
	}

	private static System.Collections.IEnumerator WaitDoCoroutine ( float duration, System.Action action )
	{
		yield return new WaitForSeconds ( duration );
		action ();
		yield break;
	}
}


public static class ColorExtensions
{
	internal static Color WithAlphaAt ( this Color self, float alpha )
	{
		self.a = alpha;
		return self;
	}
}

public static class ColliderExtensions
{
	internal static Vector3 GetRandomPosition ( this BoxCollider2D self, PositionSpace space = PositionSpace.World )
	{
		Vector3 pos = ( space == PositionSpace.Local ) ?
						self.transform.localPosition
					: self.transform.position;

		pos += (Vector3)self.offset + new Vector3 ( RandomFloat.Range ( -self.size.x, self.size.x ) / 2, RandomFloat.Range ( -self.size.y, self.size.y ) / 2, 0 );

		return pos;
	}

	internal static Vector3 GetRandomPosition ( this CircleCollider2D self, PositionSpace space = PositionSpace.World )
	{
		Vector3 pos = ( space == PositionSpace.Local ) ?
						self.transform.localPosition
					: self.transform.position;

		Vector3 point = new Vector3 ( RandomFloat.Range ( -self.radius, self.radius ), 0, 0 );
		point = point.RotateZ ( RandomFloat.Range ( 0, 360 ) );

		pos += (Vector3)self.offset + point;

		return pos;
	}
}


public static class ListAndArrayExtensions
{
	internal static T GetRandomElement<T> ( this List<T> self )
	{
		int count = self.Count;
		if ( count == 0 )
			return default ( T );
		else
			return self[RandomInt.Range ( 0, count - 1 )];
	}

	internal static T GetRandomElement<T> ( this T[] self )
	{
		int count = self.Length;
		if ( count == 0 )
			return default ( T );
		else
			return self[RandomInt.Range ( 0, count - 1 )];
	}

	internal static bool GetRandomElementNotIn<T> ( this T[] self, IEnumerable<T> list, out T element )
	{
		int count = self.Length;
		int initialIndex = RandomInt.Range ( 0, count - 1 );
		int index = initialIndex;

		while ( true )
		{
			element = self[index];
			if ( !list.Contains<T> ( element ) )
				return true;

			index = ( index + 1 ) % count;
			if ( index == initialIndex )
				return false;
		}
	}

	public static bool Contains<T> ( this T[] self, T element )
	{
		return ( self.IndexOf ( element ) != -1 );
	}

	public static int IndexOf<T> ( this T[] self, T element )
	{
		for ( int i = 0; i < self.Length; i++ )
		{
			// if ( System.Nullable.GetUnderlyingType ( typeof ( T ) ) != null )
			if ( System.Object.Equals ( element, self[i] ) )
				// if ( element.Equals ( self[i] ) )
				return i;
		}

		return -1;
	}

	public static T[] Shuffle<T> ( this T[] self )
	{
		for ( int t = 0; t < self.Length; t++ )
		{
			T tmp = self[t];
			int r = Random.Range ( t, self.Length );
			self[t] = self[r];
			self[r] = tmp;
		}
		return self;
	}

	public static T[] Filter<T> ( this T[] self, System.Func<T, bool> filterFunc )
	{
		List<T> filtered = new List<T> ();
		for ( int i = 0; i < self.Length; i++ )
			if ( filterFunc ( self[i] ) )
				filtered.Add ( self[i] );

		return filtered.ToArray ();
	}

	public static void Remove<T> ( this List<T> list, T[] elements )
	{
		list.RemoveAll ( element => elements.Contains ( element ) );
	}

	public static void AddOrRemove<T> ( this List<T> list, T element )
	{
		int index = list.IndexOf ( element );
		if ( index != -1 )
			list.RemoveAt ( index );
		else
			list.Add ( element );
	}

	public delegate bool Equals<T> ( T x, T y );

	public static void AddOnlyOnce<T> ( this List<T> list, T element, Equals<T> comparer )
	{
		for ( int i = 0; i < list.Count; i++ )
		{
			T listElement = list[i];
			if ( comparer ( listElement, element ) )
				return;
		}

		list.Add ( element );
	}

	public static void AddRangeOnlyOnce<T> ( this List<T> list, IEnumerable<T> elements, Equals<T> comparer )
	{
		foreach ( T t in elements )
			list.AddOnlyOnce ( t, comparer );
	}

	public static bool AddOnlyOnce<T> ( this List<T> list, T element )
	{
		if ( !list.Contains ( element ) )
		{
			list.Add ( element );
			return true;
		}
		else
			return false;
	}

	public static void AddRangeOnlyOnce<T> ( this List<T> list, IEnumerable<T> elements )
	{
		foreach ( T t in elements )
			list.AddOnlyOnce ( t );
	}

	public static void AddMultiple<T> ( this List<T> list, params T[] elements )
	{
		for ( int i = 0; i < elements.Length; i++ )
			list.Add ( elements[i] );
	}

	public static T PopLast<T> ( this List<T> list )
	{
		int indexOfLast = list.Count - 1;
		if ( indexOfLast >= 0 )
		{
			T element = list[indexOfLast];
			list.RemoveAt ( indexOfLast );
			return element;
		}
		else
			return default ( T );
	}

	public static List<T> FilterElementsNotIn<T> ( this List<T> list, List<T> filteringList )
	{
		List<T> result = list;
		for ( int ui = 0; ui < filteringList.Count; ui++ )
		{
			int index = result.IndexOf ( filteringList[ui] );
			if ( index != -1 )
			{
				if ( result == list )
					result = new List<T> ( list );
				result.RemoveAt ( index );
			}
		}
		return result;
	}

	public static void Resize<T> ( this List<T> list, int sz, T c )
	{
		int cur = list.Count;
		if ( sz < cur )
			list.RemoveRange ( sz, cur - sz );
		else if ( sz > cur )
		{
			if ( sz > list.Capacity )//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
				list.Capacity = sz;
			for ( int i = 0; i < sz - cur; i++ )
				list.Add ( c );
		}
	}

	public static void Resize<T> ( this List<T> list, int sz ) where T : new()
	{
		Resize ( list, sz, new T () );
	}

	public static void Shuffle<T> ( this IList<T> list )
	{
		int n = list.Count;
		while ( n > 1 )
		{
			n--;
			int k = RandomInt.Range ( 0, n );
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}