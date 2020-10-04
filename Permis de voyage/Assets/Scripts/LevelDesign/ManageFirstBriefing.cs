using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageFirstBriefing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReStartBrief()
    {
        SceneManager.LoadScene(1);


    }

    public void StartLevel()
    {
        SceneManager.LoadScene(2);
    }
}
