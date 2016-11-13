using UnityEngine;
using System.Collections;

public class Caterpillar2 : MonoBehaviour {

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
	Vector3 target;
	public float targetSpeed = 0.1f;
	public float phaseTime = 10;
	private float timeB;
	private float speed {
		get;
		set;
	}
	private Animator anim;
	private SpriteRenderer renderer;
	//On doit calculer la vitesse du caterpillar pour savoir si il est bloqué
	private float lastX, lastY;

	void Start () {
		timeB = Time.time;
		lastX = transform.position.x;
		lastY = transform.position.y;
		body = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		renderer = GetComponent<SpriteRenderer> ();
		NewPosition ();
		//body.velocity = new Vector3(-1, -1, 0).normalized;
		this.floatTo("speed", 1f, targetSpeed / 10, false);
	}

	public void NewPosition()
	{
		target = new Vector3(Random.Range(-GameCamera.Instance.maxWidth, GameCamera.Instance.maxWidth), Random.Range(-GameCamera.Instance.maxHeight, GameCamera.Instance.maxHeight), 0).normalized * 1000;
		//Debug.Log (target);
	}

	void FixedUpdate () {
		body.velocity = (target/* - transform.position*/) * speed;
		if (Time.time - timeB >= phaseTime) {
			timeB = Time.time;
			NewPosition ();
		}

		if (direction == 1)
			renderer.flipX = false;
	
		if (direction == 2)
			renderer.flipX = true;

		Debug.Log (transform.position.x - lastX);

		//On check si le caterpillar est bloqué
		if ((transform.position.x - lastX < 0.1f && transform.position.x - lastX > -0.1f) || (transform.position.y - lastY < 0.1f && transform.position.y - lastY > -0.1f)) {
			NewPosition ();
			Debug.Log ("Caterpillar blocked, trying to generate new angle");
		}
		//Debug.Log (body.velocity.x);

		lastX = transform.position.x;
		lastY = transform.position.y;
		anim.SetInteger ("Direction", direction);
	}

	void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.name != "Body") {
			NewPosition ();
		}
	}
}
