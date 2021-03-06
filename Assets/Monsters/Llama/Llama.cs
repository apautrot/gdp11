﻿using UnityEngine;
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

    AudioClip[] audioLama;
    AudioClip[] audioLamaDie;

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
        audioLama = new AudioClip[] { (AllSounds.Instance.Llama1), (AllSounds.Instance.Llama2), (AllSounds.Instance.Llama3) };
        audioLamaDie = new AudioClip[] { (AllSounds.Instance.LlamaDies1), (AllSounds.Instance.LlamaDies2)};

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
                
                this.WaitAndDo (0.85f, Shoot);

            }
		}
		anim.SetInteger ("Direction", direction);
		//Debug.Log (anim.GetBool("Sputum"));
		//Debug.Log (anim.GetInteger("Direction"));
		//Debug.Log(anim.GetBool("Sputum"));
	}

	void Shoot ()
	{
        
        GameObject go = (GameObject)Instantiate(prefab);
		go.transform.position = transform.position;
        // Son de tirs
        Audio.Instance.PlaySound(AllSounds.Instance.PlayThisClip(audioLama));

        go.GetComponent<Sputum>().Fire(destX, destY, fireSpeed);

        anim.SetBool ("Sputum", false);
	}

    protected override void ApplySound()
    {
        Audio.Instance.PlaySound(AllSounds.Instance.PlayThisClip(audioLamaDie));
    }

}
