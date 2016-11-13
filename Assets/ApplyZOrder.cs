using UnityEngine;
using System.Collections;

public class ApplyZOrder : MonoBehaviour
{
	void Update ()
	{
		transform.position = transform.position.WithZReplacedBy ( transform.position.y );
	}
}
