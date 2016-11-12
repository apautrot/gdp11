using UnityEngine;
using System.Collections;

public class GameCamera : SceneSingleton<GameCamera> 
{
    internal float maxHeight;
	internal float maxWidth;

	new void Awake ()
    {
        maxHeight = this.GetComponent<Camera>().orthographicSize;
        maxWidth = maxHeight * this.GetComponent<Camera>().aspect;
        base.Awake ();
	}
}
