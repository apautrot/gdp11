using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class FontData
{
	public float CapHeight
	{
		get;
		private set;
	}

	public float XHeight
	{
		get;
		private set;
	}

	public float AscenderHeight
	{
		get;
		private set;
	}

	public float DescenderHeight
	{
		get;
		private set;
	}

	public float FontHeight
	{
		// get { return AscenderHeight + XHeight + DescenderHeight; }
		get { return CapHeight + DescenderHeight; }
	}

	public float LineHeight
	{
		get;
		private set;
	}

	public Font Font
	{
		get;
		private set;
	}

	private string xHeightLetters = "xeaonsrcumvwz";
	public string XHeightLetters
	{
		get { return xHeightLetters; }
		set
		{
			xHeightLetters = value;
			ComputeXHeightAndAscenderHeight ();
			ComputeDescenderHeight (); // dependent of xHeight
		}
	}

	private void ComputeXHeightAndAscenderHeight ()
	{
		XHeight = 0;
		for ( int i = 0 ; i < xHeightLetters.Length; i++ )
		{
			char c = xHeightLetters[i];
			CharacterInfo ci;
			if ( Font.GetCharacterInfo ( c, out ci ) )
			{
				XHeight = System.Math.Abs ( ci.maxY - ci.minY );
				AscenderHeight = System.Math.Abs ( ci.maxY - ci.minY ) - System.Math.Abs ( ci.minY );
				return;
			}
		}
	}

	private string capLetters = "MNBDCEFKAGHIJLOPQRSTUVWXYZ";
	public string CapLetters
	{
		get { return capLetters; }
		set
		{
			capLetters = value;
			ComputeCapHeight ();
		}
	}

	private void ComputeCapHeight ()
	{
		CapHeight = 0;
		if ( Font != null )
			for ( int i = 0; i < capLetters.Length; i++ )
			{
				char c = capLetters[i];
				CharacterInfo ci;
				if ( Font.GetCharacterInfo ( c, out ci ) )
				{
					CapHeight = System.Math.Abs ( ci.maxY - ci.minY );
					return;
				}
			}
	}

	private string descenderLetters = "gpqyj";
	public string DescenderLetters
	{
		get { return descenderLetters; }
		set
		{
			descenderLetters = value;
			ComputeDescenderHeight ();
		}
	}

	private void ComputeDescenderHeight ()
	{
		DescenderHeight = 0;
		for ( int i = 0; i < descenderLetters.Length; i++ )
		{
			char c = descenderLetters[i];
			CharacterInfo ci;
			if ( Font.GetCharacterInfo ( c, out ci ) )
			{
				DescenderHeight = System.Math.Abs ( ci.maxY - ci.minY ) - XHeight;
				return;
			}
		}
	}

	private void ComputeLineHeight ()
	{
		LineHeight = FontHeight * 1.1f; // 1.62f;
	}

	public FontData ( Font font )
	{
		Font = font;
		ComputeFontData ();
	}

	internal void ComputeFontData()
	{
// 		string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 .,:;!?-+/*&'\"()[]îéêèàâçùûô";
// 		Font.RequestCharactersInTexture ( alphabet );
		ComputeCapHeight ();
		ComputeXHeightAndAscenderHeight ();
		ComputeDescenderHeight (); // dependent of xHeight
		ComputeLineHeight ();
	}
}



public enum TextAlignment
{
	Left,
	Center,
	Right,
	Justify
}

public enum LetterSpacingMode
{
	Add,
	Replace
}

public class TextRenderer
{
	public class LineDescriptor
	{
		internal int charStartIndexInText;
		internal int charCount;
		internal int quadStartIndexInBatch;
		internal int quadCount;
		internal bool issuedFromLineCut;

		internal LineDescriptor ( int startIndexInText, int length, bool issuedFromLineCut )
		{
			this.charStartIndexInText = startIndexInText;
			this.charCount = length;
			this.issuedFromLineCut = issuedFromLineCut;
		}

		internal LineDescriptor ( int startIndexInText, int length, int quadIndexInBatch, int quadCount, bool issuedFromLineCut )
		{
			this.charStartIndexInText = startIndexInText;
			this.charCount = length;
			this.quadStartIndexInBatch = quadIndexInBatch;
			this.quadCount = quadCount;
			this.issuedFromLineCut = issuedFromLineCut;
		}
	}

	protected Font font;
//	protected Material customMaterial;
	protected FontData fontData;
	protected QuadBatch quadBatch = new QuadBatch ();
	protected Mesh mesh;
	protected Vector3 position = new Vector3 ();
	protected float scale = 1;
	protected TextAlignment alignment = TextAlignment.Left;
	protected float lineMaxWidth = float.MaxValue;
	protected Vector2 letterSpacing;
	protected LetterSpacingMode letterSpacingMode;
	protected bool preserveWords = true;
	protected Color color = Color.white;
	protected List<LineDescriptor> lineList = new List<LineDescriptor> ();
	protected string text = "";
	protected Rect bounds = new Rect ();
	// protected bool trimTopLeftSpace = true;
	protected float rotation = 0.0f;

	private bool quadBatchUpdateNeeded = true;
// 	private bool _quadBatchUpdateNeeded = true;
// 	private bool quadBatchUpdateNeeded { get { return _quadBatchUpdateNeeded; }
// 		set { _quadBatchUpdateNeeded = value; } }

	private bool meshUpdateNeeded = true;
// 	private bool _meshUpdateNeeded = true;
// 	private bool meshUpdateNeeded { get { return _meshUpdateNeeded; }
// 		set { _meshUpdateNeeded = value; } }

	public TextRenderer ()
	{
	}

	public TextRenderer ( Font font )
	{
		Font = font;
	}

	public TextRenderer ( TextRenderer text )
	{
		Set ( text );
	}

	public QuadBatch QuadBatch
	{
		get
		{
			RecreateQuadBatchIfNeeded ();
			return quadBatch;
		}
		set
		{
			quadBatch = value;
			meshUpdateNeeded = true;
		}
	}

	public void RecreateQuadBatchIfNeeded ()
	{
		if ( quadBatchUpdateNeeded )
		{
			RecreateQuadBatch ();
			meshUpdateNeeded = true;
			quadBatchUpdateNeeded = false;
		}
	}

	public Font Font
	{
		get { return font; }
		set
		{
			if ( font != null )
				Font.textureRebuilt -= OnFontTextureRebuilt;

			this.font = value;

			if ( font != null )
			{
				// if ( FontDepot.InstanceCreated )	// happened once during a TextMeshEx.Validate during DockArea.OnGUI
				fontData = FontDepot.Instance.GetData ( font );
				Font.textureRebuilt += OnFontTextureRebuilt;
			}
			quadBatchUpdateNeeded = true;
		}
	}

// 	public Material CustomMaterial
// 	{
// 		get { return customMaterial; }
// 		set
// 		{
// 			customMaterial = value;
// 		}
// 	}

	private void OnFontTextureRebuilt ( Font font )
	{
		if ( font == this.font )
			quadBatchUpdateNeeded = true;
	}

	void Set ( TextRenderer bitmapText )
	{
		this.font = bitmapText.font;
		this.fontData = bitmapText.fontData;
		this.scale = bitmapText.scale;
		this.color = bitmapText.color;
		this.lineMaxWidth = bitmapText.lineMaxWidth;
		this.alignment = bitmapText.alignment;
		this.text = bitmapText.text;
//		this.setPosition ( bitmapText.getX (), bitmapText.getY () );
	}

	public float LineMaxWidth
	{
		get { return lineMaxWidth; }
		set
		{
			lineMaxWidth = value;
			quadBatchUpdateNeeded = true;
		}
	}

	public LetterSpacingMode LetterSpacingMode
	{
		get { return letterSpacingMode; }
		set
		{
			letterSpacingMode = value;
			quadBatchUpdateNeeded = true;
		}
	}

	public Vector2 LetterSpacing
	{
		get { return letterSpacing; }
		set
		{
			letterSpacing = value;
			quadBatchUpdateNeeded = true;
		}
	}

	public float Scale
	{
		get { return scale; }
		set
		{
			scale = value;
			quadBatchUpdateNeeded = true;
		}
	}

// 	public bool TrimTopLeftSpace
// 	{
// 		get { return trimTopLeftSpace; }
// 		set
// 		{
// 			trimTopLeftSpace = value;
// 			updateNeeded = true;
// 		}
// 	}

	public void SetEmptyText ()
	{
		text = "";
		quadBatch.setSize ( 0 );
		bounds = new Rect();
		quadBatchUpdateNeeded = true;
	}

	public Color Color
	{
		get { return color; }
		set
		{
			color = value;
			quadBatch.colorize ( 0, quadBatch.Count, color );
			meshUpdateNeeded = true;
		}
	}

	public float Alpha
	{
		get { return color.a; }
		set
		{
			color.a = value;
			quadBatch.colorize ( 0, quadBatch.Count, color );
			meshUpdateNeeded = true;
		}
	}

	public void SetCharColor ( int index, Color color )
	{
		quadBatch.colorize ( index, 1, color );
		meshUpdateNeeded = true;
	}

	public void SetLineColor ( int index, Color color )
	{
		LineDescriptor line = lineList[index];
		quadBatch.colorize ( line.quadStartIndexInBatch, line.quadCount, color );
		meshUpdateNeeded = true;
	}

	public TextAlignment Alignment
	{
		get { return alignment; }
		set
		{
			alignment = value;
			quadBatchUpdateNeeded = true;
		}
	}

	private TextAnchor anchor;
	public TextAnchor Anchor
	{
		get { return anchor; }
		set
		{
			anchor = value;
			quadBatchUpdateNeeded = true;
		}
	}

	public float LineHeight
	{
		get { return fontData.LineHeight * scale; }
		set
		{
			scale = value / fontData.LineHeight;
			quadBatchUpdateNeeded = true;
		}
	}

	public bool IsDirty ()
	{
		return ( meshUpdateNeeded || quadBatchUpdateNeeded );
	}

	public Mesh Mesh
	{
		get
		{
			if ( quadBatchUpdateNeeded )
				RecreateQuadBatchIfNeeded ();

			if ( meshUpdateNeeded )
			{
				mesh = QuadBatch.CreateMesh ( mesh );
				if ( mesh != null )
					mesh.hideFlags = HideFlags.DontSave;

				meshUpdateNeeded = false;
			}

			return mesh;
		}
	}

	public string Text
	{
		get { return text; }
		set
		{
			text = value;
			quadBatchUpdateNeeded = true;
		}
	}

	private class CreateTextGeometryResult
	{
		public int processedCharCount;
		public int renderedCharCount;
		public int spaceRemovedAtBeginning;
	}

#if UNITY_EDITOR
	public static string errorMessage;
#endif

	private CreateTextGeometryResult createTextGeometry ( Vector3 position, string text, int startIndex, int length, int quadStartIndex, bool lineIsIssuedFromLineCut, float scaleFactor, float lineMaxWidth, bool preserveWords )
	{
		CreateTextGeometryResult result = new CreateTextGeometryResult ();

		if ( lineIsIssuedFromLineCut )
		{
			// trim spaces at start
			while ( ( startIndex < startIndex + length ) && ( ( text[startIndex] == 0x20 ) || ( text[startIndex] == 0xA0 ) ) )
			{
				startIndex++;
				length--;
				result.processedCharCount++;
				result.spaceRemovedAtBeginning++;
			}
		}

		// 		float pageWidth = font.getRegion ().getRegionWidth ();
		// 		float pageHeight = font.getRegion ().getRegionHeight ();
		// 		Glyph glyph = null;
		CharacterInfo glyph;
		// Font.fontChar letterStruct = null;
		Vector3 letterPosition = new Vector3 ();
		Vector2 letterDimension = new Vector2 ();
		Vector2 letterTopLeftUV = new Vector2 ();
		Vector2 letterTopRightUV = new Vector2 ();
		Vector2 letterBottomLeftUV = new Vector2 ();
		Vector2 letterBottomRightUV = new Vector2 ();
		float advance = 0;
		int lastSpaceIndex = startIndex;
		for ( int i = startIndex; i < startIndex + length; i++ )
		{
			char charToRender = text[i];
			if ( charToRender == 0xA0 )
				charToRender = (char)0x20; // convert unbreakable space into space

			if ( !font.GetCharacterInfo ( charToRender, out glyph ) )
			{
#if UNITY_EDITOR
				if ( charToRender != 0x0D )
				{
					if ( errorMessage == null )
						errorMessage = "";

					errorMessage += "Font character unknown : '" + charToRender + "' (0x" + ( (int)charToRender ).ToString ( "X2" ) + "), used in text \"" + text + "\"" + "\n";
					// Debug.LogWarning ( "Font character unknown : '" + charToRender + "' (0x" + ( (int)charToRender ).ToString ( "X2" ) + "), used in text \"" + text + "\"" );
					if ( !font.GetCharacterInfo ( ' ', out glyph ) )
						errorMessage += "Font default replacement character [space] not defined.";
						// Debug.LogError ( "Font default replacement character [space] not defined." );
				}
#endif
			}

			// 			glyph = font.getData ().getGlyph ( charToRender );
			// 			if (glyph == null)
			// 				// throw new RuntimeException ( "Character '" + charToRender + "' is not defined in font" );
			// 				glyph = font.getData ().getGlyph ( ' ' );

			int kerning = 0; // kerning to add
			// if (i > 0)
			// {
			// char previousCharToRender = text.charAt ( i - 1 );
			// if (previousCharToRender == 0xA0)
			// previousCharToRender = (char) 0x20; // convert unbreakable space into space
			//
			// font.kerningTable.TryGetValue ( new KeyValuePair<char, char> ( previousCharToRender, charToRender ), out kerning );
			// }

			if ( charToRender == 0x20 )
			{
				lastSpaceIndex = i; // keep index of last space
// 
// 				// space special case
// 				letterPosition.x = advance + kerning + glyph.minX;
// 				letterPosition.y = ( glyph.minY + glyph.maxY - glyph.minY );
// 				letterDimension.x = (float)glyph.maxX - glyph.minX;
// 				letterDimension.y = glyph.maxY - glyph.minY; // 0.0f;
// 				letterTopLeftUV = glyph.uvTopLeft;
// 				letterTopRightUV = glyph.uvTopRight;
// 				letterBottomLeftUV = glyph.uvBottomLeft;
// 				letterBottomRightUV = glyph.uvBottomRight;
			}
// 			else
 			{
				letterPosition.x = advance + kerning + glyph.minX;
				letterPosition.y = ( glyph.minY + glyph.maxY - glyph.minY );
				letterDimension.x = glyph.maxX - glyph.minX;
				letterDimension.y = -( glyph.maxY - glyph.minY );
				letterTopLeftUV = glyph.uvTopLeft;
				letterTopRightUV = glyph.uvTopRight;
				letterBottomLeftUV = glyph.uvBottomLeft;
				letterBottomRightUV = glyph.uvBottomRight;
			}
			
			letterPosition *= scaleFactor; // apply scaling
			letterDimension *= scaleFactor;

			// check if text is over the max line width
			if ( letterPosition.x + letterDimension.x > lineMaxWidth )
			{
				// generation is not finished, return count of processed characters
				if ( preserveWords && ( lastSpaceIndex > startIndex ) )
				{
					// cut where the last space char was found, will truncate and discard all chars after
					result.processedCharCount -= ( i - lastSpaceIndex );
					result.renderedCharCount -= ( i - lastSpaceIndex );
					break;
				}
				else
				{
					break; // line break in the middle of a word
				}
			}

			letterPosition += position; // then offset by position

			// if ( charToRender != 0x20 )
				quadBatch.setQuad ( quadStartIndex + result.renderedCharCount, letterPosition, letterDimension, letterTopLeftUV, letterTopRightUV, letterBottomLeftUV, letterBottomRightUV, color );
//  			if ( glyph.flipped )
//  				quadBatch.flipUvs ( quadStartIndex + result.renderedCharCount );

			if ( letterSpacingMode == LetterSpacingMode.Add )
				advance += ( glyph.advance + letterSpacing.x );
			else
				advance +=					letterSpacing.x;

			result.processedCharCount++;
			result.renderedCharCount++;
		}

		return result;
	}

	public void RecreateQuadBatch ()
	{
		if ( font == null )
			return;

		//
		// first, cut into lines
		//

		// the line list is a list of pair <first char index, line text>
		lineList.Clear ();
		{
 			int lineStartIndex = 0;
			int lineBreakIndex = 0;
			int length = text.Length;
			while ( lineBreakIndex < length )
			{
				char c = text[lineBreakIndex];
				bool antiSlashN = ( c == '\n' );
				bool antiSlashR = ( c == '\r' );
				if ( antiSlashN || antiSlashR )
				{
					lineList.Add ( new LineDescriptor ( lineStartIndex, ( lineBreakIndex - lineStartIndex ), false ) );

					if (	antiSlashR								// if \r is found
						&&	( lineBreakIndex < length - 1 )
						&&	( text[lineBreakIndex + 1] == '\n' ) )	// skip possible \n following it
						lineBreakIndex++;

					lineStartIndex = lineBreakIndex + 1; // start index of next line
				}
				lineBreakIndex++;
			}
			lineList.Add ( new LineDescriptor ( lineStartIndex, ( text.Length - lineStartIndex ), false ) );

// 			int startIndex = 0;
// 			int lineBreakIndex = -1;
// 			while ( ( lineBreakIndex = text.IndexOf ( '\n', startIndex ) ) != -1 )
// 			{
// 				lineList.Add ( new LineDescriptor ( startIndex, lineBreakIndex - startIndex, false ) );
// 				startIndex = lineBreakIndex + 1; // start index of next line
// 			}
// 			lineList.Add ( new LineDescriptor ( startIndex, text.Length - startIndex, false ) );
		}

#if UNITY_EDITOR
		errorMessage = null;
#endif

		//
		// new create lines geometry
		//

		quadBatch.setSize ( text.Length );

		int lineIndex = 0;
		Vector3 linePosition = new Vector3 ( position.x, position.y, position.z );
		int quadStartIndex = 0;
		while ( lineIndex < lineList.Count )
		{
			// TODO : optim : "lineList.get( lineIndex )" as local ?
			int startIndex = lineList[lineIndex].charStartIndexInText;
			int length = lineList[lineIndex].charCount;
			bool issuedFromLineCut = lineList[lineIndex].issuedFromLineCut;

			CreateTextGeometryResult result = createTextGeometry ( linePosition, text, startIndex, length, quadStartIndex, issuedFromLineCut, scale, lineMaxWidth, preserveWords );

			if ( ( result.processedCharCount > 0 ) && ( result.processedCharCount != length ) ) // line not fully processed ? (see line break near end of
			// createTextGeometry method)
			{
				// insert remaining of the line
				lineList.Insert ( lineIndex + 1, new LineDescriptor ( startIndex + result.processedCharCount, length - result.processedCharCount, true ) );
			}

			// updates line description
			lineList[lineIndex] = new LineDescriptor ( startIndex + result.spaceRemovedAtBeginning, result.processedCharCount - result.spaceRemovedAtBeginning, quadStartIndex, result.renderedCharCount, issuedFromLineCut );

			quadStartIndex += result.renderedCharCount; // moves into the quad buffer

			if ( fontData != null )
				if ( letterSpacingMode == LetterSpacingMode.Add )
					linePosition.y -= ( ( fontData.LineHeight +	letterSpacing.y ) * scale );
				else
					linePosition.y -= (							letterSpacing.y * scale );
				// TODO : might be an option instead of lineHeight : result.boundingRectMax.Y

			// result.boundingRectMin.Y;
			lineIndex++;
		}

#if UNITY_EDITOR
		if ( errorMessage != null )
			Debug.LogError ( "Invalid char found in text to render :\n" + errorMessage );
#endif

		quadBatch.setSize ( quadStartIndex );

		//
		// align each line's geometry
		//

		bounds = new Rect ( 0, 0, -1, -1 );

		if ( lineList.Count > 1 )
		{
			float lineWidth = lineMaxWidth;

			if (
				( lineMaxWidth == float.MaxValue )
			||	( alignment == TextAlignment.Center )	// we also want real width when centering
				)
			{
				bounds = quadBatch.getBounds ();
				lineWidth = bounds.width;
			}

			switch ( alignment )
			{
				case TextAlignment.Left:
					// default alignment
					break;

				case TextAlignment.Center:
					for ( int i = 0; i < lineList.Count; i++ )
					{
						LineDescriptor lineDesc = lineList[i];
						Rect boundingRect = quadBatch.getBounds ( lineDesc.quadStartIndexInBatch, lineDesc.quadCount );
						quadBatch.translate ( lineDesc.quadStartIndexInBatch, lineDesc.quadCount, new Vector3 ( ( lineWidth - boundingRect.width ) / 2, 0, 0 ) );
					}
					break;

				case TextAlignment.Right:
					for ( int i = 0; i < lineList.Count; i++ )
					{
						LineDescriptor lineDesc = lineList[i];
						Rect boundingRect = quadBatch.getBounds ( lineDesc.quadStartIndexInBatch, lineDesc.quadCount );
						quadBatch.translate ( lineDesc.quadStartIndexInBatch, lineDesc.quadCount, new Vector3 ( lineWidth - boundingRect.width, 0, 0 ) );
					}
					break;

				case TextAlignment.Justify:
					for ( int i = 0; i < lineList.Count; i++ )
					{
						LineDescriptor lineDesc = lineList[i];

						// line is cut if next line is issued from the cut
						bool lineWasCut = ( i < lineList.Count - 1 ) && ( lineList[i + 1].issuedFromLineCut );

						// TODO : add right/centered aligned justify as Photoshop do ?
						if ( lineWasCut ) // only align line that are cut, others are already left aligned
						{
							// get space count in that line
							List<int> charCountsToProcess = new List<int> ();
							int noSpaceCharCount = 0;
							for ( int j = 0; j < lineDesc.charCount; j++ )
							{
								char charToRender = text[lineDesc.charStartIndexInText + j];
								if ( charToRender == 0xA0 )
									charToRender = (char)0x20; // convert unbreakable space into space

								if ( charToRender == 0x20 )
								{
									charCountsToProcess.Add ( noSpaceCharCount );
									noSpaceCharCount = 0;
								}
								else
									noSpaceCharCount++;
							}
							charCountsToProcess.Add ( noSpaceCharCount );
							if ( charCountsToProcess.Count > 1 )
							{
								Rect boundingRect = quadBatch.getBounds ( lineDesc.quadStartIndexInBatch, lineDesc.quadCount );
								float spaceToAdd = ( lineWidth - boundingRect.width ) / ( charCountsToProcess.Count - 1 );

								if ( spaceToAdd > 0 )
								{
									// distribute translation to each spacing
									int quadIndex = lineDesc.quadStartIndexInBatch;
									int quadCount = 0;
									for ( int j = 0; j < charCountsToProcess.Count; j++ )
									{
										quadCount = charCountsToProcess[j];
										if ( quadCount > 0 )
										{
											// don't process first word, that stay left aligned
											if ( j > 0 )
											{
												// quadBatch.colorize ( quadIndex, 1, Color.RED ); // DEBUG : colorize first letter in red
												quadBatch.translate ( quadIndex, quadCount, new Vector3 ( spaceToAdd * j, 0, 0 ) );
											}
											quadIndex += charCountsToProcess[j];
										}

										quadIndex++; // skip space char, that have not to be processed
									}
								}
							}
						}
					}
					break;
			}
		}

		if ( bounds.width == -1 || bounds.height == -1 ) // might have been already computed above, don't do it twice
			bounds = quadBatch.getBounds ();

// 		if ( trimTopLeftSpace ) // remove empty top left space created by glyph xoffset/yoffset
// 		{
// 			position.x = quadBounds.x;
// 			position.y = quadBounds.y;
// 			bounds.x = quadBounds.x;
// 			bounds.y = quadBounds.y;
// 		}

		float dx = 0;
		switch ( anchor )
		{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				// dx = - bounds.width / 2;
				dx = -bounds.xMin;
				break;

			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				// dx = - bounds.width / 2;
				dx = -bounds.center.x;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				// dx = - bounds.width;
				dx = -bounds.xMax;
				break;
		}

		// float dy = -( bounds.y + bounds.height );
		float dy = 0;
		switch ( anchor )
		{
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperRight:
				// dy += bounds.height / 2;
				dy = -bounds.yMax;
				break;
			case TextAnchor.MiddleCenter:
			case TextAnchor.MiddleLeft:
			case TextAnchor.MiddleRight:
				// dy += bounds.height / 2;
				dy = -bounds.center.y;
				break;
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerRight:
				// dy += bounds.height;
				dy = -bounds.yMin;
				break;
		}

		quadBatch.translate ( new Vector3 ( dx, dy, 0 ) );
	}

	public void AlignVerticesOnIntegers ()
	{
		quadBatch.alignVerticesOnIntegers ();
	}

	public Rect Bounds
	{
		get
		{
			RecreateQuadBatchIfNeeded ();
			return bounds;
		}
	}

// 	public float getLineBaseHeight ()
// 	{
// 		if ( multiline == true )
// 			return -1;
// 
// 		string ypqgj = "ypqgj";
// 
// 		float baseLineHeight = 0.0f;
// 		for ( int i = 0; i < text.Length; i++ )
// 		{
// 			char c = text[i];
// 			float height = 0;
// 			if ( char.IsLower ( c ) && ( ypqgj.IndexOf ( c ) != -1 ) )
// 				height = fontData.XHeight;
// 			else
// 			{
// 				CharacterInfo ci = font.characterInfo[0];
// 				// 				Glyph glyph = font.getData ().getGlyph ( c );
// 				// 				height = glyph.height /* + glyph.yoffset* /;
// 			}
// 
// 			baseLineHeight = System.Math.Max ( baseLineHeight, height );
// 		}
// 
// 		return baseLineHeight * scale;
// 	}
}

