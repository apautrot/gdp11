using UnityEngine;
using System.Collections;


public enum GoTweenState
{
	Running,
	Paused,
	Complete,	// we could remove this state very easily, I don't get the point of it
	Destroyed
}
