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
                throw new NullReferenceException($"Missing a {typeof(Game).Name} instance in the current scene. " +
                    $"Consider adding an instance of the {typeof(GameInstantiator)} prefab to the scene.");
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

    [SerializeField]
    private int briefingSceneIndex = 1;

    private int? currentLevelIndex = null;

    public static bool IsInstanceSet => instance != null;

    protected virtual void Awake()
    {
        // Set the Instance property
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Bootstrap if we are started directly in a level scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int levelIndexSearch = levelSceneIndices.IndexOf(currentSceneIndex);
        if (levelIndexSearch >= 0)
        {
            Debug.Log("Start level index detected: " + levelIndexSearch.ToString());
            currentLevelIndex = levelIndexSearch;
        }
    }

    public void GoToFirstLevel()
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

    public void GoToTitleScene()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    public void GoToBriefing()
    {
        SceneManager.LoadScene(briefingSceneIndex);
    }

    public void WinLevel(Level level)
    {
        if (currentLevelIndex.HasValue)
        {
            if (currentLevelIndex.Value < levelSceneIndices.Count - 1)
            {
                Debug.Log("Loading the next level scene");
                currentLevelIndex++;
                RestartLevel();
            }
            else
            {
                Debug.Log("All levels have been completed => Back to menu, until we have a credits scene.");
                GoToTitleScene();
            }
        }
        else
        {
            Debug.Log("No current level is defined; this may happen if one started a level scene directly. Going back to the title scene");
            GoToTitleScene();
        }
    }
}


