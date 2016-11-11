using UnityEngine;
using System.Collections;

public class Sputum : MonoBehaviour {

	float speed;
	Rigidbody2D body;
	Vector3 direction;
	float maxWidth, maxHeight;


	void Awake() {
		body = GetComponent<Rigidbody2D> ();
		maxHeight = GameCamera.Instance.GetComponent<Camera> ().orthographicSize;
		maxWidth = maxHeight * GameCamera.Instance.GetComponent<Camera> ().aspect;
		//Debug.Log (maxHeight + "   " + maxWidth);
	}

	public void Fire(float x, float y, float speed) {
		this.speed = speed;
		direction = new Vector3 (x, y, 0) - transform.position;
		//On décale le crachat en direction du joueur pour qu'il ne rentre pas dans le lama
		transform.Translate (direction / 5);
		//Debug.Log ("Fired");
	}

	void FixedUpdate() {
		body.velocity = direction * speed;
		//On supprime le GameObject si il n'est plus visible
		if (transform.position.x > maxWidth|| transform.position.x < -maxWidth || transform.position.y > maxHeight || transform.position.y < -maxHeight) {
			Destroy (gameObject);
			//Debug.Log ("Bullet destroyed");
		}
	}

	void OnCollisionEnter2D (Collision2D other) {
		Destroy (gameObject);
	}
}
