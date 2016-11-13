using UnityEngine;
using System.Collections;

public class Lock : MonoBehaviour
{
	AbstractGoTween rotateTween;

	void OnDestroy ()
	{
		if ( rotateTween != null )
			rotateTween.destroy ();
	}

	void Start()
	{
		transform.localEulerAngles = new Vector3 ( 0, 0, -15 );
		transform.scaleFrom ( 0.5f, 0 );
		rotateTween = transform.rotationTo ( 1, 15.0f ).loopsInfinitely ( GoLoopType.PingPong );
	}

}
