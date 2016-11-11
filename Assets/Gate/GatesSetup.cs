using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ObjectType
{
	ItemHeart,
	ItemSpeedUp,

	EnemyMushroom,
	EnemyLlama,
	EnemyJumpingRock
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
}

public class GatesSetup : MonoBehaviour
{
	public List<SpawnList> SpawnLists;
}
