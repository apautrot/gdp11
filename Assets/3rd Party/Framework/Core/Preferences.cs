using System.Collections.Generic;
using UnityEngine;
using System.Text;



class Preferences
{
	private string path;
	Dictionary<string, string> values = new Dictionary<string, string> ();
	internal Dictionary<string, string> Values { get { return values; } }

	internal Preferences ( string path )
	{
		this.path = path;
	}

	internal void Clear ()
	{
		values.Clear ();
		PlayerPrefs.DeleteKey ( path );
	}

	private string EncodeValues ()
	{
		bool firstEntry = true;
		StringBuilder encodeValues = new StringBuilder();
		foreach ( var v in values )
		{
			string encodedKey = v.Key.Replace ( "|", "%|" ).Replace ( "=", "%=" );
			string encodedValue = v.Value.Replace ( "|", "%|" ).Replace ( "=", "%=" );
			
			if ( firstEntry )	firstEntry = false;
			else				encodeValues.Append ( "|" );
			
			encodeValues.Append ( encodedKey );
			encodeValues.Append ( "=" );
			encodeValues.Append ( encodedValue );
		}
		return encodeValues.ToString();
	}

	private void DecodeValues ( string encodedValues )
	{
		values.Clear ();

		if ( ( encodedValues == null ) || ( encodedValues.Length == 0 ) )
			return;

		char previous = (char)0;
		char c = (char)0;
		StringBuilder key = new StringBuilder ();
		StringBuilder value = new StringBuilder ();
		StringBuilder current = key;
		int length = encodedValues.Length;
		for ( int i = 0; i < length; i++ )
		{
			previous = c;
			c = encodedValues[i];
			if ( ( c == '=' ) && ( previous != '%' ) )
			{
				current = value;
			}
			else if ( ( c == '|' ) && ( previous != '%' ) )
			{
				values[key.ToString ()] = value.ToString ();
				key.Remove ( 0, key.Length );
				value.Remove ( 0, value.Length );
				current = key;
			}
			else
			{
				if ( previous == '%' )
					if ( ( c == '=' ) || ( c == '|' ) )
						current.Remove ( current.Length-1, 1 );	// remove previous %, that is prefix of encoding of %= or %|

				current.Append ( c );
			}
		}

		values[key.ToString ()] = value.ToString ();
	}

	internal void Load ()
	{
		string encodedValues = PlayerPrefs.GetString ( path, null );
		if ( encodedValues != null )
			DecodeValues ( encodedValues );
	}

	internal void Save ()
	{
		string encodedValues = EncodeValues ();
		PlayerPrefs.SetString ( path, encodedValues );
		PlayerPrefs.Save ();
	}

	internal bool GetBool ( string name, bool defaultValue = false )
	{
		string value;
		if ( !values.TryGetValue ( name, out value ) )
			return defaultValue;
		return value == "1";
	}

	internal int GetInt ( string name, int defaultValue = 0 )
	{
		string value;
		if ( !values.TryGetValue ( name, out value ) )
			return defaultValue;
		return int.Parse ( value );
	}

	internal string GetString ( string name, string defaultValue = null )
	{
		string value;
		if ( !values.TryGetValue ( name, out value ) )
			return defaultValue;
		return value;
	}

	internal void SetBool ( string name, bool value )
	{
		values[name] = value ? "1" : "0";
	}

	internal void SetInt ( string name, int value )
	{
		values[name] = value.ToString();
	}

	internal void SetString ( string name, string value )
	{
		values[name] = value;
	}

	internal bool GetOrCreateBool ( string name, bool defaultValue = false )
	{
		if ( !values.ContainsKey ( name ) )
			SetBool ( name, defaultValue );

		return GetBool ( name, defaultValue );
	}

	internal int GetOrCreateInt ( string name, int defaultValue = 0 )
	{
		if ( !values.ContainsKey ( name ) )
			SetInt ( name, defaultValue );

		return GetInt ( name, defaultValue );
	}

	internal string GetOrCreateString ( string name, string defaultValue = "" )
	{
		if ( !values.ContainsKey ( name ) )
			SetString ( name, defaultValue );

		return GetString ( name, defaultValue );
	}
}