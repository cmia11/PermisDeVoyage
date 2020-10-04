using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TimelinedObject))]
public class MovesRecorder : MonoBehaviour
{
    /// <summary>
    /// The local time which this action uses to update its state.
    /// </summary>
    public TimelinedObject Target
    {
        get
        {
            target = target != null ? target : GetComponent<TimelinedObject>();
            return target;
        }
    }
    private TimelinedObject target;

    protected virtual void Update()
    {
        // Every time the object is moving, an action should record it. Else create one.
        if (Target.IsMoving)
        {
            if (!target.Actions.Any(a => a.IsActiveAt(Target.LocalTime.Value).Equals(true)))
            {
                MoveAction newAction = Target.CreateAction<MoveAction>();
                newAction.StartFreeMove();
            }
        }
    }

}
