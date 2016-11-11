using UnityEngine;
using System.Collections;

public class Sputum : MonoBehaviour {

	float speed;
	Rigidbody2D body;
	Vector3 direction;

	void Start() {
		body = GetComponent<Rigidbody2D> ();
	}

	public void Fire(float x, float y, float speed) {
		this.speed = speed;
		direction = new Vector3 (x, y, 0) - transform.position;
		transform.Translate (direction / 6);
	}

	void FixedUpdate() {
		body.velocity = direction * speed;

		if (transform.position.x > 10 || transform.position.x < -10 || transform.position.y > 10 || transform.position.y < -10) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D (Collision2D other) {
		Destroy (gameObject);
	}
}
