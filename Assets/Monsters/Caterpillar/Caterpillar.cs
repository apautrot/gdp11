using UnityEngine;
using System.Collections;

public class Caterpillar : MonoBehaviour {

    Rigidbody2D body;
    public GameObject sectionPrefab;
    
    public float caterpillarLength = 5;
    public float targetSpeed, accelerationDuration;
    Vector3 positionCaterpillar;

    public float speed;

    float maxHeight;
    float maxWidth;

    float countSection;

    void Start()
    {
        speed = 100;
        countSection = 0;
        body = GetComponent<Rigidbody2D>();
        maxHeight = GameCamera.Instance.maxHeight;
        maxWidth = GameCamera.Instance.maxWidth;

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

            body.velocity = (positionCaterpillar - transform.position).normalized * speed;

            if (countSection < caterpillarLength)
            {
                GameObject go = GameObject.Instantiate(sectionPrefab);
                go.transform.position = new Vector3(transform.position.x + 100, transform.position.y + 100, 0);
                go.GetComponent<SectionCaterpillar>().following = lastSectionInstantiated;
                lastSectionInstantiated = go;
                countSection++;
            }
        
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
