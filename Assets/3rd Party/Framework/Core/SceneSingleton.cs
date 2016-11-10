using UnityEngine;
using System.Collections;









public class SceneSingleton<T> : MonoBehaviour, ISingleton
 	where T : MonoBehaviour, ISingleton
{
	protected static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = GameObject.FindObjectOfType<T> () as T;
				if ( instance == null )
					throw new System.Exception ( "A scene singleton of type " + typeof ( T ).ToString () + " was not found in scene." );

				// Debug.LogWarning ( "A scene singleton of type " + typeof ( T ).ToString () + " was accessed in scene before it has been enabled." );
				instance.OnInstanceCreated ();
			}
			return instance;
		}
	}

	public static bool InstanceCreated
	{
		get
		{
			if ( instance == null )
			{
				instance = GameObject.FindObjectOfType<T> () as T;
				if ( instance != null )
				{
					// Debug.LogWarning ( "A scene singleton of type " + typeof ( T ).ToString () + " was accessed in scene before it has been enabled." );
					instance.OnInstanceCreated ();
				}
			}

			return instance != null;
		}
	}

	protected void Awake ()
	{
		// Debug.Log ( "Scene singleton Awake for type " + typeof ( T ).ToString () );

		if ( instance == null )
		{
			instance = this as T;
			instance.OnInstanceCreated ();
		}

		if ( instance != this )
			Debug.LogError ( "A duplicate scene singleton of type " + typeof ( T ).ToString () + " has been enabled. Duplicate name : " + this.name + ", already present in " + instance.name );
	}

	protected void OnDestroy ()
	{
		if ( this == instance )
			instance = null;
	}

	public virtual void OnInstanceCreated ()
	{
	}
}