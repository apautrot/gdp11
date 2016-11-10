using UnityEngine;
using System.Collections;


public class ScaleTweenProperty : AbstractVector3TweenProperty
{
	public ScaleTweenProperty( Vector3 endValue, bool isRelative = false ) : base( endValue, isRelative )
	{}
	
	
	#region Object overrides
	
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
	
	
	public override bool Equals( object obj )
	{
		// if base already determined that we are equal no need to check further
		if( base.Equals( obj ) )
			return true;
		
		// we can be equal if the other object is a ScalePathTweenProperty
		return obj.GetType() == typeof( ScalePathTweenProperty );
	}
	
	#endregion
	
	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		
		Vector3 localScale = _target.localScale;

		if ( _isRelative )
			_endValue = new Vector3
			(
				_originalEndValue.x * localScale.x,
				_originalEndValue.y * localScale.y,
				_originalEndValue.z * localScale.z
			);
		else
			_endValue = _originalEndValue;
		
		// if this is a from tween we need to swap the start and end values
		if( _ownerTween.isFrom )
		{
			_startValue = _endValue;
			_endValue = localScale;
		}
		else
		{
			_startValue = localScale;
		}
		
		base.prepareForUse();
	}
	
	
	public override void tick( float totalElapsedTime )
	{
		var easedTime = _easeFunction( totalElapsedTime, 0, 1, _ownerTween.duration );
		_target.localScale = GoTweenUtils.unclampedVector3Lerp( _startValue, _diffValue, easedTime );
	}

}
