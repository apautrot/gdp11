using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {

	public float targetSpeed, accelerationDuration;
	private float destX, destY;
    Animator anim;
    SpriteRenderer sprite;
    float speed {
		get;
		set;
	}

    int direction
    {
        get
        {
            if ((body.velocity.x > 0 && body.velocity.x > body.velocity.y) || (body.velocity.x < 0 && body.velocity.x < body.velocity.y))
            {
                if (body.velocity.x > 0)
                    return 2;
                if (body.velocity.x < 0)
                    return 1;
            }
            else
            {
                if (body.velocity.y > 0)
                    return 3;
                if (body.velocity.y < 0)
                    return 0;
            }
            return 0;
        }

        set
        {
            direction = value;
        }
    }
    Rigidbody2D body;

	void Start () {
		speed = 0;
		body = GetComponent<Rigidbody2D> ();
		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);
        sprite = gameObject.FindChildByName("Sprite").GetComponent<SpriteRenderer>();
        anim = sprite.GetComponent<Animator>();

    }

    void FixedUpdate() {
		destX = Player.Instance.transform.position.x;
		destY = Player.Instance.transform.position.y;

        if (direction == 2)
            sprite.flipX = true;

        if (direction == 1)
            sprite.flipX = false;

        if (Player.Instance.EnergyPoints > 0)
			body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;

        anim.SetInteger("Direction", direction);
    }
}
