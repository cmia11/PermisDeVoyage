using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip mouseClick;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void StartGame()
    {
        audioSource.PlayOneShot(mouseClick, 1.0f);
        Game.Instance.GoToBriefing();
     }
}
