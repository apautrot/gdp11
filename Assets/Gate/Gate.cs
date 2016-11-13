using UnityEngine;
using System.Collections;


public enum GateName
{
	Basique,
	Bleue,
	Cassée,
	Cloche,
	Colonne,
	Herbe,
	Jaune,
	Oeil,
	Orange,
	Orientale,
	Rouge,
	TriangleDroite,
	TriangleGauche,
	Verte
}

public class Gate : MonoBehaviour
{
	internal int SpawnListIndex = -1;
	bool isOpened;

	GameObject sprite;
	GameObject spriteA;
	GameObject spriteB;

	public GateName gateName = GateName.Basique;

	new Collider2D collider;

	void Awake ()
	{
		sprite = gameObject.FindChildByName ( "Sprite", false );
		spriteA = gameObject.FindChildByName ( "Sprite A", false );
		spriteB = gameObject.FindChildByName ( "Sprite B", false );
		//		spawnArea = gameObject.FindChildByName ( "Spawn Area" );
		collider = GetComponent<Collider2D> ();
	}

	public void Open ()
	{
		if ( !isOpened )
			StartCoroutine ( OpenDoor () );
	}

	IEnumerator OpenDoor ()
	{
		isOpened = true;

		float duration = AnimateOpening ();

		if ( collider != null )
			collider.enabled = false;

		//o Son d'une porte qui s'ouvre (nombre de son en fonction du nombre de porte
		switch ( gateName )
		{
			case GateName.Basique:
				Audio.Instance.PlaySound ( AllSounds.Instance.PorteBasique ); break;
			case GateName.Bleue:
				Audio.Instance.PlaySound ( AllSounds.Instance.PorteBleue ); break;
            case GateName.Cassée:
                Audio.Instance.PlaySound(AllSounds.Instance.Cassée); break;
            case GateName.Cloche:
                Audio.Instance.PlaySound(AllSounds.Instance.Cloche); break;
            case GateName.Colonne:
                Audio.Instance.PlaySound(AllSounds.Instance.Colonne); break;
            case GateName.Herbe:
                Audio.Instance.PlaySound(AllSounds.Instance.Herbe); break;
            case GateName.Jaune:
                Audio.Instance.PlaySound(AllSounds.Instance.Jaune); break;
            case GateName.Oeil:
                Audio.Instance.PlaySound(AllSounds.Instance.Oeil); break;
            case GateName.Orange:
                Audio.Instance.PlaySound(AllSounds.Instance.Orange); break;
            case GateName.Orientale:
                Audio.Instance.PlaySound(AllSounds.Instance.Orientale); break;
            case GateName.Rouge:
                Audio.Instance.PlaySound(AllSounds.Instance.Rouge); break;
            case GateName.TriangleDroite:
                Audio.Instance.PlaySound(AllSounds.Instance.TriangleDroite); break;
            case GateName.TriangleGauche:
                Audio.Instance.PlaySound(AllSounds.Instance.TriangleGauche); break;
            case GateName.Verte:
                Audio.Instance.PlaySound(AllSounds.Instance.Verte); break;

	
        }

		yield return new WaitForSeconds ( duration );

		if ( SpawnListIndex == -1 )
			Debug.LogError ( "This gate ( " + name + " has an invalid setup index ! Find GatesSetup game object and setup it." );
		else
			GatesSetup.Instance.Spawn ( this );

// 		if ( OnOpeningEnded != null )
// 			OnOpeningEnded ();
	}

	internal void SpawnItem ( ObjectType type )
	{
		GameObject prefab = Game.Instance.GetPrefab ( type );
		GameObject go = Game.Instance.gameObject.InstantiateSibling ( prefab );
		// go.transform.position = spawnArea.transform.position;

		go.transform.position = transform.position;
	}

	float AnimateOpening ()
	{
		AbstractGoTween tween = null;
		switch ( gateName )
		{
			case GateName.Basique:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( -70, 0, 0 ), true );
				break;

			case GateName.Bleue:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, -120, 0 ), true );
				break;

			case GateName.Cassée:
				{
					GoTweenChain chain = new GoTweenChain ();
					chain.insert ( 0, sprite.transform.localPositionTo ( 0.5f, new Vector3 ( -2, -3, 0 ) ) );
					chain.insert ( 0.5f, sprite.transform.localPositionTo ( 1.0f, new Vector3 ( -60, 770, 0 ) ) );
					chain.Start ();
					tween = chain;
				}
				break;

			case GateName.Cloche:
				{
					GoTweenChain chain = new GoTweenChain ();
					chain.insert ( 0, sprite.transform.localPositionTo ( 0.5f, new Vector3 ( 5, -5, 0 ) ) );
					chain.insert ( 0.5f, sprite.transform.localPositionTo ( 1.0f, new Vector3 ( -30, 55, 0 ) ) );
					chain.Start ();
					tween = chain;
				}
				break;

			case GateName.Colonne:
				{
					GoTweenChain chain = new GoTweenChain ();
					chain.insert ( 0, spriteA.transform.localPositionTo ( 1, new Vector3 ( -75, 10, 0 ) ) );
					chain.insert ( 0, spriteB.transform.localPositionTo ( 1, new Vector3 ( 95, 10, 0 ) ) );
					chain.insert ( 1, spriteA.transform.localPositionTo ( 1, new Vector3 ( -75, 95, 0 ) ) );
					chain.insert ( 1, spriteB.transform.localPositionTo ( 1, new Vector3 ( 105, 95, 0 ) ) );
					chain.Start ();
					tween = chain;
				}
				break;

			case GateName.Herbe:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, -40, 0 ) );
				break;

			case GateName.Jaune:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, -70, 0 ) );
				break;

			case GateName.Oeil:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, 85, 0 ) );
				break;

			case GateName.Orange:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 70, 0, 0 ) );
				break;

			case GateName.Orientale:
				{
					GoTweenChain chain = new GoTweenChain ();
					chain.insert ( 0, spriteA.transform.localPositionTo ( 1, new Vector3 ( 0, 50, 0 ), true ) );
					chain.insert ( 0, spriteB.transform.localPositionTo ( 1, new Vector3 ( 0, -50, 0 ), true ) );
					chain.Start ();
					tween = chain;
				}
				break;

			case GateName.Rouge:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, -70, 0 ), true );
				break;

			case GateName.TriangleDroite:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 65, 0, 0 ), true );
				break;

			case GateName.TriangleGauche:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( -70, 0, 0 ), true );
				break;

			case GateName.Verte:
				tween = sprite.transform.localPositionTo ( 1, new Vector3 ( 0, -65, 0 ), true );
				break;

			default:
				return 0;
		}

		return tween.totalDuration;
	}
}
