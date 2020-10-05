using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LocalTime))]
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Default timeline for most of the objects in the game
    /// </summary>
    public LocalTime DefaultTime { get; private set; }

    /// <summary>
    /// Unique instance of the game manager
    /// </summary>
    public static GameManager Instance {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning($"Auto-instancing a {typeof(GameManager).Name} since none is registered when accessing the {nameof(Instance)} property.");
                GameObject emergencyObject = new GameObject("Auto-instantiated " + typeof(GameManager).Name);
                // Will set itself to the instance field on Awake().
                emergencyObject.AddComponent<GameManager>();
            }
            return instance;
        }
        private set
        {
            instance = value != null ? value : throw new ArgumentNullException();
        }
    }
    private static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DefaultTime = GetComponent<LocalTime>();
    }


}
