
public class NakedSingleton<T> : ISingleton where T : ISingleton, new ()
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if ( instance == null )
				CreateInstance ();
			return instance;
		}
	}

	internal static bool InstanceCreated
	{
		get { return instance != null; }
	}

	internal static void CreateInstance ()
	{
		instance = new T ();
		instance.OnInstanceCreated ();
	}

	internal static void DestroyInstance()
	{
		instance = default(T);
	}

	public virtual void OnInstanceCreated ()
	{
	}
}
