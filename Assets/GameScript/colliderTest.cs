using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class colliderTest : MonoBehaviour
{
    public Collider rootCollider;
    public Collider childCollider;

    private void Start()
    {
        Physics.IgnoreCollision(rootCollider, childCollider, true);
    }
}
