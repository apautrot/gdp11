using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
	bool isOpened;

//	System.Action OnOpeningEnded;

	GameObject frame;
	GameObject door;
	GameObject ground;

	void Awake ()
	{
		frame = gameObject.FindChildByName ( "Frame" );
		door = gameObject.FindChildByName ( "Door" );
		ground = gameObject.FindChildByName ( "Ground" );
	}

	public void Open ()
	{
		if ( !isOpened )
			StartCoroutine ( OpenDoor () );
	}

	IEnumerator OpenDoor ()
	{
		isOpened = true;

		float duration = 1;
		door.transform.localPositionTo ( 1, new Vector3 ( 0, 100, 0 ), true );

		yield return new WaitForSeconds ( duration );

// 		if ( OnOpeningEnded != null )
// 			OnOpeningEnded ();
	}
}
