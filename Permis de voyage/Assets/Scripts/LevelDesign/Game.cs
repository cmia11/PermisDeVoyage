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

    public static bool IsInstanceSet => instance != null;

    [SerializeField]
    private int menuSceneBuildIndex = 0;

    [SerializeField]
    private int creditsBuildIndex = 1;

    [SerializeField]
    private int firstLevelBuildIndex = 2;

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
    }

    public void GoToFirstLevel()
    {
        if (firstLevelBuildIndex >= SceneManager.sceneCountInBuildSettings)
            throw new InvalidOperationException($"The first level scene index {firstLevelBuildIndex} does not match an existing scene " +
                $"(scene count = {SceneManager.sceneCountInBuildSettings}). Unable to start playing.");
        SceneManager.LoadScene(firstLevelBuildIndex);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitleScene()
    {
        SceneManager.LoadScene(menuSceneBuildIndex);
    }

    public void GoToCreditsScene()
    {
        SceneManager.LoadScene(creditsBuildIndex);
    }

    public void WinLevel()
    {
        int currentSceneIndexIndex = SceneManager.GetActiveScene().buildIndex;
        if (0 <= currentSceneIndexIndex && currentSceneIndexIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            Debug.Log("Loading the next scene");
        SceneManager.LoadScene(currentSceneIndexIndex +1);
        }
        else
        {
            Debug.Log("All levels have been completed => Go to the Credits scene.");
            GoToCreditsScene();
        }
    }

    public void LoseLevel()
    {
        ReloadCurrentScene();
    }
}


