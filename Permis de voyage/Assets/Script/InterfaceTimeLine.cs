using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
[RequireComponent(typeof(LocalTime))]
public class InterfaceTimeLine : MonoBehaviour
{
    private float localTime;
    private AudioSource audiosource;
    public bool isGameOver = false;
   // public GameObject Text;
    //public float value;
    public TextMeshPro horloge;

    void Start()
    {
        horloge = GetComponent<TextMeshPro>();
        localTime = GetComponent<LocalTime>().Value;
        audiosource = GetComponent<AudioSource>();
        // value = gameObject.GetComponent<Value>;
        audiosource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        horloge.text = "bonjour";
        Debug.Log(localTime);
    }
}
