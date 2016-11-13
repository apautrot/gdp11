using UnityEngine;
using System.Collections;

public class Music : SceneSingleton<Music>
{
	internal int _enemyCount;
	internal int EnemyCount
	{
		get { return _enemyCount; }
		set
		{
			if ( _enemyCount != value )
			{
				_enemyCount = value;
				if ( fmodEmitter != null )
					fmodEmitter.SetParameter ( "EnemiesOnScreen", _enemyCount );
			}

			DebugWindow.Log ( "Music", "EnemyCount", value );
		}
	}

	internal bool _finalDoorOpened;
	internal bool FinalDoorOpened
	{
		get { return _finalDoorOpened; }
		set
		{
			if ( _finalDoorOpened != value )
			{
				_finalDoorOpened = value;
				if ( fmodEmitter != null )
					fmodEmitter.SetParameter ( "FinalDoorOpened", _finalDoorOpened ? 1 : 0 );
			}

			DebugWindow.Log ( "Music", "FinalDoorOpened", value );
		}
	}

	internal int _doorsOpened;
	internal int DoorOpened
	{
		get { return _doorsOpened; }
		set
		{
			if ( _doorsOpened != value )
			{
				_doorsOpened = value;
				if ( fmodEmitter != null )
					fmodEmitter.SetParameter ( "DoorsOpened", _doorsOpened );
			}

			DebugWindow.Log ( "Music", "DoorsOpened", value );
		}
	}

	internal float _lifePercent;
	internal float LifePercent
	{
		get { return _lifePercent; }
		set
		{
			if ( _lifePercent != value )
			{
				_lifePercent = value;
				if ( fmodEmitter != null )
					fmodEmitter.SetParameter ( "Life", _lifePercent );
			}

			DebugWindow.Log ( "Music", "Life", value );
		}
	}

	FMODUnity.StudioEventEmitter fmodEmitter;

	new void Awake ()
	{
		GameObject.DontDestroyOnLoad ( gameObject );

		fmodEmitter = GetComponent<FMODUnity.StudioEventEmitter> ();
		Debug.LogWarning ( "No music component detected. Start the Title Scene to hae music." );
	}

	void Update ()
	{
		if ( Input.GetKeyDown ( KeyCode.O ) )
			LifePercent = 0;
		if ( Input.GetKeyDown ( KeyCode.P ) )
			LifePercent = 100;

		//  		if ( Input.GetKeyDown ( KeyCode.P ) )
		//  			Player.Instance.EnergyPoints++;
		//  
		//  		if ( Input.GetKeyDown ( KeyCode.M ) )
		//  			Player.Instance.EnergyPoints--;
	}

	internal void ReinitializeVariables ()
	{
		EnemyCount = 0;
		FinalDoorOpened = false;
		DoorOpened = 0;
		LifePercent = 100;
	}
}
