using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerFinishLevel : VerboseComponent
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            Log("The player has reached the far side of the level.");
            Level.Instance.HasReachedFarSide = true;
        }
    }

}