using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// base class for TweenChains and TweenFlows
/// </summary>
public class AbstractGoTweenCollection : AbstractGoTween
{
	protected List<TweenFlowItem> _tweenFlows = new List<TweenFlowItem>();

	/// <summary>
	/// data class that wraps an AbstractTween and its start time for the timeline
	/// </summary>
	protected class TweenFlowItem
	{
		public float startTime;
		public float duration;
		public AbstractGoTween tween;
		public System.Action action;
		internal bool actionFired;

		public TweenFlowItem ( float startTime, System.Action action )
		{
			this.action = action;
			this.startTime = startTime;
			this.duration = 0;
		}


		public TweenFlowItem( float startTime, AbstractGoTween tween )
		{
			this.tween = tween;
			this.startTime = startTime;
			this.duration = tween.totalDuration;
		}
		
		
		public TweenFlowItem( float startTime, float duration )
		{
			this.duration = duration;
			this.startTime = startTime;
		}

		public override string ToString ()
		{
			return "<" + startTime + ":" + duration + ">" + ( ( tween != null ) ? tween.ToString () : action.ToString() );
		}
	}
	
	public AbstractGoTweenCollection( GoTweenCollectionConfig config )
	{
		// copy the TweenConfig info over
		id = config.id;
		loopType = config.loopType;
		iterations = config.iterations;
		updateType = config.propertyUpdateType;
		_onComplete = config.onCompleteHandler;
		_onStart = config.onStartHandler;
		_onIterate = config.onIterateHandler;
		timeScale = 1;
		state = GoTweenState.Paused;
		autoRemoveOnComplete = true;
		// Go.addTween( this );
	}
		
	
	#region AbstractTween overrides
	
	/// <summary>
	/// returns a list of all Tweens with the given target in the collection
	/// technically, this should be marked as internal
	/// </summary>
	public List<GoTween> tweensWithTarget( object target )
	{
		List<GoTween> list = new List<GoTween>();
		
		foreach( var item in _tweenFlows )
		{
			// skip TweenFlowItems with no target
			if( item.tween == null )
				continue;
			
			// check Tweens first
			var tween = item.tween as GoTween;
			if( tween != null && tween.target == target )
				list.Add( tween );
			
			// check for TweenCollections
			if( tween == null )
			{
				var tweenCollection = item.tween as AbstractGoTweenCollection;
				if( tweenCollection != null )
				{
					var tweensInCollection = tweenCollection.tweensWithTarget( target );
					if( tweensInCollection.Count > 0 )
						list.AddRange( tweensInCollection );
				}
			}
		}
		
		return list;
	}
	
	
	public override bool removeTweenProperty( AbstractTweenProperty property )
	{
		foreach( var tweenFlowItem in _tweenFlows )
		{
			// skip delay items which have no tween
			if( tweenFlowItem.tween == null )
				continue;
			
			if( tweenFlowItem.tween.removeTweenProperty( property ) )
				return true;
		}
		
		return false;
	}
	
	
	public override bool containsTweenProperty( AbstractTweenProperty property )
	{
		foreach( var tweenFlowItem in _tweenFlows )
		{
			// skip delay items which have no tween
			if( tweenFlowItem.tween == null )
				continue;
			
			if( tweenFlowItem.tween.containsTweenProperty( property ) )
				return true;
		}
		
		return false;
	}
	
	
	public override List<AbstractTweenProperty> allTweenProperties()
	{
		var propList = new List<AbstractTweenProperty>();
		
		foreach( var tweenFlowItem in _tweenFlows )
		{
			// skip delay items which have no tween
			if( tweenFlowItem.tween == null )
				continue;
			
			propList.AddRange( tweenFlowItem.tween.allTweenProperties() );
		}
		
		return propList;
	}

	
	/// <summary>
	/// we are always considered valid because our constructor adds us to Go and we start paused
	/// </summary>
	public override bool isValid()
	{
		return true;
	}
	
	
	/// <summary>
	/// tick method. if it returns true it indicates the tween is complete
	/// </summary>
	public override bool update( float deltaTime )
	{
		base.update( deltaTime );

		if ( !_delayComplete )
			return false;

		// if we are looping back on a PingPong loop
		var convertedElapsedTime = _isLoopingBackOnPingPong ? duration - _elapsedTime : _elapsedTime;
		
		if ( ( state == GoTweenState.Complete ) || ( _tweenFlows.Count == 0 ) )
		{
			//Make sure all tweens get completed
			//Sometimes, it happens the last tween has not yet completed when the tweenchain is completed (maybe for a 0.0000000001 difference in the way convertedElapsedTime calculated)
			completeFlows ();
			
			if( !_didComplete )
				onComplete ();
			
			return true; // true because complete
		}
		else
		{
			// update all properties
			foreach( var flow in _tweenFlows )
			{
				// only update whose startTime has passed
				if ( convertedElapsedTime > flow.startTime )
				{
					// if ( flow.tween != null && ( ! flow.tween.isCompleted ) )
					// when tween collection is doing pingpong, tween in collection might have reached the end,
					// and get completed, but still we want to play them with time reversing..

					// following test below
					// && ( tweenConvertedElapsed < flow.duration )
					// is not working properly when flow has a delay because flow.duration doesn't take that delay into account

					// following test below
					// && ( tweenConvertedElapsed < flow.duration )
					// is not working properly because flow is likely to not being called with elapsed time at flow.duration ( ex: will be called until reaching 4.95f and not 5.0f )
					// which make the flow never do the latest step of its tweening

					// update flows that have a Tween
					float tweenConvertedElapsed = convertedElapsedTime - flow.startTime;
					if ( ( flow.tween != null ) && ( flow.tween.state == GoTweenState.Running ) /* && ( tweenConvertedElapsed < flow.duration ) */ )
					{
						// TODO: further narrow down who gets an update for efficiency
						flow.tween.goTo ( tweenConvertedElapsed );
					}

					// execute whose that have action not fired yet
					if ( flow.action != null && !flow.actionFired )
					{
						flow.action ();
						flow.actionFired = true;
					}
				}
			}

			return false; //false if not complete

		}
	}
	
	
	public override void rewind()
	{
		state = GoTweenState.Paused;
		
		// reset all state here
		_elapsedTime = _totalElapsedTime = 0;
		_isLoopingBackOnPingPong = false;
		_completedIterations = 0;

		restartFlows ();
	}
	
	
	/// <summary>
	/// completes the tween. sets the object to it's final position as if the tween completed normally
	/// </summary>
	public override void complete()
	{
		if( iterations < 0 )
			return;
		
		base.complete();

		completeFlows ();

// 		foreach( var flow in _tweenFlows )
// 		{
// 			// only update flows that have a Tween
// 			if( flow.tween != null )
// 				flow.tween.goTo( flow.tween.totalDuration );
// 		}
	}


	/// <summary>
	/// called once per tween when it iterates (if and when looping)
	/// </summary>
	protected override void onIterate ()
	{
		// ensure latest flows have their onComplete being called
		completeFlows ();

		// ensure flows restart in the exact state than before their first play
		restartFlows ();

		// completeFlows may change animated properties to their ending state
		// call onIterate handler after so it can reset animated properties state
		base.onIterate();
	}


	/// <summary>
	/// restart all flows
	/// </summary>
	private void restartFlows ()
	{
		foreach ( var flow in _tweenFlows )
		{
			// only complete flows that have a Tween, that can complete and that are not yet completed
			if ( flow.tween != null )
			{
				flow.tween.restart ( false /*don't skip delay*/ );
				flow.actionFired = false;
			}
		}
	}


// 	public virtual void restart ( bool skipDelay = true )
// 	{
// 		base.restart ( skipDelay );
// 	}


	/// <summary>
	/// complete all flows
	/// </summary>
	private void completeFlows ()
	{
		foreach ( var flow in _tweenFlows )
		{
			// only complete flows that have a Tween, that can complete and that are not yet completed
			if ( flow.tween != null && flow.tween.iterations >= 0 && flow.tween.state != GoTweenState.Complete )
			{
				flow.tween.goTo ( isReversed ? 0 : flow.tween.totalDuration );
				flow.tween.complete ();
			}

			if ( flow.action != null && !flow.actionFired )
			{
				flow.action ();
				flow.actionFired = true;
			}

			// flow.actionFired = false;
		}
	}
	
	#endregion

	static string indentSpace;

	static int _indent;
	static int indent
	{
		get
		{
			return _indent;
		}
		set
		{
			_indent = value;
			indentSpace = "";
			for ( int i = 0; i < _indent; i++ )
				indentSpace += "   ";
		}
	}

	public override string ToString ()
	{
		indent++;

		string output = base.ToString();
		
		foreach ( var flow in _tweenFlows )
			output += "\n" + indentSpace + flow.ToString ();

		indent--;

		return output;
	}

}
