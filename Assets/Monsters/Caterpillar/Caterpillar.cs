using UnityEngine;
using System.Collections;

public class Caterpillar : Monster
{
    public GameObject sectionPrefab;
    
    public float caterpillarLength = 5;
    public float targetSpeed, accelerationDuration;
    Vector3 positionCaterpillar;

    public float speed;

    float maxHeight;
    float maxWidth;

    float countSection;

    Animator anim;
    SpriteRenderer sprite;

    new void Start()
    {
		base.Start ();

        speed = 100;
        countSection = 0;
        maxHeight = GameCamera.Instance.maxHeight;
        maxWidth = GameCamera.Instance.maxWidth;

        sprite = gameObject.FindChildByName("Sprite").GetComponent<SpriteRenderer>();
        anim = gameObject.FindChildByName("Sprite").GetComponent<Animator>();

        StartCoroutine(AnimateCoroutine());
    }

    IEnumerator AnimateCoroutine()
    {
        GameObject lastSectionInstantiated = gameObject;
        this.NewPosition();
        while (true)
        {
            if ((int)this.transform.position.x == (int)positionCaterpillar.x && (int)this.transform.position.y == (int)positionCaterpillar.y)
                this.NewPosition();

			if (Player.Instance.EnergyPoints > 0)
            	body.velocity = (positionCaterpillar - transform.position).normalized * speed;

            if (countSection < caterpillarLength)
            {
                GameObject go = GameObject.Instantiate(sectionPrefab);
                go.transform.position = new Vector3(transform.position.x + 100, transform.position.y + 100, 0);
                go.GetComponent<SectionCaterpillar>().following = lastSectionInstantiated;
                lastSectionInstantiated = go;

                // Son de pop des morceaux de la chenille

                countSection++;
            }

            if (direction == 2)
                sprite.flipX = true;

            if (direction == 1)
                sprite.flipX = false;

            anim.SetInteger("Direction", direction);

            yield return new WaitForSeconds(1f);
        }
    }

    void NewPosition()
    {
        positionCaterpillar = new Vector3(Random.Range(-maxHeight, maxHeight), Random.Range(-maxWidth, maxWidth), 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NewPosition();
    }
}
