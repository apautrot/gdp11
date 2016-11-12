using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
	public float openingDuration = 1;
	public int SpawnListIndex = -1;

	bool isOpened;
	GameObject sprite;

	Collider2D collider;

	void Awake ()
	{
		sprite = gameObject.FindChildByName ( "Sprite", false );
		//		spawnArea = gameObject.FindChildByName ( "Spawn Area" );
		collider = GetComponent<Collider2D> ();
	}

	public void Open ()
	{
		if ( !isOpened )
			StartCoroutine ( OpenDoor () );
	}

	IEnumerator OpenDoor ()
	{
		isOpened = true;

		if ( sprite != null )
			sprite.transform.localPositionTo ( openingDuration, new Vector3 ( 0, 100, 0 ), true );

		if ( collider != null )
			collider.enabled = false;

		//o Son d'une porte qui s'ouvre (nombre de son en fonction du nombre de porte

		yield return new WaitForSeconds ( openingDuration );

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
