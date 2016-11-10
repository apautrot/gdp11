using UnityEngine;
using System.Collections;

static class RectExtensions
{
	internal static Rect FromCollider ( BoxCollider2D collider )
	{
		float scaleX = 1;
		float scaleY = 1;

		if ( collider.transform.parent != null )
		{
			scaleX = collider.transform.parent.lossyScale.x;
			scaleY = collider.transform.parent.lossyScale.y;
		}

		float sizeX = collider.size.x * scaleX;
		float sizeY = collider.size.y * scaleY;
		float centerX = collider.offset.x * scaleX;
		float centerY = collider.offset.y * scaleY;

		float left = collider.transform.position.x - ( sizeX / 2 ) + centerX;
		float bottom = collider.transform.position.y - ( sizeY / 2 ) + centerY;
		return new Rect ( left, bottom, sizeX, sizeY );
	}

	public static bool IntersectsLine ( this Rect r, Vector2 p1, Vector2 p2 )
	{
		return LineIntersectsLine ( p1, p2, new Vector2 ( r.x, r.y ), new Vector2 ( r.x + r.width, r.y ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x + r.width, r.y ), new Vector2 ( r.x + r.width, r.y + r.height ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x + r.height, r.y + r.height ), new Vector2 ( r.x, r.y + r.height ) ) ||
			   LineIntersectsLine ( p1, p2, new Vector2 ( r.x, r.y + r.height ), new Vector2 ( r.x, r.y ) ) ||
			   ( r.Contains ( p1 ) && r.Contains ( p2 ) );
	}

	private static bool LineIntersectsLine ( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4 )
	{

		Vector2 a = p2 - p1;
		Vector2 b = p3 - p4;
		Vector2 c = p1 - p3;

		float alphaNumerator = b.y * c.x - b.x * c.y;
		float alphaDenominator = a.y * b.x - a.x * b.y;
		float betaNumerator = a.x * c.y - a.y * c.x;
		float betaDenominator = a.y * b.x - a.x * b.y;

		bool doIntersect = true;

		if ( alphaDenominator == 0 || betaDenominator == 0 )
			doIntersect = false;
		else
		{

			if ( alphaDenominator > 0 )
			{
				if ( alphaNumerator < 0 || alphaNumerator > alphaDenominator )
					doIntersect = false;
			}
			else if ( alphaNumerator > 0 || alphaNumerator < alphaDenominator )
				doIntersect = false;

			if ( doIntersect && betaDenominator > 0 )
			{
				if ( betaNumerator < 0 || betaNumerator > betaDenominator )
					doIntersect = false;
			}
			else if ( betaNumerator > 0 || betaNumerator < betaDenominator )
				doIntersect = false;
		}

		return doIntersect;
	}
}
