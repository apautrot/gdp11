using UnityEngine;
using System.Collections;

public class Mushroom : Monster
{
	public float targetSpeed, accelerationDuration;
	private float destX, destY;
    Animator anim;
    SpriteRenderer sprite;
    float speed {
		get;
		set;
	}

	new void Start ()
	{
		base.Start();

		speed = 0;

		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);
        sprite = gameObject.FindChildByName("Sprite").GetComponent<SpriteRenderer>();
        anim = sprite.GetComponent<Animator>();

    }

    void FixedUpdate()
	{
		if ( state == MonsterState.Living )
		{
			destX = Player.Instance.transform.position.x;
			destY = Player.Instance.transform.position.y;

			if ( direction == 2 )
				sprite.flipX = true;

			if ( direction == 1 )
				sprite.flipX = false;

			if ( Player.Instance.EnergyPoints > 0 )
				body.velocity = ( new Vector3 ( destX, destY, 0 ) - transform.position ).normalized * speed;

			anim.SetInteger ( "Direction", direction );
		}
    }
}
