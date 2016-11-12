using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
	bool isOpened;

//	System.Action OnOpeningEnded;

	GameObject sprite;
//	GameObject spawnArea;

	float duration = 1;

	public int SpawnListIndex = -1;

	void Awake ()
	{
		sprite = gameObject.FindChildByName ( "Sprite" );
//		spawnArea = gameObject.FindChildByName ( "Spawn Area" );
	}

	public void Open ()
	{
		if ( !isOpened )
			StartCoroutine ( OpenDoor () );
	}

	IEnumerator OpenDoor ()
	{
		isOpened = true;

		sprite.transform.localPositionTo ( 1, new Vector3 ( 0, 100, 0 ), true );

        //o Son d'une porte qui s'ouvre (nombre de son en fonction du nombre de porte
        yield return new WaitForSeconds ( duration );

		if ( SpawnListIndex == -1 )
			Debug.LogError ( "This gate ( " + name + " has an invalid setup index ! Find GatesSetup game object and setup it." );
		else
			GatesSetup.Instance.Spawn ( this );

// 		if ( OnOpeningEnded != null )
// 			OnOpeningEnded ();
	}

	internal void SpawnItem ( ObjectType type )
	{
		GameObject prefab = Game.Instance.GetPrefab ( type );
		GameObject go = Game.Instance.gameObject.InstantiateSibling ( prefab );
		// go.transform.position = spawnArea.transform.position;

		go.transform.position = transform.position;
	}
}
