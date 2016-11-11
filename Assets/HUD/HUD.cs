using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour {

	TextMesh roomLabel, timeLabel;
	List<GameObject> hearts;
	Player player;

	void Start () {
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
		setRoom ("Room #1");
		setTime ("04:33");
		setLifes (5);
	}

	void setTime(string time) {
		this.timeLabel.text = time;
	}

	void setRoom(string room) {
		this.roomLabel.text = room;
	}

	void setLifes(int lifes) {
		for (int i = 0; i < hearts.Count; i++) {
			if (i < lifes) {
				hearts [i].SetActive (true);
			} else {
				hearts [i].SetActive (false);
			}
		}
	}

	void OnEnergyPointChanged (int previous, int current) {
		setLifes (current);
	}

}
