using UnityEngine;
using System.Collections;


// à voir : http://wiki.unity3d.com/index.php/AutoSingletonManager

/*
public interface ISingletonType
{
}

public interface SingletonAutoCreated : ISingletonType
{
}

public interface SingletonFoundInScene : ISingletonType
{
}

public class SingletonEx<T,SingletonOption> : MonoBehaviour, ISingleton
	where T : MonoBehaviour, ISingleton where SingletonOption : ISingletonType
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				if ( typeof ( SingletonOption ) == typeof ( SingletonFoundInScene ) )
				{
					instance = GameObject.FindObjectOfType<T> () as T;
					if ( instance == null )
						throw new System.Exception ( "Scene singleton " + typeof ( T ).ToString () + " not found in scene." );
				}
				else if ( typeof ( SingletonOption ) == typeof ( SingletonAutoCreated ) )
				{
					instance = GameObject.FindObjectOfType<T> () as T;
					if ( instance == null )
					{
						GameObject singletonManagerObject = GameObject.Find ( "Singletons" );
						if ( singletonManagerObject == null )
						{
							singletonManagerObject = new GameObject ();
							singletonManagerObject.name = "Singletons";
						}

						instance = singletonManagerObject.AddComponent<T> ();
					}
				}
			}
			return instance;
		}
	}

	public static bool IsInstanceCreated ( bool autoCreate = true )
	{
		if ( instance == null )

		return ( instance != null );
	}

	void Awake ()
	{
		if ( instance != null )
		{
			if ( instance == this )
				Debug.LogWarning ( "Singleton " + typeof ( T ).ToString () + " has been accessed before being awake !" );
			else
				Debug.LogError ( "Awaking " + name + ", instance of " + typeof ( T ).ToString () + ", which is duplicate of singleton instance " + instance.name );
		}
		else
			instance = this as T;

		OnSingletonCreated ();
	}

	void OnDestroy ()
	{
		if ( instance != this )
			Debug.LogWarning ( "Destroying " + name + ", instance of " + typeof(T).ToString() + ", that is a duplicate singleton instance" );

		instance = null;
	}

	public virtual void OnSingletonCreated ()
	{
	}
}

*/
















public interface ISingleton
{
	void OnInstanceCreated ();
}


public class Singleton<T> : MonoBehaviour, ISingleton
	where T : MonoBehaviour, ISingleton
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = GameObject.FindObjectOfType<T> () as T;
				if ( instance == null )
				{
					GameObject singletonManagerObject = GameObject.Find ( "Singletons" );
					if ( singletonManagerObject == null )
					{
						singletonManagerObject = new GameObject ();
						singletonManagerObject.name = "Singletons";
					}

					instance = singletonManagerObject.GetOrCreateComponent<T> ();

					if ( isShuttingDown && Application.isPlaying )
						Debug.LogError ( "Creating an new singleton instance while quitting application !" );

					// Debug.Log ( "Created an instance of " + typeof ( T ).Name );
					// instance.name = typeof ( T ).Name;
				}

				// MonoBehaviour mb = (MonoBehaviour)instance;
				// GameObject.DontDestroyOnLoad ( mb.gameObject );
				instance.OnInstanceCreated ();
			}
			return instance;
		}
	}

	internal static T CreateInstance ()
	{
		if ( InstanceCreated )
			throw new System.Exception ( "Instance of " + typeof ( T ).Name + " already created" );

		return Instance;
	}

	internal static bool InstanceCreated
	{
		get { return instance != null; }
	}

	protected void OnDestroy ()
	{
		if ( this == instance )
			instance = null;
	}

	public virtual void OnInstanceCreated ()
	{
	}

	internal static bool isShuttingDown;

	void OnApplicationQuit ()
	{
// 		if ( !isShuttingDown )
// 		{
// 			Debug.Log ( "Detecting application shutdown" );
			isShuttingDown = true;
//		}
	}
}