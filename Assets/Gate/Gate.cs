using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
	bool isOpened;

//	System.Action OnOpeningEnded;

	GameObject door;
	GameObject spawnArea;

	float duration = 1;

	public int SpawnListIndex = -1;

	void Awake ()
	{
		door = gameObject.FindChildByName ( "Door" );
		spawnArea = gameObject.FindChildByName ( "Spawn Area" );
	}

	public void Open ()
	{
		if ( !isOpened )
			StartCoroutine ( OpenDoor () );
	}

	IEnumerator OpenDoor ()
	{
		isOpened = true;

		door.transform.localPositionTo ( 1, new Vector3 ( 0, 100, 0 ), true );

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
	}
}
