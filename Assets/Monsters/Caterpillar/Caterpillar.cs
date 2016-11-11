using UnityEngine;
using System.Collections;

public class Caterpillar : MonoBehaviour {

    Rigidbody2D body;
    GameObject sprite;
    public GameObject prefab;
    
    public float caterpillarLength = 5;
    public float targetSpeed, accelerationDuration;
    private float destX, destY;
    float speed
    {
        get;
        set;
    }
    float timeBefore;

    void Start()
    {
        speed = 0;
        body = GetComponent<Rigidbody2D>();
        timeBefore = Time.time;
        //Acceleration
        this.floatTo("speed", accelerationDuration, targetSpeed, false);
        NewPosition();

    }

    void FixedUpdate()
    {
        if ((int)this.transform.position.x == (int)destX && (int)this.transform.position.y == (int)destY)
            NewPosition();

        body.velocity = (new Vector3(destX, destY, 0) - transform.position).normalized * speed;
    }

    void NewPosition()
    {
        float maxHeight = GameCamera.Instance.maxHeight;
        float maxWidth = GameCamera.Instance.maxWidth;
        destX = Random.Range(-maxHeight, maxHeight);
        destY = Random.Range(-maxWidth, maxWidth);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        NewPosition();
    }
}
