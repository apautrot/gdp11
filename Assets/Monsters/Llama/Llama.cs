using UnityEngine;
using System.Collections;

public class Llama : MonoBehaviour {
	public GameObject prefab;
	public float targetSpeed, accelerationDuration, fireRate, fireSpeed, noFireArea;
	Animator anim;
	SpriteRenderer sprite;
	float destX, destY;
	float speed {
		get;
		set;
	}
	int direction {
		get {
			if ((body.velocity.x > 0 && body.velocity.x > body.velocity.y) || (body.velocity.x < 0 && body.velocity.x < body.velocity.y)) {
				if (body.velocity.x > 0)
					return 2;
				if (body.velocity.x < 0)
					return 1;
			}
			else {
				if (body.velocity.y > 0)
					return 3;
				if (body.velocity.y < 0)
					return 0;
			}
			return 0;
		}

		set	{
			direction = value;
		}
	}

	float timeBefore;
	Rigidbody2D body;


	void Start () {
		speed = 0;
		body = GetComponent<Rigidbody2D> ();
		timeBefore = Time.time;
		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);
		sprite = gameObject.FindChildByName ("Sprite").GetComponent<SpriteRenderer> ();
		anim = gameObject.FindChildByName("Sprite").GetComponent<Animator> ();
	}

	void FixedUpdate() {
		destX = Player.Instance.transform.position.x;
		destY = Player.Instance.transform.position.y;

		if (Player.Instance.EnergyPoints > 0)
			body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;

		if (direction == 2)
			sprite.flipX = true;
	
		if (direction == 1)
			sprite.flipX = false;


		//Tire quand le firerate est dépassé
		if (Time.time - timeBefore >= fireRate) {
			if (Vector3.Distance(new Vector3(destX, destY, 0), transform.position) > noFireArea) {
                
				anim.SetBool ("Sputum", true);
				timeBefore = Time.time;
				this.WaitAndDo (1.5f, Shoot);

			}
		}
		anim.SetInteger ("Direction", direction);
		//Debug.Log (anim.GetBool("Sputum"));
		//Debug.Log (anim.GetInteger("Direction"));
	}

	void Shoot () {
		GameObject go = (GameObject)Instantiate(prefab);
		go.transform.position = transform.position;
		// Son de tirs
		go.GetComponent<Sputum> ().Fire (destX, destY, fireSpeed);
		anim.SetBool ("Sputum", false);
	}

}
