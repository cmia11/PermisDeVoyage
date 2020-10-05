using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageFirstBriefing : MonoBehaviour
{
    private AudioSource audiosource;
    public AudioClip backgroundVocal;

    protected virtual void Start()
    {
        audiosource = GetComponent<AudioSource>();
        audiosource.PlayOneShot(backgroundVocal, 4.0f);
    }

    public void ReStartBrief()
    {
        Game.Instance.ReloadCurrentScene();
    }

    public void StartLevel()
    {
        Game.Instance.GoToFirstLevel();
    }
}
