using UnityEngine;
using System.Collections;

public enum Direction
{
	None,
	Up,
	Right,
	Down,
	Left
}

public static class DirectionExtensions
{
	public static float ToRotationAngle ( this Direction self )
	{
		switch ( self )
		{
			case Direction.Up: return 0;
			case Direction.Left: return 90;
			case Direction.Down: return 180;
			case Direction.Right: return -90;
			default: return 0;
		}
	}
}


public class Player : SceneSingleton<Player>
{
	public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	public ForceMode MoveForceMode = ForceMode.Force;

	public GameObject UmbrellaPrefab;

	new Rigidbody2D rigidbody;
	IWeapon currentWeapon;

	GameObject frontPivot;

	Direction _direction;
	Direction direction
	{
		get
		{
			return _direction;
		}

		set
		{
			if ( _direction != value )
			{
				_direction = value;
				frontPivot.transform.localEulerAngles = new Vector3 ( 0, 0, direction.ToRotationAngle () );
			}
		}
	}

	new void Awake ()
	{
		base.Awake ();
		rigidbody = GetComponent<Rigidbody2D> ();
		frontPivot = gameObject.FindChildByName ( "Front Pivot" );
	}

	void Start ()
	{

	}

	void Update ()
	{
		if ( Input.GetKey ( KeyCode.LeftArrow ) || Input.GetAxis("Horizontal") < 0)
		{
			rigidbody.AddForce ( new Vector3 ( -moveSpeed.x, 0, 0 ), MoveForceMode );
			direction = Direction.Left;
		}

		if ( Input.GetKey ( KeyCode.RightArrow ) || Input.GetAxis("Horizontal") > 0)
		{
			rigidbody.AddForce ( new Vector3 ( moveSpeed.x, 0, 0 ), MoveForceMode );
			direction = Direction.Right;
		}

		if ( Input.GetKey ( KeyCode.UpArrow ) || Input.GetAxis("Vertical") > 0)
		{
			rigidbody.AddForce ( new Vector3 ( 0, moveSpeed.y, 0 ), MoveForceMode );
			direction = Direction.Up;
		}

		if ( Input.GetKey ( KeyCode.DownArrow ) || Input.GetAxis("Vertical") < 0)
		{
			rigidbody.AddForce ( new Vector3 ( 0, -moveSpeed.y, 0 ), MoveForceMode );
			direction = Direction.Down;
		}

		if ( Input.GetKey ( KeyCode.Space ))
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