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
	public float acceleration = 150;
	public float maximumSpeed = 100;
	public float decelerationFactor = 0.5f;
	Animator animator;

	// public Vector3 moveSpeed = new Vector3 ( 10, 10, 10 );
	// public ForceMode MoveForceMode = ForceMode.Force;

	public GameObject WeaponPrefab;

	const int MaximumEnergyPoints = 5;
	const int EnergyPointsAtStartOfGame = 5;

	internal int _energyPoints = 0;
	internal int EnergyPoints
	{
		get { return _energyPoints; }
		set
		{
			value = Mathf.Clamp ( value, 0, MaximumEnergyPoints );

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
			}
		}
	}

	new void Awake ()
	{
		base.Awake ();
		rigidbody = GetComponent<Rigidbody2D> ();
		animator = gameObject.FindChildByName("Sprite").GetComponent<Animator> ();
		animator.speed = 0.5f;
	}

	void Reawake ()
	{
		Awake ();
	}

	void Start ()
	{
		EnergyPoints = EnergyPointsAtStartOfGame;
	}

	void FixedUpdate()
	{
		bool isActivelyMoving = false;

		Vector2 addedVelocity = Vector2.zero;

		// if ( Input.GetKey ( KeyCode.LeftArrow ) || Input.GetAxis("Horizontal") < 0)
		if ( InputConfiguration.Instance.Left.IsDown )
		{
			isActivelyMoving = true;
			addedVelocity += Vector2.left;
			direction = Direction.Left;
			if ( animator != null )
			{
				animator.SetInteger ( "Direction", 1 );
				animator.SetBool ( "Walking", true );
			}
		
		}

		// if ( Input.GetKey ( KeyCode.RightArrow ) || Input.GetAxis("Horizontal") > 0)
		if ( InputConfiguration.Instance.Right.IsDown )
		{
			isActivelyMoving = true;
			addedVelocity += Vector2.right;
			direction = Direction.Right;
			if ( animator != null )
			{
				animator.SetInteger ( "Direction", 2 );
				animator.SetBool ( "Walking", true );
			}
		}

		// if ( Input.GetKey ( KeyCode.UpArrow ) || Input.GetAxis("Vertical") > 0)
		if ( InputConfiguration.Instance.Up.IsDown )
		{
			isActivelyMoving = true;
			addedVelocity += Vector2.up;
			direction = Direction.Up;
			if ( animator != null )
			{
				animator.SetInteger ( "Direction", 3 );
				animator.SetBool ( "Walking", true );
			}
		}

		// if ( Input.GetKey ( KeyCode.DownArrow ) || Input.GetAxis("Vertical") < 0)
		if ( InputConfiguration.Instance.Down.IsDown )
		{
			isActivelyMoving = true;
			addedVelocity += Vector2.down;
			direction = Direction.Down;
			if ( animator != null )
			{
				animator.SetInteger ( "Direction", 0 );
				animator.SetBool ( "Walking", true );
			}
		}

		rigidbody.velocity += ( addedVelocity.normalized * acceleration );

		// DebugWindow.ClearGroup ( "Player" );

		if ( rigidbody.velocity.magnitude > maximumSpeed )
		{
			rigidbody.velocity = rigidbody.velocity.normalized * maximumSpeed;
			// DebugWindow.Log ( "Player", "maximum speed reached", "" );
		}

		if ( rigidbody.velocity != Vector2.zero && !isActivelyMoving )
		{
			Vector2 deceleration = ( rigidbody.velocity * decelerationFactor * Time.fixedDeltaTime );
			// DebugWindow.Log ( "Player", "deceleration", deceleration.ToStringEx () );

			rigidbody.velocity -= deceleration;
		}

		// DebugWindow.Log ( "Player", "addedVelocity", addedVelocity.ToStringEx () );
		// DebugWindow.Log ( "Player", "rigidbody.velocity", rigidbody.velocity.ToStringEx () );

		if ( Input.GetKey ( KeyCode.Space ) )
			SpawnItem ();

		if ( Input.GetKey ( KeyCode.Return ) )
			OpenGate ();

		if ( Input.GetKeyDown ( KeyCode.A ) )
			DebugDrawRect ( new Vector3 ( 0, 0, 0 ), new Vector3 ( 50, 50, 0 ), Color.red, 1 );

		if ((rigidbody.velocity.x > 1 || rigidbody.velocity.x < -1) || (rigidbody.velocity.y > 1 || rigidbody.velocity.y < -1)) {
			animator.SetBool ("Walking", true);
		} else {
			animator.SetBool ("Walking", false);
		}

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
		if (collision.gameObject.GetComponent<Monster>())
		{
			rigidbody.velocity = (transform.position - collision.transform.position) * collision.gameObject.GetComponent<Monster>().damage * 10f;

			if (EnergyPoints <= 0) {
				animator.SetInteger ("Direction", 4);
			} else {
				EnergyPoints--;
			}
			//GameCamera.Instance.GetComponent<camera_shake> ().Shake (3);
		}
	}
}

