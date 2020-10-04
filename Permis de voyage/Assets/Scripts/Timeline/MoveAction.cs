using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveAction : TimeInversibleAction
{
    private class HistoryDataPoint
    {
        public float Time { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public HistoryDataPoint(float time, Vector3 position, Quaternion rotation)
        {
            Time = time;
            Position = position;
            Rotation = rotation;
        }

        public static HistoryDataPoint Lerp(HistoryDataPoint p1, HistoryDataPoint p2, float ratio)
        {
            if (ratio < 0 || ratio > 1)
                throw new ArgumentOutOfRangeException($"Interpolation ratio only has meaning in the [0, 1] range ({ratio} given).");
            return new HistoryDataPoint(
                (1 - ratio) * p1.Time + ratio * p2.Time,
                Vector3.Lerp(p1.Position, p2.Position, ratio),
                Quaternion.Lerp(p1.Rotation, p2.Rotation, ratio)
            );
        }
    }

    /// <summary>
    /// Maximum interval between two saved data points.
    /// </summary>
    public float TimeResolution
    {
        get
        {
            return timeResolution;
        }
        set
        {
            if (timeResolution != value)
            {
                if (history.Count > 2)
                    // Trop compliqué pour un code de jam
                    throw new NotImplementedException("Resampling the move data is not implemented.");
                timeResolution = value;
            }
        }
    }
    private float timeResolution = 0.2f;
    public override bool IsIntantaneous => false;

    public override bool CanStartForward
    {
        get
        {
            if (Owner.Actions.Any(a => a.CurrentState == State.RunningBackwards))
            {
                // Forward moves cannot interrupt recorded backward moves by design.
                return false;
            }
            else if (Owner.Actions.Any(a => a.CurrentState == State.RunningForward))
            {
                // We cannot start either if the owner object is already recording positions.
                return false;
            }
            return true;
        }
    }

    public override bool CanStartBackwards
    {
        get
        {
            if (Owner.Actions.Any(a => a.CurrentState == State.RunningBackwards))
            {
                // We cannot start if the owner object is already moving backwards since we would compete with another action.
                return false;
            }
            else if (Owner.Actions.Any(a => a.CurrentState == State.RunningForward))
            {
                // We cannot start if the owner object is currently moving forward either, this cancels us.
                return false;
            }
            return true;
        }
    }

    // Are we controling the target object 
    private bool IsPlayingBack
    {
        get => isPlayingBack;
        set
        {
            if (isPlayingBack != value)
            {
                isPlayingBack = value;
                if (isPlayingBack)
                    ConfigurePlaybackPhysics();
                else
                    ConfigureNormalPhysics();
            }
        }
    }
    private bool isPlayingBack = false;

    /// <summary>
    /// List of data point collected during the forward execution of the action.
    /// They are added to this list in the they are first collected.
    /// Therefore it's possible that their "Time" property goes decreasing if the action
    /// first started while the local time was going back.
    /// </summary>
    private List<HistoryDataPoint> history = new List<HistoryDataPoint>();

    // Movement parameters in the Unity reference time
    private Vector3 lastOwnerPosition;
    private Vector3 ownerVelocity;
    private float lastOwnerAngle;
    private float ownerAngularVelocity;

    /// <summary>
    /// This starts recording the moves of the action owner, driven by the Unity physics.
    /// This action will stop once the owner object stops moving in world space.
    /// </summary>
    public void StartFreeMove()
    {
        if (CanStartForward)
            StartForward();
        else
        {
            Debug.LogError("This operation cannot start, Its owner will be notified of the failure.");
            Owner.SignalActionStartFailed(this);
        }
    }

    protected override void OnStartedForward()
    {
        Transform ownerTransform = Owner.transform;
        lastOwnerPosition = ownerTransform.position;
        lastOwnerAngle = ownerTransform.localRotation.eulerAngles.z;
    }

    protected override void OnStartedBackwards()
    {
        OnStartedForward();
        IsPlayingBack = true;
    }

    protected override void OnCompletedBackwards()
    {
        IsPlayingBack = false;
    }

    protected override bool OnMakingProgress()
    {
        bool isDone = false;

        // Compute the owner new speed and angular speed
        Transform ownerTransform = Owner.transform;
        if (LocalTime.DeltaTime > 0)
        {
            ownerVelocity = (ownerTransform.position - lastOwnerPosition) / LocalTime.DeltaTime;
            ownerAngularVelocity = (ownerTransform.localRotation.eulerAngles.z - lastOwnerAngle) / LocalTime.DeltaTime;
        }
        else
        {
            ownerVelocity = Vector3.zero;
            ownerAngularVelocity = 0;
        }

        // Compute whever we're recording points or playing back the history.
        float actionProgressSign = (TimeDirectionSign.Value * LocalTime.DeltaTime);
        if (actionProgressSign > 0)
        {
            IsPlayingBack = false;
            if (ownerVelocity != Vector3.zero || ownerAngularVelocity != 0)
            {
                RecordHistory();
            }
            else
            {
                // Exit condition = we have stopped
                isDone = true;
            }
        }
        else if (actionProgressSign < 0)
        {
            IsPlayingBack = true;
            if ((ForwardStartTime.Value - LocalTime.Value) * TimeDirectionSign < 0)
            {
                DoPlayback();
            }
            else
            {
                // Exit condition = we have played back the whole history
                isDone = true;
            }
        }
        else
        {
            // The local time is frozen: nothing happens this frame.
        }

        // Update for next frame
        lastOwnerPosition = ownerTransform.position;
        lastOwnerAngle = ownerTransform.localRotation.eulerAngles.z;
        return isDone;
    }

    private void ConfigurePlaybackPhysics()
    {
        throw new NotImplementedException();
    }

    private void ConfigureNormalPhysics()
    {
        throw new NotImplementedException();
    }

    private void RecordHistory()
    {
        throw new NotImplementedException();
    }

    private void DoPlayback()
    {
        throw new NotImplementedException();
    }

}
