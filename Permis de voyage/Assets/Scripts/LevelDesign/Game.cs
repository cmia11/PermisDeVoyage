using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    /// <summary>
    /// Unique instance of the game manager
    /// </summary>
    public static Game Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
                throw new NullReferenceException($"Missing a {typeof(Level).Name} instance in the current scene.");
        }
        private set
        {
            instance = value != null ? value : throw new ArgumentNullException();
        }
    }
    private static Game instance;

    public IList<int> LevelSceneIndices => levelSceneIndices.ToArray();
    [SerializeField]
    private List<int> levelSceneIndices = new List<int>();

    [SerializeField]
    private int menuSceneIndex = 0;

    private int? currentLevelIndex = null;

    public static bool IsInstanceSet => instance != null;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public void StartPlaying()
    {
        if (levelSceneIndices.Count < 1)
            throw new InvalidOperationException("The list of level scenes indices (from the Build settings) is not set. Unable to start playing.");
        currentLevelIndex = 0;
        RestartLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(levelSceneIndices[currentLevelIndex.Value]);
    }

    public void WinLevel(Level level)
    {
        if (!currentLevelIndex.HasValue)
            throw new InvalidOperationException("There is no current level to win.");
        if (currentLevelIndex.Value < levelSceneIndices.Count -1)
        {
            // Go to the next level
            currentLevelIndex++;
            RestartLevel();
        }
        else
        {
            // All levels have been completed => Back to menu, until we have a credits scene.
            SceneManager.LoadScene(menuSceneIndex);
        }
    }
}


