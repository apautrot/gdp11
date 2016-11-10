using UnityEngine;
using System.Collections;

public class Player : SceneSingleton<Player>
{
	new Rigidbody rigidbody;

	public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	public ForceMode MoveForceMode = ForceMode.Force;

	new void Awake ()
	{
		base.Awake ();
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Update ()
	{
		if ( Input.GetKey ( KeyCode.LeftArrow ) )		rigidbody.AddForce ( new Vector3 ( - moveSpeed.x, 0, 0 ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.RightArrow ) )		rigidbody.AddForce ( new Vector3 ( moveSpeed.x, 0, 0 ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.UpArrow ) )			rigidbody.AddForce ( new Vector3 ( 0, 0, moveSpeed.z ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.DownArrow ) )		rigidbody.AddForce ( new Vector3 ( 0, 0, - moveSpeed.z ), MoveForceMode );
	}
}
