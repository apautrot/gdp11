using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FontDepot : Singleton<FontDepot>
{
	private Dictionary<Font, FontData> fontData = new Dictionary<Font, FontData> ();

	public FontData GetData ( Font font )
	{
		FontData fontData = null;
		this.fontData.TryGetValue ( font, out fontData );
		if ( fontData == null )
		{
			fontData = new FontData ( font );
			this.fontData[font] = fontData;
		}

#if UNITY_EDITOR
		// fix problems with font texture rebuild
		if ( Application.isEditor && ! Application.isPlaying )
			fontData.ComputeFontData ();
#endif

		return fontData;
	}

	void Awake ()
	{
		hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
		Font.textureRebuilt += OnFontTextureRebuilt;
	}

	private void OnFontTextureRebuilt ( Font font )
	{
		FontData fontData = null;
		if ( this.fontData.TryGetValue ( font, out fontData ) )
		{
			Debug.Log ( "Found a rebuilt font : " + font.ToString () );
			fontData.ComputeFontData ();
		}
	}
}