using UnityEngine;
using System.Collections;

public class PlayerTest : MonoBehaviour {

	new Rigidbody2D rigidbody;

	public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	public ForceMode2D MoveForceMode = ForceMode2D.Force;

	new void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	void Update ()
	{
		if ( Input.GetKey ( KeyCode.LeftArrow ) )		rigidbody.AddForce ( new Vector2 ( - moveSpeed.x, 0), MoveForceMode );
		if ( Input.GetKey ( KeyCode.RightArrow ) )		rigidbody.AddForce ( new Vector2 ( moveSpeed.x, 0), MoveForceMode );
		if ( Input.GetKey ( KeyCode.UpArrow ) )			rigidbody.AddForce ( new Vector2 ( 0, moveSpeed.y), MoveForceMode );
		if ( Input.GetKey ( KeyCode.DownArrow ) )		rigidbody.AddForce ( new Vector2 ( 0, -moveSpeed.x), MoveForceMode );
	}
}
