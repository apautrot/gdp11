using UnityEngine;
using System.Collections;

public class Player : SceneSingleton<Player>
{
	new Rigidbody2D rigidbody;

	public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	public ForceMode MoveForceMode = ForceMode.Force;

	public GameObject UmbrellaPrefab;

	IWeapon currentWeapon;

	new void Awake ()
	{
		base.Awake ();
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate ()
	{
	}

	void Update ()
	{
		if ( Input.GetKey ( KeyCode.LeftArrow ) )		rigidbody.AddForce ( new Vector3 ( - moveSpeed.x, 0, 0 ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.RightArrow ) )		rigidbody.AddForce ( new Vector3 ( moveSpeed.x, 0, 0 ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.UpArrow ) )			rigidbody.AddForce ( new Vector3 ( 0, moveSpeed.y, 0 ), MoveForceMode );
		if ( Input.GetKey ( KeyCode.DownArrow ) )		rigidbody.AddForce ( new Vector3 ( 0, -moveSpeed.y, 0 ), MoveForceMode );

		if ( Input.GetKey ( KeyCode.Space ) )
			SpawnItem ();
	}

	void SpawnItem ()
	{
		if ( currentWeapon == null )
		{
			GameObject weaponGO = gameObject.InstantiateChild ( UmbrellaPrefab );
			weaponGO.transform.localPosition = new Vector3 ( 0, 32, 0 );

			currentWeapon = weaponGO.GetComponentAs<IWeapon> ();
			currentWeapon.OnEnd += OnWeaponEffectEnd;
		}
	}

	void OnWeaponEffectEnd ()
	{
		currentWeapon = null;
	}
}