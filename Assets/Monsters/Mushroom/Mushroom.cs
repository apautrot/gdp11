using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour {

	public float targetSpeed, accelerationDuration;
	private float destX, destY;
	float speed {
		get;
		set;
	}
	Rigidbody2D body;

	void Start () {
		speed = 0;
		body = GetComponent<Rigidbody2D> ();
	}
		
	void FixedUpdate() {
		destX = GameObject.Find ("Player Test").transform.position.x;
		destY = GameObject.Find ("Player Test").transform.position.y;

		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);

		body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;
	}
}
