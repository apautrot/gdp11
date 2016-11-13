using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : Singleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		public GameObject LockPrefab;

		public GameObject MushroomPrefab;
		public GameObject LlamaPrefab;
		public GameObject JumpingRockPrefab;
		public GameObject CaterpillarPrefab;
        public GameObject ClockItemPrefab;
		public GameObject HeartItemPrefab;
		public GameObject KeyItemPrefab;

		public GameObject GameOverOverlay;
	}

	public Prefabs prefabs;

	internal GameObject GetPrefab ( ObjectType type )
	{
		switch ( type )
		{
			case ObjectType.EnemyJumpingRock: return prefabs.JumpingRockPrefab;
			case ObjectType.EnemyLlama: return prefabs.LlamaPrefab;
			case ObjectType.EnemyMushroom: return prefabs.MushroomPrefab;
			case ObjectType.EnemyCaterpillar: return prefabs.CaterpillarPrefab;
			case ObjectType.ItemHeart: return prefabs.HeartItemPrefab;
            case ObjectType.ItemClock: return prefabs.ClockItemPrefab;
			case ObjectType.ItemKey: return prefabs.KeyItemPrefab;
			default: return null;
		}
	}

	//Temps du timer
	public int time = 60;
	private float lastTime;

	GameObject blackFade;

	void Awake ()
	{
		blackFade = gameObject.FindChildByName ( "Black Fade" );
	}

	void Start ()
	{
		Gate[] gates = FindObjectsOfType<Gate> ();

		if ( ( gates.Length > 0 ) && ( !GatesSetup.InstanceCreated ) )
		{
			Debug.LogError ( "There is no GatesSetup in this scene." );
			return;
		}

		List<SpawnList> spawnLists = GatesSetup.Instance.SpawnLists;
		if ( spawnLists.Count > 0 )
		{
			if ( spawnLists.Count < gates.Length )
				Debug.LogWarning ( "The GatesSetup in this scene is not properly setup. Add more spawn list. You have " + gates.Length + " gate(s) in this scene. You have only " + spawnLists.Count + " spawn list. Create at least as many spawn list that there are gates in this scene." );

			List<int> indices = new List<int> ();

			for ( int i = 0; i < gates.Length; i++ )
				indices.Add ( i % spawnLists.Count );

			indices.Shuffle ();

			for ( int i = 0; i < gates.Length; i++ )
				gates[i].SpawnListIndex = indices[i];
		}
		else
			Debug.LogError ( "The GatesSetup in this scene is not properly setup." );

		int exitGateIndex = RandomInt.Range ( 0, gates.Length - 1 );
		gates[exitGateIndex].IsExit = true;
		Debug.Log ( "Exit gate is " + gates[exitGateIndex].gateName.ToString () );

		//Init timer
		lastTime = Time.time;

		blackFade.SetActive ( true );
		blackFade.FadeOut ( 0.5f, FadeOutEndAction.Destroy );
	}

	void FixedUpdate() {
		//Décompte timer
		if (Time.time - lastTime >= 1 && time > 0) {
			lastTime = Time.time;
			time--;
			HUD.Instance.GetComponent<HUD> ().SetTime (time);
		}

		if (time == 0 && Player.Instance.EnergyPoints != 0) {
            Audio.Instance.PlaySound(AllSounds.Instance.TimeOver);
            Player.Instance.Die();
		}
	}
}
