using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

// http://wiki.unity3d.com/index.php?title=Expose_properties_in_inspector

[ExecuteInEditMode]
public class TextMeshEx : MonoBehaviour
{
	[SerializeField, TextArea]
	private string _Text = "";

	public string Text
	{
		get { return _Text; }
		set
		{
			_Text = value;
			textRenderer.Text = value;
		}
	}


	[SerializeField]
	private float _CharacterSize = 1.0f;

	public float CharacterSize
	{
		get { return _CharacterSize; }
		set
		{
			_CharacterSize = value;
			textRenderer.Scale = value;
		}
	}


	[SerializeField]
	private float _LineMaxWidth = 0.0f;

	public float LineMaxWidth
	{
		get { return _LineMaxWidth; }
		set
		{
			_LineMaxWidth = value;
			textRenderer.LineMaxWidth = value > 0 ? value : float.MaxValue;
		}
	}


	[SerializeField]
	private TextAnchor _Anchor;

	public TextAnchor Anchor
	{
		get { return _Anchor; }
		set
		{
			_Anchor = value;
			textRenderer.Anchor = value;
		}
	}


	[SerializeField]
	private TextAlignment _Alignment;

	public TextAlignment Alignement
	{
		get { return _Alignment; }
		set
		{
			_Alignment = value;
			textRenderer.Alignment = value;
		}
	}


	[SerializeField]
	private Color _Color = UnityEngine.Color.white;

	public Color Color
	{
		get { return _Color; }
		set
		{
			_Color = value;
			textRenderer.Color = value;
		}
	}


	[SerializeField]
	private Font _Font;

	public Font Font
	{
		get { return _Font; }
		set
		{
			_Font = value;
			textRenderer.Font = value;
		}
	}

	[SerializeField]
	private Material _CustomMaterial;

	public Material CustomMaterial
	{
		get { return _CustomMaterial; }
		set
		{
			_CustomMaterial = value;
			// textRenderer.CustomMaterial = value;
		}
	}

	[SerializeField]
	private Vector2 _LetterSpacing;

	public Vector2 LetterSpacing
	{
		get { return _LetterSpacing; }
		set
		{
			_LetterSpacing = value;
			textRenderer.LetterSpacing = value;
		}
	}


	[SerializeField]
	private LetterSpacingMode _LetterSpacingMode;

	public LetterSpacingMode LetterSpacingMode
	{
		get { return _LetterSpacingMode; }
		set
		{
			_LetterSpacingMode = value;
			textRenderer.LetterSpacingMode = value;
		}
	}

	// public float OffsetZ;
	// public float LineSpacing = 1;
	// public bool RichText;

	private TextRenderer textRenderer = new TextRenderer ();
	public TextRenderer TextRenderer
	{
		get { return textRenderer; }
	}

	public float Height
	{
		get
		{
			return textRenderer.Bounds.size.y;
		}
	}

	void Awake ()
	{
		ForceRecreate ();
	}

	void Restart ()
	{
		ForceRecreate ();
	}

#if UNITY_EDITOR
	void OnValidate ()
	{
		if ( !gameObject.IsPrefab () )
			ForceRecreate ();
	}

	public void OnDrawGizmosSelected ()
	{
		Rect bounds = new Rect ( 0, 0, LineMaxWidth, textRenderer.Bounds.height );
		Gizmos.color = Color.yellow;
		Gizmos.matrix = Matrix4x4.TRS ( transform.position, transform.rotation, transform.lossyScale );

		switch ( Anchor )
		{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				break;

			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				bounds.position -= new Vector2 ( bounds.width / 2, 0 );
				break;

			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				bounds.position -= new Vector2 ( bounds.width, 0 );
				break;
		}

		switch ( Anchor )
		{
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperRight:
				bounds.position -= new Vector2 ( 0, bounds.height );	
				break;

			case TextAnchor.MiddleLeft:
			case TextAnchor.MiddleRight:
			case TextAnchor.MiddleCenter:
				bounds.position -= new Vector2 ( 0, bounds.height / 2 );
				break;
			
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerRight:
				break;
		}

		Gizmos.DrawLine
		(
			new Vector3 ( bounds.xMin, bounds.yMin, 0 ), 
			new Vector3 ( bounds.xMin, bounds.yMax, 0 )
		);

		Gizmos.DrawLine
		(
			new Vector3 ( bounds.xMax, bounds.yMin, 0 ),
			new Vector3 ( bounds.xMax, bounds.yMax, 0 )
		);

		Gizmos.matrix = Matrix4x4.identity;
		// Handles.RectangleCap ( 0, position, Quaternion.identity, bounds.size );
		// DrawGizmo ( Color.magenta ); //Color.Lerp ( Color.magenta, Color.clear, 0.25f ) );
	}

#endif

	public void ForceRecreate ()
	{
		textRenderer.Text = Text;
		textRenderer.Scale = CharacterSize;
		textRenderer.Alignment = Alignement;
		textRenderer.Anchor = Anchor;
		textRenderer.Color = Color;
		textRenderer.Font = Font;
		textRenderer.LineMaxWidth = LineMaxWidth > 0 ? LineMaxWidth : float.MaxValue;
		textRenderer.LetterSpacing = LetterSpacing;
		textRenderer.LetterSpacingMode = LetterSpacingMode;

		UpdateGeometry ();
	}

	public void UpdateGeometry ()
	{
		MeshFilter filter = gameObject.GetOrCreateComponent<MeshFilter> ();
		MeshRenderer renderer = gameObject.GetOrCreateComponent<MeshRenderer> ();

		if ( Font != null )
		{
			if ( Application.isEditor && !Application.isPlaying )
			{
				filter.sharedMesh = textRenderer.Mesh;
				renderer.sharedMaterial = _CustomMaterial != null ? _CustomMaterial : Font.material;
			}
			else
			{
				filter.mesh = textRenderer.Mesh;

 				if ( renderer.sharedMaterial == null )
					renderer.sharedMaterial = _CustomMaterial != null ? _CustomMaterial : Font.material;
			}
		}
		else
			filter.mesh = null;

#if UNITY_EDITOR
		if ( TextRenderer.errorMessage != null )
		{
			string path = name;

			Object parentObject = PrefabUtility.GetPrefabParent ( gameObject );
			if ( parentObject != null )
				path += " (" + AssetDatabase.GetAssetPath ( parentObject ) + ")";
			
			Debug.LogError ( "Text rendering error for object " + path + " in " + SceneManager.GetActiveScene().path );
		}
#endif
	}

	private void OnWillRenderObject ()
	{
#if UNITY_EDITOR
		if ( Application.isEditor && !Application.isPlaying )
			return;
#endif
		if ( textRenderer.IsDirty () )
			UpdateGeometry ();
	}

}