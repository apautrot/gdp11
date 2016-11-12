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

	public static Vector2 ToVector2 ( this Direction self )
	{
		switch ( self )
		{
			case Direction.Up: return Vector2.up;
			case Direction.Left: return Vector2.left;
			case Direction.Down: return Vector2.down;
			case Direction.Right: return Vector2.right;
			default: return Vector2.zero;
		}
	}
}


public class Player : SceneSingleton<Player>
{
	public float moveSpeed = 50;
	// public float velocityDecreasePerSecond = 50;
	public float decelerationFactorPerSec = 0.5f;
	Vector2 velocity = Vector2.zero;
	Animator animator;

	// public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	// public ForceMode MoveForceMode = ForceMode.Force;

	public GameObject WeaponPrefab;

	internal int _energyPoints = 5;
	internal int EnergyPoints
	{
		get { return _energyPoints; }
		set
		{
			if ( _energyPoints != value )
			{
				int previousValue = _energyPoints;
				_energyPoints = value;
				if ( OnEnergyPointChanged != null )
					OnEnergyPointChanged ( previousValue, _energyPoints );
			}
		}
	}
	internal System.Action<int, int> OnEnergyPointChanged;

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
		animator = gameObject.FindChildByName("Sprite").GetComponent<Animator> ();
	}

	void Start ()
	{

	}

	void Update ()
	{

		if ( Input.GetKey ( KeyCode.LeftArrow ) || Input.GetAxis("Horizontal") < 0)
		{
			velocity += Vector2.left;
			direction = Direction.Left;
			animator.SetInteger ("Direction", 1);
			animator.SetBool ("Walking", true);
		
		}

		if ( Input.GetKey ( KeyCode.RightArrow ) || Input.GetAxis("Horizontal") > 0)
		{
			velocity += Vector2.right;
			direction = Direction.Right;
			animator.SetInteger ("Direction", 2);
			animator.SetBool ("Walking", true);
		}

		if ( Input.GetKey ( KeyCode.UpArrow ) || Input.GetAxis("Vertical") > 0)
		{
			velocity += Vector2.up;
			direction = Direction.Up;
			animator.SetInteger ("Direction", 3);
			animator.SetBool ("Walking", true);
		}

		if ( Input.GetKey ( KeyCode.DownArrow ) || Input.GetAxis("Vertical") < 0)
		{
			velocity += Vector2.down;
			direction = Direction.Down;
			animator.SetInteger ("Direction", 0);
			animator.SetBool ("Walking", true);
		}

		if (Input.GetKeyUp ( KeyCode.DownArrow ) || Input.GetKeyUp ( KeyCode.UpArrow ) || Input.GetKeyUp ( KeyCode.LeftArrow ) || Input.GetKeyUp ( KeyCode.RightArrow )) {
			
		}

			
		rigidbody.velocity = ( velocity.normalized * moveSpeed );
		velocity *= 0.25f;

		if ( Input.GetKey ( KeyCode.Space ) )
			SpawnItem ();

		if ( Input.GetKey ( KeyCode.Return ) )
			OpenGate ();

		if ( Input.GetKeyDown ( KeyCode.A ) )
			DebugDrawRect ( new Vector3 ( 0, 0, 0 ), new Vector3 ( 50, 50, 0 ), Color.red, 1 );


	}

	void SpawnItem ()
	{
		if ( currentWeapon == null )
		{
			if ( WeaponPrefab != null )
			{
				GameObject weaponGO = gameObject.InstantiateChild ( WeaponPrefab );
				weaponGO.transform.localPosition = new Vector3 ( 0, 32, 0 );

				currentWeapon = weaponGO.GetComponentAs<IWeapon> ();
				currentWeapon.OnEnd += OnWeaponEffectEnd;
			}
		}
	}

	void OpenGate ()
	{
		Vector3 hitTestCenterPosition = transform.position + (Vector3) ( direction.ToVector2 () * 50 );

		RaycastHit2D[] hits = Physics2D.CircleCastAll ( hitTestCenterPosition, 200, Vector2.up, 0 );
		for ( int i = 0; i < hits.Length; i++ )
		{
			RaycastHit2D hit = hits[i];
			if ( hit.collider.isTrigger )
			{
				Gate gate = hit.collider.gameObject.GetComponent<Gate> ();
				if ( gate != null )
				{
					gate.Open ();
				}
			}
		}
	}

	void OnWeaponEffectEnd ()
	{
		currentWeapon = null;
	}

	static void DebugDrawRect ( Vector3 from, Vector3 to, Color color, float duration = 0 )
	{
		Debug.DrawLine ( from, to.WithYReplacedBy ( from.y ), color, duration );
		Debug.DrawLine ( from.WithYReplacedBy ( to.y ), to, color, duration );
		Debug.DrawLine ( from, to.WithXReplacedBy ( from.y ), color, duration );
		Debug.DrawLine ( from.WithXReplacedBy ( to.y ), to, color, duration );
	}

	void OnCollisionEnter2D (Collision2D collision) {
		if (collision.gameObject.GetComponent<Monster>()) {
			velocity = (transform.position - collision.transform.position) * collision.gameObject.GetComponent<Monster>().damage * 1.5f;
			EnergyPoints--;
			GameCamera.Instance.GetComponent<camera_shake> ().Shake (3);
		}
	}
}

