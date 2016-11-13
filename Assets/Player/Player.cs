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
	public float invincibilityTime = 3f;
	private float lastDamage;
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

	public bool Movable, Hurtable;

	AudioSource walkAudioInstance;

	new void Awake ()
	{
		base.Awake ();
		rigidbody = GetComponent<Rigidbody2D> ();
		animator = gameObject.FindChildByName("Sprite").GetComponent<Animator> ();
		animator.speed = 0.5f;
		Movable = true;
		Hurtable = true;
		lastDamage = Time.time;
	}

	void Reawake ()
	{
		Awake ();
	}

	void Start ()
	{
		EnergyPoints = EnergyPointsAtStartOfGame;
	}

	void Update ()
	{
		if ( InputConfiguration.Instance.ActionA.IsJustDown )
			UseWeapon ();

		if ( InputConfiguration.Instance.ActionB.IsJustDown )
			OpenGate ();
	}

	void FixedUpdate()
	{
		bool isActivelyMoving = false;

		Vector2 addedVelocity = Vector2.zero;

		InputConfiguration inputs = InputConfiguration.Instance;
		if ( inputs != null )
		{

			if ( inputs.Left.IsDown && Movable )
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

			if ( inputs.Right.IsDown && Movable )
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

			if ( inputs.Up.IsDown && Movable )
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

			if ( inputs.Down.IsDown && Movable )
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
		}

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

		if ((rigidbody.velocity.x > 1 || rigidbody.velocity.x < -1) || (rigidbody.velocity.y > 1 || rigidbody.velocity.y < -1))
		{
			if ( walkAudioInstance == null )
			{
				walkAudioInstance = Audio.Instance.PlaySound ( AllSounds.Instance.PlayerWalk1 );
				if ( walkAudioInstance != null )
					walkAudioInstance.loop = true;
			}

			animator.SetBool ("Walking", true);
		}
		else
		{
			if ( walkAudioInstance != null )
			{
				walkAudioInstance.Stop ();
				walkAudioInstance = null;
			}

			animator.SetBool ("Walking", false);
		}

		//On check si le joueur a été frappé il y a moins de x secondes
		if (Time.time - lastDamage >= invincibilityTime) {
			Hurtable = true;
		}

	}

	void UseWeapon ()
	{
		if ( currentWeapon == null )
		{
			if ( WeaponPrefab != null )
			{
				GameObject weaponGO = gameObject.InstantiateSibling ( WeaponPrefab );
				weaponGO.transform.position = gameObject.transform.position + (Vector3) ( direction.ToVector2() * 64 );

				currentWeapon = weaponGO.GetComponentAs<IWeapon> ();
				currentWeapon.OnEnd += OnWeaponEffectEnd;
				currentWeapon.Throw ( direction );
			}
		}
	}

	void OpenGate ()
	{
		Vector3 hitTestCenterPosition = transform.position + (Vector3) ( direction.ToVector2 () * 50 );

		Gate nearest = null;
		RaycastHit2D[] hits = Physics2D.CircleCastAll ( hitTestCenterPosition, 200, Vector2.up, 0 );
		for ( int i = 0; i < hits.Length; i++ )
		{
			RaycastHit2D hit = hits[i];
			Gate gate = hit.collider.gameObject.GetComponent<Gate> ();
			if ( gate != null )
			{
				if ( nearest == null )
					nearest = gate;
				else
					if ( gate.transform.DistanceTo ( gameObject ) < nearest.transform.DistanceTo ( gameObject ) )
						nearest = gate;
			}
		}

		if ( nearest != null )
			nearest.Open ();
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

	void OnCollisionEnter2D (Collision2D collision)
	{
		if (collision.gameObject.GetComponent<Monster>())
		{
			rigidbody.velocity = ( transform.position - collision.transform.position ) * 1500f;

			if (Hurtable) {
                Audio.Instance.PlaySound(AllSounds.Instance.PlayerTakesDamage1);
                lastDamage = Time.time;
				Hurtable = false;
				//GetComponent<SpriteRenderer> ().color.a;
				if (EnergyPoints <= 0) {
					Die();
				} else {
					EnergyPoints--;
				}
			}
			//GameCamera.Instance.GetComponent<camera_shake> ().Shake (3);
		}
	}

	public void Die() {
		animator.SetBool ("Dead", true);
		animator.Play ("Die");
        Audio.Instance.PlaySound(AllSounds.Instance.PlayerDies1);
        Movable = false;
        EnergyPoints = 0;
        GetComponent<CircleCollider2D> ().enabled = false;
	}
}

