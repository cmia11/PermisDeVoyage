using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bump : MonoBehaviour
{
    public Rigidbody2D target;
    public Vector3 bumpDirection = new Vector3(1, 0.5f, 0);
    public float bumpForce = 1.0f;
    public float bumpTime = 1.0f;
    public bool hasSentBump = false;

    // Update is called once per frame
    void Update()
    {
        if (!hasSentBump && Time.time > bumpTime)
        {
            hasSentBump = true;
            target.AddForce(bumpDirection.normalized * bumpForce, ForceMode2D.Impulse);
        }
    }
}
