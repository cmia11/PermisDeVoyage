using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;

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
        get => hasReachedFarSide;
        set
        {
            hasReachedFarSide = value;
            DefaultTime.RelativeSpeed = hasReachedFarSide ? -1.0f : 1.0f;
            if (shadowPostprocess != null)
                shadowPostprocess.enabled = hasReachedFarSide;
        }
    }
    public bool hasReachedFarSide;

    /// <summary>
    /// Unique instance of the game manager
    /// </summary>
    public static Level Instance {
        get
        {
            if (instance != null)
                return instance;
            else
                throw new NullReferenceException($"Missing a {typeof(Level).Name} instance in the current scene. " +
                    $"Please add an instance of the level prefab to this scene.");
        }
        private set
        {
            instance = value != null ? value : throw new ArgumentNullException();
        }
    }
    private static Level instance;

    public PostProcessLayer shadowPostprocess;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DefaultTime = GetComponent<LocalTime>();
        DefaultTime.RelativeSpeed = 1.0f;
    }

    protected virtual void Update()
    {
        if (HasReachedFarSide && DefaultTime.Value <= 0)
        {
            Game.Instance.LoseLevel(this);
        }
    }

    /// <summary>
    /// Callback for the "restart level" button
    /// </summary>
    public void OnReplayLevelClick()
    {
        Game.Instance.ReloadCurrentScene();
    }
}
