using UnityEngine;
using System.Collections;

public class Llama : MonoBehaviour {
	public GameObject prefab;
	public float targetSpeed, accelerationDuration, fireRate, fireSpeed, noFireArea;
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
		//Acceleration
		this.floatTo("speed", accelerationDuration, targetSpeed, false);
	}

	void FixedUpdate() {
		destX = Player.Instance.transform.position.x;
		destY = Player.Instance.transform.position.y;

		body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;

		//Tire quand le firerate est dépassé
		if (Time.time - timeBefore >= fireRate) {
			if (Vector3.Distance(new Vector3(destX, destY, 0), transform.position) > noFireArea) {
				GameObject go = (GameObject)Instantiate(prefab);
				go.transform.position = transform.position;
				go.GetComponent<Sputum> ().Fire (destX, destY, fireSpeed);
				timeBefore = Time.time;
			}
		}
	}

}
