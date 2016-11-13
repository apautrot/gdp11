using UnityEngine;
using System.Collections;

public class ItemKey : ItemBase
{
	protected override void ApplyEffect ()
	{
		Audio.Instance.PlaySound ( AllSounds.Instance.GetKey );
		Player.Instance.IsHavingKey = true;
	}
}
