using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bump : MonoBehaviour
{
    public Rigidbody2D target;
    public float bumpForce = 2.0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 bumpDirection = Random.insideUnitCircle;
            bumpDirection.y = Mathf.Abs(bumpDirection.y);
            target.AddForce(bumpDirection * bumpForce, ForceMode2D.Impulse);
        }
    }
}
