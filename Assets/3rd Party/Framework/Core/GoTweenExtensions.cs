using UnityEngine;
using System.Collections.Generic;

enum RecurseOption
{
	SelfOnly,
	RecurseOnChildren
}

static class MaterialUtils
{
	internal static int _ColorNameId = Shader.PropertyToID ( "_Color" );
	internal static int _EmissionColorNameId = Shader.PropertyToID ( "_EmissionColor" );
}

static class GoTweenExtensions
{
	internal static GoTweenChain Start ( this GoTweenChain self )
	{
		Go.addTween ( self );
		return self;
	}

	internal static GoTweenFlow Start ( this GoTweenFlow self )
	{
		Go.addTween ( self );
		return self;
	}

	internal static AbstractGoTween delays ( this AbstractGoTween tween, float delay )
	{
		tween.delay = delay;
		return tween;
	}

	internal static AbstractGoTween loops ( this AbstractGoTween tween, int count )
	{
		tween.loopCount = count;
		return tween;
	}

	internal static AbstractGoTween loops ( this AbstractGoTween tween, int count, GoLoopType loopType )
	{
		tween.loopCount = count;
		tween.loopType = loopType;
		return tween;
	}

	internal static AbstractGoTween loopsInfinitely ( this AbstractGoTween tween )
	{
		tween.loopCount = -1;
		return tween;
	}

	internal static AbstractGoTween loopsInfinitely ( this AbstractGoTween tween, GoLoopType loopType )
	{
		tween.loopCount = -1;
		tween.loopType = loopType;
		return tween;
	}

	internal static GoTween eases ( this GoTween tween, GoEaseType ease )
	{
		tween.easeType = ease;
		return tween;
	}

	internal static AbstractGoTween updateTypeIs ( this AbstractGoTween tween, GoUpdateType updateType )
	{
		tween.updateType = updateType;
		return tween;
	}

	internal static List<GoTween> getTweens ( this GameObject self )
	{
		return Go.tweensWithTarget ( self );
	}

	internal static void killAllTweening ( this GameObject self, RecurseOption recurseOption = RecurseOption.SelfOnly )
	{
		killTweening ( self );
		killTweening ( self.transform );
		if ( self.GetComponent<Renderer>() != null )
			if ( self.GetComponent<Renderer>().material != null )
				killTweening ( self.GetComponent<Renderer>().material );

		if ( recurseOption == RecurseOption.RecurseOnChildren )
			for ( int i = 0 ; i < self.transform.childCount; i++ )
				killAllTweening ( self.transform.GetChild ( i ).gameObject, RecurseOption.RecurseOnChildren );
	}

	internal static void killTweening ( this GameObject self )
	{
		List<GoTween> tweens = Go.tweensWithTarget ( self );
		foreach ( GoTween tween in tweens )
		{
			tween.pause ();
			tween.destroy ();
		}
	}

	internal static void killTweening ( this Object self )
	{
		List<GoTween> tweens = Go.tweensWithTarget ( self );
		foreach ( GoTween tween in tweens )
		{
			tween.pause ();
			tween.destroy ();
		}
	}

	internal static void killTweening ( this Transform self )
	{
		List<GoTween> tweens = Go.tweensWithTarget ( self );
		foreach ( GoTween tween in tweens )
		{
			tween.pause ();
			tween.destroy ();
		}
	}

	internal static void killTweening ( this Material self )
	{
		List<GoTween> tweens = Go.tweensWithTarget ( self );
		foreach ( GoTween tween in tweens )
		{
			tween.pause ();
			tween.destroy ();
		}
	}

	internal static GoTweenChain animateJellyInfinite ( this Transform transform, float time, float value, GoEaseType ease = GoEaseType.QuadInOut )
	{
		GoTweenChain chain = new GoTweenChain ();

		Vector3 scaleAtStart = transform.localScale;
		Vector3 target = new Vector3 ( scaleAtStart.x - value, scaleAtStart.y + value, scaleAtStart.z );
		transform.localScale = target;

		target = new Vector3 ( scaleAtStart.x + value, scaleAtStart.y - value, scaleAtStart.z );
		chain.append ( transform.scaleTo ( time, target ).eases ( ease ) );

		target = new Vector3 ( scaleAtStart.x - value, scaleAtStart.y + value, scaleAtStart.z );
		chain.append ( transform.scaleTo ( time, target ).eases ( ease ) );

		chain.loopCount = -1;
		chain.play ();

		return chain;
	}

	internal static GoTweenChain animateJelly ( this Transform transform, float time, float value, int repeatCount = 1, GoEaseType ease = GoEaseType.QuadInOut )
	{
		Vector3 scaleAtStart = transform.localScale;

		GoTweenChain chain = new GoTweenChain ();
		for ( int i = repeatCount; i > 0; i-- )
		{
			float factor = ( (float)i ) / repeatCount;

			Vector3 target = new Vector3 ( scaleAtStart.x + ( value * factor ), scaleAtStart.y - ( value * factor ), scaleAtStart.z );
			chain.append ( transform.scaleTo ( time * factor, target ).eases ( ease ) );

			target = new Vector3 ( scaleAtStart.x - ( value * factor ), scaleAtStart.y + ( value * factor ), scaleAtStart.z );
			chain.append ( transform.scaleTo ( time * factor, target ).eases ( ease ) );
		}

		chain.append ( transform.scaleTo ( time / repeatCount, scaleAtStart ).eases ( ease ) );
		chain.play ();

		return chain;
	}

	internal static GoTween animateBounce ( this Transform transform, float time, float dx, float dy, float dz, int bounceCount = 1 )
	{
		GoTween tween = transform.localPositionTo ( time, new Vector3 ( dx, dy, dz ), true );
		tween.loopType = GoLoopType.PingPong;
		tween.loops ( bounceCount * 2 );
		return tween;
	}

	public static GoTween alphaTo ( this Material self, float duration, float endValue )
	{
		GoTween tween = self.alphaTo ( duration, endValue, MaterialUtils._ColorNameId, "_Color" );

		if ( self.HasProperty ( MaterialUtils._EmissionColorNameId ) )
			Go.to ( self, duration, new GoTweenConfig ().materialColor ( Color.black, "_EmissionColor" ) );

		return tween;
	}

	public static GoTween alphaTo ( this Material self, float duration, float endValue, int colorNameId, string colorName )
	{
#if UNITY_EDITOR
		if ( ! self.HasProperty ( colorNameId ) )
			Debug.LogWarning ( "Material " + self.name + " has no color named " + colorName, self );
#endif

		Color endColor = self.GetColor ( colorNameId );
		endColor.a = endValue;
		return Go.to ( self, duration, new GoTweenConfig ().materialColor ( endColor, colorName ) );
	}

	public static GoTween alphaFrom ( this Material self, float duration, float endValue, string colorName = "_Color" )
	{
#if UNITY_EDITOR
		if ( ! self.HasProperty ( colorName ) )
			Debug.LogWarning ( "Material " + self.name + " has no color named " + colorName, self );
#endif

		Color endColor = self.color;
		endColor.a = endValue;
		return Go.from ( self, duration, new GoTweenConfig ().materialColor ( endColor, colorName ) );
	}

	public static float getAlpha ( this Material self )
	{
		return self.color.a;
	}

	public static void SetColor ( this Material self, Color color )
	{
		self.SetColor ( "_Color", color );
	}

	public static GoTween alphaTo ( this SpriteRenderer self, float duration, float endValue, bool isRelative = false )
	{
		Color endColor = self.color;
		endColor.a = endValue;
		return Go.to ( self, duration, new GoTweenConfig ().colorProp ( "color", endColor, isRelative ) );
	}

	public static GoTween alphaFrom ( this SpriteRenderer self, float duration, float endValue, bool isRelative = false )
	{
		Color endColor = self.color;
		endColor.a = endValue;
		return Go.from ( self, duration, new GoTweenConfig ().colorProp ( "color", endColor, isRelative ) );
	}

	public static float getAlpha ( this SpriteRenderer self )
	{
		return self.color.a;
	}

	public static void setAlpha ( this SpriteRenderer self, float alpha )
	{
		Color c = self.color;
		c.a = alpha;
		self.color = c;
	}

// 	public static void setAlpha ( this Renderer self, float alpha )
// 	{
// 		self.material.SetAlpha ( alpha );
// 	}
// 
// 	public static GoTween alphaTo ( this Renderer self, float duration, float endValue, bool isRelative = false )
// 	{
// 		return self.material.alphaTo ( duration, endValue );
// 	}
// 
// 	public static GoTween alphaFrom ( this Renderer self, float duration, float endValue, bool isRelative = false )
// 	{
// 		return self.material.alphaFrom ( duration, endValue );
// 	}

	public static GoTween localRotationTo ( this Transform self, float duration, Quaternion endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().localRotation ( endValue, isRelative ) );
	}

	public static GoTween localRotationTo ( this Transform self, float duration, Vector3 endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().localRotation ( endValue, isRelative ) );
	}

	public static GoTween rotationTo ( this Transform self, float duration, Quaternion endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().rotation ( endValue, isRelative ) );
	}

	public static GoTween rotationTo ( this Transform self, float duration, Vector3 endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().rotation ( endValue, isRelative ) );
	}

	public static GoTween rotationTo ( this Transform self, float duration, float angle, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig ().rotation ( new Vector3 ( 0, 0, angle ), isRelative ) );
	}

	public static GoTween destroyOnEnd ( this GoTween self )
	{
		self.setOnCompleteHandler ( c => GameObject.Destroy ( self.target as Object ) );
		return self;
	}

	public static GoTween floatTo ( this Object self, string propertyName, float duration, float endValue, bool isRelative = false )
	{
		return Go.to ( self, duration, new GoTweenConfig().floatProp ( propertyName, endValue, isRelative ) );
	}

}