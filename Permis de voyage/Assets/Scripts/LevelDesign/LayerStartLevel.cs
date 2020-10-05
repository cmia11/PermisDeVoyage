using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LayerStartLevel : MonoBehaviour
{
    public int level;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.DefaultTime.Value < 0)
        {
            GameManager.Instance.DefaultTime.RelativeSpeed = 1.0f;
            SceneManager.LoadScene(level);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {


        if (other.name == "Player" && GameManager.Instance.DefaultTime.RelativeSpeed < 0 && GameManager.Instance.DefaultTime.Value > 0)
        {

            Debug.Log("YOU HAVE WON" + GameManager.Instance.DefaultTime.RelativeSpeed);
            GameManager.Instance.DefaultTime.RelativeSpeed = 1.0f;
            SceneManager.LoadScene(level+1);
        }





    }
}
