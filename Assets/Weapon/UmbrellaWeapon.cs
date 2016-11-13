using UnityEngine;
using System.Collections;

interface IWeapon
{
	void Throw ( Direction direction );

	System.Action OnEnd { get; set; }
}

public class UmbrellaWeapon : MonoBehaviour, IWeapon
{
	public float shootDuration = 0.35f;
	public float shootDistance = 100;
	public GoEaseType shootEase = GoEaseType.QuadOut;

	public float timeBetweenWeapons= 0.35f;

	System.Action onEnd;
	public System.Action OnEnd { get { return onEnd; } set { onEnd = value; } }

	GoTweenChain tween;

	void OnDestroy ()
	{
		if ( tween != null )
			tween.destroy ();
	}

	public void Throw ( Direction direction )
	{
		transform.SetScale ( 0.25f );

		transform.localEulerAngles = new Vector3 ( 0, 0, direction.ToRotationAngle () );

		Vector2 localMoveVector = direction.ToVector2 () * shootDistance;

		if ( tween != null )
		{
			Debug.LogError ( "Tween is already running" );
			gameObject.DestroySelf ();
		}

		tween = new GoTweenChain ();
		tween.insert ( 0, transform.localPositionTo ( shootDuration, localMoveVector, true ).eases ( shootEase ) );
		tween.insert ( 0, transform.scaleTo ( 0.125f, 1 ).eases ( GoEaseType.QuartIn ) );
		tween.insert ( 0.25f, transform.scaleTo ( 0.5f, 0 ).eases ( GoEaseType.QuadOut ) );
		tween.insertAction ( timeBetweenWeapons, () => { if ( onEnd != null ) onEnd (); } );
		tween.Start ();
		tween.setOnCompleteHandler ( c => gameObject.DestroySelf () );
	}

	void OnTriggerEnter2D ( Collider2D collider )
	{
		Monster monster = collider.gameObject.GetComponent<Monster> ();
		if ( monster != null )
		{
			monster.TakeDamage ();
			//Son quand il frappe (en fonction du type d'arme)
		}
		else
		{
			// Son quand il frappe et qu'il touche ne touche rien
		}
	}
}
