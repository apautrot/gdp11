using UnityEngine;
using System.Collections;

public class Llama : MonoBehaviour {
	public GameObject prefab;
	public float targetSpeed, accelerationDuration, fireRate, fireSpeed;
	float destX, destY;
	float speed {
		get;
		set;
	}
	float timeBefore;
	Rigidbody2D body;


	void Start () {
		speed = 0;
		body = GetComponent<Rigidbody2D> ();
		timeBefore = Time.time;
	}

	void FixedUpdate() {
		destX = Player.Instance.transform.position.x;
		destY = Player.Instance.transform.position.y;

		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);

		body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;

		//Tire quand le firerate est dépassé
		if (Time.time - timeBefore >= fireRate) {
			GameObject go = (GameObject)Instantiate(prefab);
			go.transform.position = transform.position;
			//go.transform.position = new Vector3(2,2,0);
			go.GetComponent<Sputum> ().Fire (destX, destY, fireSpeed);
			timeBefore = Time.time;
		}
	}

}
