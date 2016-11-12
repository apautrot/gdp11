﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : Singleton<Game>
{
	[System.Serializable]
	public class Prefabs
	{
		GameObject MushroomPrefab;
		GameObject LlamaPrefab;
		GameObject JumpingRockPrefab;
		GameObject CaterpillarPrefab;

		GameObject HeartItemPrefab;
	}

	public Prefabs prefabs;

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
				Debug.LogWarning ( "The GatesSetup in this scene is not properly setup. Add more spawn list. You have " + gates.Length  + " gate(s) in this scene. You have only " + spawnLists.Count + " spawn list. Create at least as many spawn list that there are gates in this scene." );

			List<int> indices = new List<int> ();

			for ( int i = 0; i < gates.Length; i++ )
				indices.Add ( i % spawnLists.Count );

			indices.Shuffle ();

			for ( int i = 0; i < gates.Length; i++ )
				gates[i].SpawnListIndex = indices[i];
		}
		else
			Debug.LogError ( "The GatesSetup in this scene is not properly setup." );
	}
}