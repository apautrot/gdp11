using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
	GameObject blackFade;
	bool waitForStartButton;

	void Awake ()
	{
		blackFade = gameObject.FindChildByName ( "Black Fade" );
	}

	void Start ()
	{
		if ( Music.InstanceCreated )
			Music.Instance.ReinitializeVariables ();

		blackFade.SetActive ( true );
		blackFade.FadeOut ( 1, FadeOutEndAction.DoNothing ).setOnCompleteHandler ( c => waitForStartButton = true );
	}

	void Update ()
	{
		if ( waitForStartButton )
			if ( InputConfiguration.Instance.ActionA.IsJustUp )
			{
				blackFade.SetActive ( true );
				blackFade.SetAlpha ( 0 );
				blackFade.FadeIn ( 1 ).setOnCompleteHandler ( c => SceneManager.LoadScene ( "Game Scene" ) );
			}
	}
}
