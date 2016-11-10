using UnityEngine;
using System.Collections;


/// <summary>
/// TweenFlows are used for creating a chain of Tweens via the append/prepend methods. You can also get timeline
/// like control by inserting Tweens and setting them to start at a specific time. Note that TweenFlows do not
/// honor the delays set within regular Tweens. Use the append/prependDelay method to add any required delays
/// </summary>
public class GoTweenFlow : AbstractGoTweenCollection
{
	public GoTweenFlow() : this(new GoTweenCollectionConfig()) {}
	public GoTweenFlow(GoTweenCollectionConfig config) : base(config) {}
	
	#region internal Flow management
	
	/// <summary>
	/// the item being added already has a start time so no extra parameter is needed
	/// </summary>
	private void insert( TweenFlowItem item )
	{
		// early out for invalid items
		if( item.tween != null && !item.tween.isValid() )
			return;
		
		if( float.IsInfinity( item.duration ) )
		{
			Debug.Log( "adding a Tween with infinite iterations to a TweenFlow is not permitted" );
			return;
		}
		
		// ensure the tween isnt already live
		if( item.tween != null )
			Go.removeTween( item.tween );
		
		// add the item then sort based on startTimes
// 		_tweenFlows.Add( item );
// 		_tweenFlows.Sort( ( x, y ) =>
// 		{
// 			return x.startTime.CompareTo( y.startTime );
// 		} );
		int insertionIndex = _tweenFlows.Count;
		while ( insertionIndex > 0 )
		{
			if ( item.startTime >= _tweenFlows[insertionIndex - 1].startTime )
				break;
			insertionIndex--;
		}
		_tweenFlows.Insert ( insertionIndex, item );


		duration = Mathf.Max( item.startTime + item.duration, duration );
		totalDuration = duration * iterations;
	}
	
	#endregion
	
	
	#region Flow management
	
	/// <summary>
	/// inserts a Tween and sets it to start at the given startTime
	/// </summary>
	public GoTweenFlow insert( float startTime, AbstractGoTween tween )
	{
		var item = new TweenFlowItem( startTime, tween );
		insert( item );
		
		return this;
	}

	/// <summary>
	/// inserts a delegate and sets it to start at the given startTime
	/// </summary>
	public GoTweenFlow insertAction ( float startTime, System.Action action )
	{
		var item = new TweenFlowItem ( startTime, action );
		insert ( item );

		return this;
	}

	/// <summary>
	/// inserts a delay and sets it to start at the given startTime
	/// </summary>
	public GoTweenFlow insertDelay ( float startTime, float delay )
	{
		if ( delay > 0 )
		{
			var item = new TweenFlowItem ( startTime, delay );
			insert ( item );
		}

		return this;
	}

	#endregion
	

}
