using UnityEngine;
using System.Collections;

interface IWeapon
{
	System.Action OnEnd { get; set; }
}

public class UmbrellaWeapon : MonoBehaviour, IWeapon
{
	System.Action onEnd;
	public System.Action OnEnd { get { return onEnd; } set { onEnd = value; } }

	void Start ()
	{
		RaycastHit2D[] hits = Physics2D.BoxCastAll ( (Vector2)transform.position + new Vector2 ( 0, 32 ), new Vector2 ( 64, 64 ), 0, Vector2.up, 0 );
		for ( int i = 0; i < hits.Length; i++ )
		{
			Collider2D collider = hits[i].collider;
			if ( collider.gameObject.GetComponent<Monster> () != null )
			{
				collider.gameObject.GetComponent<Rigidbody2D> ().AddForce ( new Vector2 ( 0, 50 ), ForceMode.VelocityChange );
				//Son quand il frappe (en fonction du type d'arme)
			}
			else
			{
				// Son quand il frappe et qu'il touche ne touche rien
			}

			//			Debug.Log ( "Hits[" + i + "] => " + hits[i].collider.name );
		}

		// 		transform.localEulerAngles = new Vector3 ( 0, 0, 90 );
		// 		transform.localEularAnglesTo ( 0.25f, new Vector3 ( 0, 0, -90 ) );

		gameObject.DestroySelf ();

		if ( onEnd != null )
			onEnd ();
	}
	

// 	void Update ()
// 	{
// 		DebugWindow.ClearGroup ( "Umbrella" );
// 		RaycastHit2D[] hits = Physics2D.BoxCastAll ( (Vector2) transform.position + new Vector2 ( 0, 32 ), new Vector2 ( 32, 64 ), 0, Vector2.up, 0 );
// 		for ( int i = 0; i < hits.Length; i++ )
// 		{
// 			DebugWindow.Log ( "Umbrella", "Hits[" + i + "]", hits[i].collider.name );
// 		}
// 	}

// 	IEnumerator AnimateCoroutine ()
// 	{
// 		
// 	}

}
