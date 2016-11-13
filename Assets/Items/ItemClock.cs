using UnityEngine;
using System.Collections;

public class ItemClock : ItemBase
{
	protected override void ApplyEffect ()
	{
		Audio.Instance.PlaySound ( AllSounds.Instance.GetItem );
		Game.Instance.time += 5;
	}
}
