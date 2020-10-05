using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(LocalTime))]

public class Level : MonoBehaviour
{
    /// <summary>
    /// Default timeline for most of the objects in the game
    /// </summary>
    public LocalTime DefaultTime { get; private set; }

    /// <summary>
    /// Has the player reached the other end of the level?
    /// </summary>
    public bool HasReachedFarSide {
        get => hasReachedOtherEnd;
        set
        {
            hasReachedOtherEnd = value;
            DefaultTime.RelativeSpeed = hasReachedOtherEnd ? -1.0f : 1.0f;
        }
    }
    public bool hasReachedOtherEnd;

    /// <summary>
    /// Unique instance of the game manager
    /// </summary>
    public static Level Instance {
        get
        {
            if (instance == null)
            {
                Debug.LogWarning($"Auto-instancing a {typeof(Level).Name} since none is registered when accessing the {nameof(Instance)} property.");
                GameObject emergencyObject = new GameObject("Auto-instantiated " + typeof(Level).Name);
                // Will set itself to the instance field on Awake().
                emergencyObject.AddComponent<Level>();
            }
            return instance;
        }
        private set
        {
            instance = value != null ? value : throw new ArgumentNullException();
        }
    }
    private static Level instance;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DefaultTime = GetComponent<LocalTime>();
        DefaultTime.RelativeSpeed = 1.0f;
    }


}
