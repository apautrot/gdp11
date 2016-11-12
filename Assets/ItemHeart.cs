using UnityEngine;
using System.Collections;

public class ItemHeart : MonoBehaviour
{
	void OnTriggerEnter2D ( Collider2D collider )
	{
		if ( collider.GetComponent<Player> () != null )
		{
			Player.Instance.EnergyPoints++;
			gameObject.DestroySelf ();
		}
	}
}
