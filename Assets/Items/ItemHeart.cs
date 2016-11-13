using UnityEngine;
using System.Collections;


public class ItemBase : MonoBehaviour
{
	public float jumpDuration = 0.5f;
	public float jumpHeight = 10;
	public GoEaseType jumpEase = GoEaseType.QuadInOut;

	GameObject sprite;

	AbstractGoTween tween;

	void Awake ()
	{
		sprite = gameObject.FindChildByName ( "Sprite" );
	}

	void OnDestroy ()
	{
		if ( tween != null )
			tween.destroy ();
	}

	void Start ()
	{
		tween = sprite.transform.localPositionTo ( jumpDuration, new Vector3 ( 0, jumpHeight, 0 ), true )
			.eases ( jumpEase )
			.loopsInfinitely ( GoLoopType.PingPong );
	}

	void OnTriggerEnter2D ( Collider2D collider )
	{
		if ( collider.GetComponent<Player> () != null )
		{
			gameObject.DestroySelf ();

			ApplyEffect ();
		}
	}

	protected virtual void ApplyEffect ()
	{
		Player.Instance.EnergyPoints++;
	}
}

public class ItemHeart : ItemBase
{
	protected override void ApplyEffect ()
	{
		Audio.Instance.PlaySound ( AllSounds.Instance.GetItem );
		Player.Instance.EnergyPoints++;
	}
}
