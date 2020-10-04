using UnityEngine;
using System.Collections;

public class MoveAction : TimeInversibleAction
{
    public override bool IsIntantaneous => false;

    /// <summary>
    /// This starts recording the moves of the action owner, driven by the Unity physics.
    /// This action will stop once the owner object stops moving in world space.
    /// </summary>
    public void StartFreeMove()
    {
        
    }
}
