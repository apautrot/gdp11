public static class RandomInt
{
	public static System.Random random = new System.Random ();

	//! Range between min (included) and max (included).
	public static int Range ( int min, int max )
	{
		return random.Next ( min, max+1 );
	}
}


static class RandomBool
{
	private static System.Random rnd = new System.Random ();

	internal static bool Next ()
	{
		return ( ( rnd.Next () & 1 ) == 0 );
	}
}



public static class RandomFloat
{
	//! Range between min (included) and max (included).
	public static float Range ( float min, float max )				{ return random.Range ( min, max ); }
	public static float Range ( float min, float max, float step )	{ return random.Range ( min, max, step ); }

	internal static System.Random random = new System.Random ();

	public static float Range ( this System.Random random, float min, float max )
	{
		return (float)( ( random.NextDouble () * ( max - min ) ) + min );
	}

	public static float Range ( this System.Random random, float min, float max, float step )
	{
		float value = random.Range ( min, max );
		value = ( value - min ).RoundTo ( step ) + min;
		return value;
	}
}