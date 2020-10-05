using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageFirstBriefing : MonoBehaviour
{
    private AudioSource audiosource;
    public AudioClip backgroundVocal;
    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        audiosource.PlayOneShot(backgroundVocal, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReStartBrief()
    {
        SceneManager.LoadScene(1);


    }

    public void StartLevel()
    {
        SceneManager.LoadScene(2);
    }
}
