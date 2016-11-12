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

	TextMesh roomLabel, timeLabel;
	List<GameObject> hearts;
	Player player;

	void Start ()
	{
		player = Player.Instance;
		player.OnEnergyPointChanged += OnEnergyPointChanged;
		roomLabel = GameObject.Find ("RoomLabel").GetComponent<TextMesh> ();
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

		SetRoom ("#1");
		SetLifes ( Player.Instance.EnergyPoints );
	}

	public void SetTime(int time) {
		this.timeLabel.text = (time / 60) + ":" + (time % 60);
	}

	void SetRoom(string room) {
		this.roomLabel.text = room;
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
