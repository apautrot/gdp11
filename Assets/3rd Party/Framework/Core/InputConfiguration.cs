using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InputButton
{
	bool wasDown;
	bool isDown;
	KeyCode[] keyCodes;

#if UNITY_EDITOR
	string name;
#endif

	internal void Load ( string name, Preferences preferences, params KeyCode[] keyCodes )
	{
#if UNITY_EDITOR
		this.name = name;
#endif

		for ( int i = 0; i < keyCodes.Length; i++ )
			keyCodes[i] = (KeyCode)preferences.GetOrCreateInt ( name + ".KeyCode" + i, (int)keyCodes[i] );
		this.keyCodes = keyCodes;
	}

	internal void Update ()
	{
		for ( int i = 0; i < keyCodes.Length; i++ )
			if ( Input.GetKey ( keyCodes[i] ) )
			{
				IsDown = true;
				return;
			}

		IsDown = false;
	}

	internal bool IsDown
	{
		get { return isDown; }
		private set
		{
			wasDown = isDown;
			isDown = value;
		}
	}
	internal bool IsJustDown { get { return isDown && !wasDown; } }
	internal bool IsJustUp { get { return !isDown && wasDown; } }

#if UNITY_EDITOR
	public override string ToString ()
	{
		return name + "[" + isDown + "] - " +  keyCodes.ToString();
	}
#endif

}


public class InputConfiguration : Singleton<InputConfiguration>
{
	public delegate void ConfigurationChange ();
	public event ConfigurationChange OnConfigurationChanged;

	Preferences preferences = new Preferences ( "Input" );

	List<InputButton> inputs = new List<InputButton>();

	InputButton RegisterInput ( string name, params KeyCode[] keyCodes )
	{
		InputButton inputButton = new InputButton ();
		inputButton.Load ( name, preferences, keyCodes );
		inputs.Add ( inputButton );
		return inputButton;
	}

	void ClearAllInputs ()
	{
		inputs.Clear ();
	}

	InputButton TopLeft;
	InputButton TopRight;
	InputButton BottomLeft;
	InputButton BottomRight;
	internal InputButton Action;
	internal InputButton Cancel;
	internal InputButton Left;
	internal InputButton Right;
	internal InputButton Up;
	internal InputButton Down;

	internal Direction	MoveDirection;
	internal Direction	ShootDirection = Direction.None;
	internal Vector2	FreeDirection;

	void Awake ()
	{
		preferences.Load ();
		ReadKeyCodesFromPreferences ();
	}

	void Reawake ()
	{
		Awake ();
	}

	void ReadKeyCodesFromPreferences()
	{
		ClearAllInputs ();

		TopLeft =		RegisterInput ( "TopLeft", KeyCode.Keypad4 );
		TopRight =		RegisterInput ( "TopRight", KeyCode.Keypad5 );
		BottomLeft =	RegisterInput ( "BottomLeft", KeyCode.Keypad1 );
		BottomRight =	RegisterInput ( "BottomRight", KeyCode.Keypad2 );
		Action =		RegisterInput ( "Action", KeyCode.Space, KeyCode.Return, KeyCode.JoystickButton0 );
		Cancel =		RegisterInput ( "Cancel", KeyCode.Escape, KeyCode.JoystickButton1 );
		Left =			RegisterInput ( "Left", KeyCode.LeftArrow, KeyCode.Keypad4 );
		Right =			RegisterInput ( "Right", KeyCode.RightArrow, KeyCode.Keypad6 );
		Up =			RegisterInput ( "Top", KeyCode.UpArrow, KeyCode.Keypad8 );
		Down =			RegisterInput ( "Bottom", KeyCode.DownArrow, KeyCode.Keypad2 );
	}

	const float moveThreshold = 0.3f;
	const float shootThreshold = 0.15f;

	const float freemoveThreshold = 0.15f;

	internal void Update ()
	{
		for ( int i = 0; i < inputs.Count; i++ )
			inputs[i].Update ();

// 		FreeDirection = GetAxisVector ( "LS X Axis", "LS Y Axis", freemoveThreshold );
// 		if ( FreeDirection == Vector2.zero )
// 			FreeDirection = GetAxisVector ( "DPad X Axis", "DPad Y Axis", freemoveThreshold );
// 		if ( FreeDirection == Vector2.zero )
// 			FreeDirection = GetAxisVector ( "RS X Axis", "RS Y Axis", freemoveThreshold );
// 		if ( FreeDirection == Vector2.zero )
// 			FreeDirection = GetCardinalDirection ( Up, Down, Left, Right );
	}

	internal KeyCode GetInputKeyCode ( string inputName, int keyCodeIndex = 0 )
	{
		return (KeyCode) preferences.GetInt ( inputName + ".KeyCode" + keyCodeIndex, (int)KeyCode.None );
	}

	internal void SetInputKeyCode ( string inputName, KeyCode keyCode, int keyCodeIndex = 0 )
	{
		preferences.SetInt ( inputName + ".KeyCode" + keyCodeIndex, (int)keyCode );
		preferences.Save ();

		ReadKeyCodesFromPreferences ();

		if ( OnConfigurationChanged != null )
			OnConfigurationChanged ();
	}

	const float axisThreshold = 0.05f;

	Vector2 GetCardinalDirection ( InputButton up, InputButton down, InputButton left, InputButton right )
	{
		Vector2 direction = Vector2.zero;
		if ( up.IsDown ) direction += Vector2.up;
		if ( down.IsDown ) direction += Vector2.down;
		if ( left.IsDown ) direction += Vector2.left;
		if ( right.IsDown ) direction += Vector2.right;
		return direction;
	}

	Vector2 GetAxisVector ( string horizontalAxisName, string verticalAxisName, float threshold )
	{
		float horizontal = Input.GetAxis ( horizontalAxisName );
		float vertical = Input.GetAxis ( verticalAxisName );

		if ( ( Mathf.Abs ( horizontal ) > threshold ) || ( Mathf.Abs ( vertical ) > threshold ) )
			return new Vector2 ( horizontal, vertical );
		else
			return Vector2.zero;
	}
}
