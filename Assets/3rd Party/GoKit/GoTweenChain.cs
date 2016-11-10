using UnityEngine;
using System.Collections;

// To improve : delay is not took into duration compute (totalDuration)

public class GoTweenChain : AbstractGoTweenCollection
{
	public GoTweenChain() : this(new GoTweenCollectionConfig()) {}
	public GoTweenChain(GoTweenCollectionConfig config) : base(config) {}

	#region internal Chain management
	
	private void append( TweenFlowItem item )
	{
		// early out for invalid items
		if( item.tween != null && !item.tween.isValid() )
			return;
		
		if( float.IsInfinity( item.duration ) )
		{
			Debug.Log( "adding a Tween with infinite iterations to a TweenChain is not permitted" );
			return;
		}
		
		// ensure the tween isnt already live
		if( item.tween != null )
			Go.removeTween( item.tween );
		
		_tweenFlows.Add( item );
		
		// update the duration and total duration
		duration += item.duration;
		
		if( iterations > 0 )
			totalDuration = duration * iterations;
		else
			totalDuration = float.PositiveInfinity;
	}
	
	
	private void prepend( TweenFlowItem item )
	{
		// early out for invalid items
		if( item.tween != null && !item.tween.isValid() )
			return;
		
		if( float.IsInfinity( item.duration ) )
		{
			Debug.Log( "adding a Tween with infinite iterations to a TweenChain is not permitted" );
			return;
		}
		
		// ensure the tween isnt already live
		if( item.tween != null )
			Go.removeTween( item.tween );
		
		// fix all the start times on our previous chains
		foreach( var ci in _tweenFlows )
			ci.startTime += item.duration;
		
		_tweenFlows.Add( item );
		
		// update the duration and total duration
		duration += item.duration;
		totalDuration = duration * iterations;
	}
	
	#endregion
	
	
	#region Chain management
	
	/// <summary>
	/// appends a Tween at the end of the current flow
	/// </summary>
	public GoTweenChain append( AbstractGoTween tween )
	{
		var item = new TweenFlowItem ( this.duration, tween );
		append( item );
		
		return this;
	}
	
	
	/// <summary>
	/// appends a delay to the end of the current flow
	/// </summary>
	public GoTweenChain appendDelay( float delay )
	{
		if ( delay > 0 )
		{
			var item = new TweenFlowItem ( this.duration, delay );
			append ( item );
		}
		
		return this;
	}


	/// <summary>
	/// appends a delegate to the end of the current flow
	/// </summary>
	public GoTweenChain appendAction ( System.Action action, float delay = 0 )
	{
		var item = new TweenFlowItem ( this.duration + delay, action );
		append( item );
		
		return this;
	}
	
	
	/// <summary>
	/// adds a Tween to the front of the flow
	/// </summary>
	public GoTweenChain prepend( AbstractGoTween tween )
	{
		var item = new TweenFlowItem( 0, tween );
		prepend( item );
		
		return this;
	}
	
	
	/// <summary>
	/// adds a delay to the front of the flow
	/// </summary>
	public GoTweenChain prependDelay( float delay )
	{
		var item = new TweenFlowItem( 0, delay );
		prepend( item );
		
		return this;
	}

	#endregion


	#region internal Flow management

	/// <summary>
	/// the item being added already has a start time so no extra parameter is needed
	/// </summary>
	private void insert ( TweenFlowItem item )
	{
		// early out for invalid items
		if ( item.tween != null && !item.tween.isValid () )
			return;

		// ensure the tween isnt already live
		if ( item.tween != null )
			Go.removeTween ( item.tween );

		if ( float.IsInfinity ( item.duration ) )
		{
			Debug.Log ( "adding a Tween with infinite iterations to a TweenFlow is not permitted" );
			return;
		}

		// add the item then sort based on startTimes
// 		_tweenFlows.Add( item );
// 		_tweenFlows.Sort( ( x, y ) =>
// 		{
// 			return x.startTime.CompareTo( y.startTime );
// 		} );
		int insertionIndex = _tweenFlows.Count;
		while ( insertionIndex > 0 )
		{
			if ( item.startTime >= _tweenFlows[insertionIndex-1].startTime )
				break;
			insertionIndex--;
		}
		_tweenFlows.Insert ( insertionIndex, item );

		duration = Mathf.Max ( item.startTime + item.duration, duration );
		totalDuration = duration * iterations;
	}

	#endregion


	#region Flow management

	/// <summary>
	/// inserts a Tween and sets it to start at the given startTime
	/// </summary>
	public GoTweenChain insert ( float startTime, AbstractGoTween tween )
	{
		var item = new TweenFlowItem ( startTime, tween );
		insert ( item );

		return this;
	}

	/// <summary>
	/// inserts a Tween and sets it to start at the given startTime
	/// </summary>
	public GoTweenChain insertAction ( float startTime, System.Action action )
	{
		var item = new TweenFlowItem ( startTime, action );
		insert ( item );

		return this;
	}

	#endregion
}
