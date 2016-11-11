using UnityEngine;
using System.Collections;

public class SectionCaterpillar : MonoBehaviour {

    public float speed;
    Rigidbody2D body;
    internal GameObject following;

    void Awake()
    {
        speed = 50;
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        body.velocity = (following.transform.position - transform.position).normalized * speed;

    }
}
