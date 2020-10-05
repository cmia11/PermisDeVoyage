using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LocalTime))]
[RequireComponent(typeof(PlayerLevels))]

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Default timeline for most of the objects in the game
    /// </summary>
    public LocalTime DefaultTime { get; private set; }
    public PlayerLevels Levels { get; private set; }

    /// <summary>
    /// Unique instance of the game manager
    /// </summary>
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        DefaultTime = GetComponent<LocalTime>();
        Levels = GetComponent<PlayerLevels>();
    }


}
