using UnityEngine;
using System.Collections;

public class Caterpillar2_collision : MonoBehaviour {

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
	private float speed {
		get;
		set;
	}

	void Start () {
		body = GetComponent<Rigidbody2D> ();
		//NewPosition ();
		body.velocity = new Vector3(-1, -1, 0).normalized;
		this.floatTo("speed", 1f, targetSpeed / 10, false);
	}

	public void NewPosition()
	{
		target = new Vector3(Random.Range(-GameCamera.Instance.maxWidth, GameCamera.Instance.maxWidth), Random.Range(-GameCamera.Instance.maxHeight, GameCamera.Instance.maxHeight), 0);
		Debug.Log (target);
	}

	void FixedUpdate () {
		body.velocity = (target/* - transform.position*/).normalized * speed;
	}

	void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.tag != "Body")
			target = new Vector3(-(transform.position - other.gameObject.transform.position), (transform.position - other.gameObject.transform.position).y, 0).normalized;
	}
}
