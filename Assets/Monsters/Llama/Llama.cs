using UnityEngine;
using System.Collections;

public class Llama : Monster
{
	public GameObject prefab;
	public float targetSpeed = 50;
	public float accelerationDuration = 1;
	public float fireRate = 2;
	public float fireSpeed = 2;
	public float noFireArea;
	Animator anim;
	SpriteRenderer sprite;
	float destX, destY;
	float speed {
		get;
		set;
	}

	float timeBefore;

	new void Start ()
	{
		base.Start ();

		speed = 0;
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
		if (Time.time - timeBefore >= fireRate && Player.Instance.EnergyPoints > 0) {
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

	void Shoot ()
	{
        
        GameObject go = (GameObject)Instantiate(prefab);
		go.transform.position = transform.position;
        // Son de tirs
        Audio.Instance.PlaySound(AllSounds.Instance.Llama1);
        go.GetComponent<Sputum>().Fire(destX, destY, fireSpeed);

        anim.SetBool ("Sputum", false);
	}

}
