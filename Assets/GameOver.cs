using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{
	GameObject blackFade;

	void Awake ()
	{
		blackFade = gameObject.FindChildByName ( "Black Fade" );
		blackFade.SetActive ( false );
	}

	void Start ()
	{
		gameObject.FadeIn ();
	}

	void Update ()
	{
		if ( InputConfiguration.Instance.ActionA.IsJustUp )
		{
			blackFade.SetActive ( true );
			blackFade.SetAlpha ( 0 );
			blackFade.FadeIn ( 1 ).setOnCompleteHandler ( c => SceneManager.LoadScene ( "Test Scene Alexis" ) );
		}
	}
}
