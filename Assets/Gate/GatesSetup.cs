using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ObjectType
{
	ItemHeart,
    ItemClock,
	EnemyMushroom,
	EnemyLlama,
	EnemyJumpingRock,
	EnemyCaterpillar
}

[System.Serializable]
public class Spawn
{
	public int Chance;
	public ObjectType ObjectType;
}

[System.Serializable]
public class SpawnList
{
	public string Name;
	public List<Spawn> Items;

	internal List<ObjectType> Spawn ( Gate gate )
	{
		// Debug.Log ( "Spawning list named " + Name );

		List<ObjectType> objectsToSpawn = new List<ObjectType>();

		// get total chance token count
		int totalChanceCount = 0;
		for ( int i = 0; i < Items.Count; i++ )
			totalChanceCount += Items[i].Chance;

		// draw a value in this full range
		int chanceValue = RandomInt.Range ( 0, totalChanceCount );

		// try to find to which item this value belongs to
		int chanceRangeLowValue = 0;
		for ( int i = 0; i < Items.Count; i++ )
		{
			int changeCountForThisItem = Items[i].Chance;

			// compute low-high chance range
			int chanceRangeHighValue = chanceRangeLowValue + changeCountForThisItem;

			// spawn this item if change value in this item's chance range
			bool isChanceInThisRange = ( chanceValue >= chanceRangeLowValue ) && ( chanceValue < chanceRangeHighValue );
			// or spawn if forced by special 0 chance value
			bool isSpawnForced = ( Items[i].Chance == 0 );

			bool spawnThisItem = isSpawnForced || isChanceInThisRange;

			if ( spawnThisItem )
			{
				ObjectType type = Items[i].ObjectType;

// 				Debug.Log
// 				(
// 					"  -> Spawning item " + type.ToString () + " at gate " + gate.name + " => "
// 					+ ( isSpawnForced ? "Spawn force by special chance value at 0. " : "" )
// 					+ ( isChanceInThisRange ? ( "Chance value " + chanceValue + " in range [" + chanceRangeLowValue + " , " + chanceRangeHighValue + "]." ) : "" )
// 				);

				objectsToSpawn.Add ( type );
			}

			chanceRangeLowValue = chanceRangeHighValue;
		}

		return objectsToSpawn;
	}
}

public class GatesSetup : SceneSingleton<GatesSetup>
{
	public List<SpawnList> SpawnLists;

	new void Awake ()
	{
		base.Awake ();

		for ( int i = 0; i < SpawnLists.Count; i++ )
		{
			SpawnList spawnList = SpawnLists[i];

			if ( spawnList.Name.Length == 0 )
				Debug.LogWarning ( "The spawn list at rank " + i + " has no name." );

			if ( spawnList.Items.Count == 0 )
				Debug.LogError ( "The spawn list named '" + spawnList.Name + " ( at rank " + i + " ) is not properly setup. The list is empty." );
		}
	}

	internal void Spawn ( Gate gate )
	{
		SpawnList spawnList = SpawnLists[gate.SpawnListIndex];

		List<ObjectType> objectsToSpawn = spawnList.Spawn ( gate );

		StartCoroutine ( SpawnObjectList ( gate, objectsToSpawn ) );
	}

	IEnumerator SpawnObjectList ( Gate gate, List<ObjectType> objectsToSpawn )
	{
		for ( int i = 0; i < objectsToSpawn.Count; i++ )
		{
			gate.SpawnItem ( objectsToSpawn[i] );
			yield return new WaitForSeconds ( 1 );
		}
	}
}
