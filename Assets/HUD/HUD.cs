using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour {

	TextMesh roomLabel, timeLabel;
	List<GameObject> hearts;

	void Start () {
		roomLabel = RoomLabel.Instance.GetComponent<TextMesh> ();
		timeLabel = TimeLabel.Instance.GetComponent<TextMesh> ();
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

}
