using UnityEngine;
using System.Collections;

public class GameCamera : SceneSingleton<GameCamera> 
{
    public float maxHeight;
    public float maxWidth;

	new void Awake ()
    {
        maxHeight = this.GetComponent<Camera>().orthographicSize;
        maxWidth = maxHeight * this.GetComponent<Camera>().aspect;
        base.Awake ();
	}
}
