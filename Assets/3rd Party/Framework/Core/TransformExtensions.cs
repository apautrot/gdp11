using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions
{
	public static Transform[] FindAll ( this Transform self, string name )
	{
		List<Transform> list = new List<Transform>();
		int count = self.childCount;
		for ( int i = 0; i < count; i++ )
		{
			Transform t = self.GetChild ( i );
			if ( t.name == name )
				list.Add ( t );
		}

		return list.ToArray ();
	}

	public static void SetScale ( this Transform self, float scale )
	{
		self.localScale = new Vector3 ( scale, scale, scale );
	}

	public static void ScaleBy ( this Transform self, float scaleBy )
	{
		self.localScale *= scaleBy;
	}

	public static void SetXY ( this Transform self, float x, float y )
	{
		self.transform.position = new Vector3 ( x, y, self.transform.position.z );
	}
	public static double DistanceTo ( this Transform self, GameObject other )
	{
		return ( self.position - other.transform.position ).magnitude;
	}

	public static double DistanceTo ( this Transform self, Vector3 position )
	{
		return ( self.position - position ).magnitude;
	}

	public static int GetHierarchyLevel ( this Transform self )
	{
		int level = 0;
		while ( self != null )
		{
			self = self.parent;
			level++;
		}
		return ( level - 1 );
	}
}
