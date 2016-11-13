﻿using UnityEngine;
using System.Collections;

public class SectionCaterpillar : MonoBehaviour
{
	GameObject following;

    internal void SetParentSection ( GameObject parent )
    {
		following = parent;
		GetComponent <Joint2D> ().connectedBody = parent.GetComponent<Rigidbody2D> ();
	}
}
