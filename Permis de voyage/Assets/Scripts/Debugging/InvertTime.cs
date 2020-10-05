using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inverts the default time when pressing on a button
/// </summary>
public class InvertTime : MonoBehaviour
{
    public string buttonName = null;
    public KeyCode keyName = KeyCode.T;

    void Update()
    {
        if (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName) || Input.GetKeyDown(keyName))
        {
            Level.Instance.DefaultTime.RelativeSpeed *= -1;
            Debug.Log("Default timeline speed set to " + Level.Instance.DefaultTime.RelativeSpeed.ToString());
        }
    }
}
