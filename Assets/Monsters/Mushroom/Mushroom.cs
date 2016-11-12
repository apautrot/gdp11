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
		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);
	}
		
	void FixedUpdate() {
		destX = Player.Instance.transform.position.x;
		destY = Player.Instance.transform.position.y;

		if (Player.Instance.EnergyPoints>0) {
			body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;
		}
	}
}
