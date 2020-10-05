using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerFinishLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player" && GameManager.Instance.DefaultTime.RelativeSpeed > 0)
        {

            Debug.Log("Start Phase 2 YOU HAVE CHANGED TIME");
            GameManager.Instance.DefaultTime.RelativeSpeed = -1.0f;

        }

    }

}