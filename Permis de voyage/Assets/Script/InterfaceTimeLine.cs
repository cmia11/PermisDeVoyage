using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//[RequireComponent(typeof(LocalTime))]
public class InterfaceTimeLine : MonoBehaviour
{
    public LocalTime localtime;
    private AudioSource audiosource;
    //public float value;
    public TextMeshProUGUI horloge;
    //GameObject gameObject = this;
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        // value = gameObject.GetComponent<Value>;
        audiosource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
