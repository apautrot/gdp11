using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : SceneSingleton<HUD>
{
	public float initialHeartApparitionDelay = 0.25f;
	public float heartScaleInDuration = 0.5f;
	public float heartScaleOutDuration = 0.25f;
	public GoEaseType heartScaleInEase = GoEaseType.BounceIn;
	public GoEaseType heartScaleOutEase = GoEaseType.BounceOut;

	TextMesh timeLabel;
	List<GameObject> hearts;
	Player player;

	void Start ()
	{
		player = Player.Instance;
		player.OnEnergyPointChanged += OnEnergyPointChanged;
		timeLabel = GameObject.Find ("TimeLabel").GetComponent<TextMesh> ();
		hearts = new List<GameObject> ();
		hearts.Add(GameObject.Find ("Heart 1"));
		hearts.Add(GameObject.Find ("Heart 2"));
		hearts.Add(GameObject.Find ("Heart 3"));
		hearts.Add(GameObject.Find ("Heart 4"));
		hearts.Add(GameObject.Find ("Heart 5"));
		for ( int i = 0; i < hearts.Count; i++ )
		{
			hearts[i].SetActive ( false );
			ShowHeart ( hearts[i], i * initialHeartApparitionDelay );
		}

		SetLifes ( Player.Instance.EnergyPoints );
	}

	public void SetTime(int time) {
		string secs = (time % 60).ToString(), mins = (time / 60).ToString();
		if (time % 60 < 10)
			secs = "0" + (time % 60);
		if (time / 60 < 10)
			mins = "0" + time / 60;
		this.timeLabel.text = mins + ":" + secs;

		if (time <= 5) {
			timeLabel.color = new Color (255, 0, 0);
			timeLabel.transform.SetScale (0.7f);
			timeLabel.transform.scaleTo( heartScaleInDuration, 1 ).eases(heartScaleInEase);
		}
			
	}

	private void ShowHeart ( GameObject heart, float delay = 0 )
	{
		heart.SetActive ( true );
		heart.transform.SetScale ( 0 );
		heart.transform.scaleTo ( heartScaleInDuration, 1 )
			.eases ( heartScaleInEase )
			.delays ( delay );
	}

	private void HideHeart ( GameObject heart )
	{
		heart.transform.scaleTo ( heartScaleOutDuration, 0 )
		.eases ( heartScaleOutEase )
		.setOnCompleteHandler ( c => heart.SetActive ( false ) );
	}

	void SetLifes (int lifes)
	{
		for (int i = 0; i < hearts.Count; i++)
		{
			GameObject heart = hearts[i];
			bool isToBeActivated = ( i < lifes );
			bool isActive = heart.activeSelf;

			if ( isToBeActivated && ! isActive )
				ShowHeart ( heart );
			else if ( ! isToBeActivated && isActive )
				HideHeart ( heart );
		}
	}

	void OnEnergyPointChanged ( int previous, int current )
	{
		SetLifes (current);
	}

}
