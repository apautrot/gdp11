using UnityEngine;
using System.Collections;

public class ApplyZOrder : MonoBehaviour
{
	void FixedUpdate ()
	{
		transform.position = transform.position.WithZReplacedBy ( transform.position.y );
	}
}
