using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLevels : MonoBehaviour
{
    public bool isPlayerDead;
    // Start is called before the first frame update
    void Start()
    {

        isPlayerDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerDead)
        {
            GameOver();
        }
    }

    public void GameOver()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    
    
    }


