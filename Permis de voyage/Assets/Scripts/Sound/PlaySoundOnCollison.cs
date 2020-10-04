using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnCollison : MonoBehaviour
{
    public AudioClip audioToPlay;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        GetComponent<AudioSource>().clip = audioToPlay;
    }

    // Play the sound when Player enters the zone
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
             GetComponent<AudioSource>().Play();

             // Desactivate the collider of the zone when player exit so we don't play the sound twice
             GetComponent<Collider2D>().enabled = false;
        }
    }
}