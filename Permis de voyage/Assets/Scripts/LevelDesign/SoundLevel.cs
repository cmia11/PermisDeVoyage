using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLevel : MonoBehaviour
{
    private AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        audiosource.pitch = Level.Instance.DefaultTime.RelativeSpeed;
        audiosource.loop = true;
        //Debug.Log(GameManager.Instance.DefaultTime.RelativeSpeed);
        audiosource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}