using UnityEngine;
using System.Collections;


enum MonsterState
{
	Spawning,
	Living,
	Dying
}

public class Monster : MonoBehaviour
{
	protected int direction
	{
		get
		{
			Vector2 velocity = body.velocity;
			if ( velocity != Vector2.zero )
			{
				if ( Mathf.Abs ( velocity.x ) > Mathf.Abs ( velocity.y ) )
					return ( velocity.x < 0 ) ? 1 : 2;
				else
					return ( velocity.y < 0 ) ? 0 : 3;
			}
			else
				return 0;
		}
	}

	protected Rigidbody2D body;
	
	public int life = 3;

	internal MonsterState state = MonsterState.Spawning;

	protected void Awake ()
	{
		body = GetComponent<Rigidbody2D> ();
	}

	protected void Start ()
	{
		state = MonsterState.Living;
	}

	internal virtual void TakeDamage ()
	{
		if ( state == MonsterState.Living )
		{
			Vector3 splashVector = ( transform.position - Player.Instance.transform.position ).normalized;

			life -= 1;
			if ( life == 0 )
			{
                ApplySound();
                state = MonsterState.Dying;

				GoTweenChain chain = new GoTweenChain ();
				chain.insert ( 0,
					transform
					.scaleTo ( 0.5f, 0.75f, true )
					.eases ( GoEaseType.QuartOut )
				);
				chain.insert ( 0,
					transform
					.localEularAnglesTo ( 0.5f, new Vector3 ( 0, 0, RandomFloat.Range ( -180, 180 ) ) )
					.eases ( GoEaseType.QuadOut )
				);
				chain.insert ( 0.25f,
					gameObject
					.alphaTo ( 0.25f, 0, GoEaseType.QuartOut )
				);
				chain.setOnCompleteHandler ( c => gameObject.DestroySelf () );
				chain.Start ();
			}

			transform.position += splashVector * 25;

// 			bool isDying = ( life == 0 );
// 			transform.position += splashVector * ( isDying ? 100 : 25 );
		}
	}

    protected virtual void ApplySound()
    {
        Audio.Instance.PlaySound(AllSounds.Instance.MonsterDies);
    }
}
