using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DebugWindow : SceneSingleton<DebugWindow>
{
    private Dictionary<string, Dictionary<string, string>> values = new Dictionary<string, Dictionary<string, string>>();
    private bool isVisible = true;
#if UNITY_EDITOR
	private bool isDisplayed = true;
#else
	private bool isDisplayed = false;
#endif

    public Font Font = null;
	public int FontSize = 0;
	private GUIStyle style;

	public static void Log ( string group, string key, bool value )
	{
		Log ( group, key, value.ToString () );
	}

	public static void Log ( string group, string key, int value )
	{
		Log ( group, key, value.ToString () );
	}

	public static void Log ( string group, string key, float value )
	{
		Log ( group, key, value.ToString () );
	}

	public static void Log ( string group, string key, Vector3 value )
	{
		Log ( group, key, value.ToString () );
	}

	public static void Log ( string group, string key, string value )
	{
		if ( InstanceCreated )
			Instance.AddEntry ( group, key, value );
	}

	public static void Clear ()
	{
		if ( InstanceCreated )
			Instance.values.Clear ();
	}

	public static void ClearGroup ( string group )
	{
		if ( InstanceCreated )
			Instance.RemoveGroup ( group );
	}

	void AddEntry (string group, string key, string value )
    {
        if ( ! values.ContainsKey ( group ) )
        {
            values[group] = new Dictionary<string, string>();
        }
        values[group][key] = value;
    }

	public void RemoveGroup ( string group )
	{
		values.Remove ( group );
	}

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1) == true)
        {
            isVisible = !isVisible;
        }
    }

	private void DrawLabel ( Rect rect, string text, GUIStyle style )
	{
		Rect shadowRect = new Rect ( rect );
		shadowRect.x++;
		shadowRect.y++;

		GUI.color = new Color ( 0,0,0, 1 );
		GUI.Label ( shadowRect, text, style );
		GUI.color = new Color ( 1, 1, 1, 1 );
		GUI.Label ( rect, text, style );
	}

	private void OnGUI ()
	{
		if ( isDisplayed )
		{
			if ( Font != null )
				GUI.skin.font = Font;

			if ( style == null )
			{
				style = new GUIStyle ( GUI.skin.label );
				style.padding = new UnityEngine.RectOffset ();

				if ( Font != null )
					style.font = Font;

				if ( ( style.font == null ) || style.font.dynamic )
					if ( FontSize != 0 )
						style.fontSize = FontSize;
			}

			GUI.color = new Color ( 1, 1, 1, 1 );

			if ( !isVisible )
				DrawLabel ( new Rect ( 10, 5, 800, 25 ), "press F1 to debug", style );
			else
			{
				float y = 5;

				if ( values.Count == 0 )
					DrawLabel ( new Rect ( 10, 5, 800, 25 ), "empty debug window", style );
				else
				{
					foreach ( KeyValuePair<string, Dictionary<string, string>> entry in values )
					{
						{
							string text = entry.Key;
							Vector2 size = style.CalcSize ( new GUIContent ( text ) );
							DrawLabel ( new Rect ( 10, y, size.x, size.y ), text, style );
							y += size.y;
						}

						foreach ( KeyValuePair<string, string> kvp in entry.Value )
						{
							float x = 25;
							{
								string text = kvp.Key + ": ";
								Vector2 size = style.CalcSize ( new GUIContent ( text ) );
								DrawLabel ( new Rect ( x, y, size.x, size.y ), text, style );
								x += size.x;
							}

							{
								string text = kvp.Value;
								float height = style.CalcHeight ( new GUIContent ( text ), 800 );
								DrawLabel ( new Rect ( x, y, 800, height ), text, style );
								y += height;
							}
						}
					}
				}
			}
		}
	}
}
