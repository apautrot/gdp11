using UnityEngine;
using System.Collections;

public class Caterpillar : Monster
{
    public GameObject sectionPrefab;
    
    public float queueLength = 5;
	public float timeBetweenQueueSectionCreation = 0.25f;
	Vector3 targetPosition;

    public float speed = 100;

    float maxHeight;
    float maxWidth;

    Animator anim;
    SpriteRenderer sprite;

	Vector3 previousPosition;
    AudioClip[] audioPop;

    new void Start()
    {
		base.Start ();

        maxHeight = GameCamera.Instance.maxHeight;
        maxWidth = GameCamera.Instance.maxWidth;

        sprite = gameObject.FindChildByName("Sprite").GetComponent<SpriteRenderer>();
        anim = gameObject.FindChildByName("Sprite").GetComponent<Animator>();

		previousPosition = transform.position;

        audioPop = new AudioClip[] { (AllSounds.Instance.CaterpillarPop1), (AllSounds.Instance.CaterpillarPop2), (AllSounds.Instance.CaterpillarPop3) };

        StartCoroutine( CreateQueueCoroutine() );
		StartCoroutine ( AnimateCoroutine() );
		StartCoroutine ( CheckIfBlocked () );
	}

	IEnumerator CreateQueueCoroutine ()
	{
		GameObject lastSectionInstantiated = gameObject;

		for ( int i = 0; i < queueLength; i++ )
		{
            Audio.Instance.PlaySound(AllSounds.Instance.PlayThisClip(audioPop));
            GameObject go = GameObject.Instantiate ( sectionPrefab );
			go.transform.position = transform.position;
			go.GetComponent<SectionCaterpillar> ().SetParentSection ( lastSectionInstantiated );
			lastSectionInstantiated = go;

			// Son de pop des morceaux de la chenille

			yield return new WaitForSeconds ( timeBetweenQueueSectionCreation );
		}
	}

	IEnumerator AnimateCoroutine()
    {
        while ( true )
        {
			this.NewPosition();

			if (Player.Instance.EnergyPoints > 0)
            	body.velocity = ( targetPosition - transform.position ).normalized * speed;

            if (direction == 2)
                sprite.flipX = true;

            if (direction == 1)
                sprite.flipX = false;

            anim.SetInteger("Direction", direction);

            yield return new WaitForSeconds(1f);
        }
    }

	IEnumerator CheckIfBlocked ()
	{
		while ( true )
		{
			yield return new WaitForSeconds ( 0.5f );

			float distanceDone = transform.position.DistanceTo ( previousPosition );
			// Debug.Log ( "Check si bloqué.. distance done :" + distanceDone );

			if ( distanceDone < 15 )
			{
				// Debug.Log ( "Bloqué, change direction" );
				NewPosition ();
			}

			previousPosition = transform.position;
		}
	}

    void NewPosition()
    {
        targetPosition = new Vector3(Random.Range(-maxHeight, maxHeight), Random.Range(-maxWidth, maxWidth), 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NewPosition();
    }
}
