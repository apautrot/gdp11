using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : Singleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		public GameObject MushroomPrefab;
		public GameObject LlamaPrefab;
		public GameObject JumpingRockPrefab;
		public GameObject CaterpillarPrefab;

		public GameObject HeartItemPrefab;
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
			default: return null;
		}
	}

	//Temps du timer
	public int time = 60;
	private float lastTime;

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

		//Init timer
		lastTime = Time.time;
	}

	void FixedUpdate() {
		//Décompte timer
		if (Time.time - lastTime >= 1 && time > 0) {
			lastTime = Time.time;
			time--;
			HUD.Instance.GetComponent<HUD> ().SetTime (time);
		}

		if (time == 0) {
			Player.Instance.Die ();
		}
	}

// 	void Update ()
// 	{
// 		if ( Input.GetKeyDown ( KeyCode.P ) )
// 			Player.Instance.EnergyPoints++;
// 
// 		if ( Input.GetKeyDown ( KeyCode.M ) )
// 			Player.Instance.EnergyPoints--;
// 	}
}
