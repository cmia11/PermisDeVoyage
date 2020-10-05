using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TimelinedObject))]
public class MovesRecorder : MonoBehaviour
{
    // No action will be started before this settle time has elapsed, in order to let some time to the physics to get everything in place. 
    public float settleTime = 0.5f;
    public bool singleActivation = false;

    private float enableTime = 0;

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

    protected virtual void OnEnable()
    {
        enableTime = Time.time;
    }

    protected virtual void Update()
    {
        // Every time the object is moving, an action should record it. Else create one.
        if (Target.IsMoving && Time.time >= enableTime + settleTime)
        {
            if (!target.Actions.Any(a => a.IsActiveAt(Target.LocalTime.Value).Equals(true)))
            {
                MoveAction newAction = Target.CreateAction<MoveAction>();
                newAction.StartFreeMove();

                if (singleActivation)
                    enabled = false;
            }
        }
    }

}
