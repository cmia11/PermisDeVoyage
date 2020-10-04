using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class MoveAction : TimeInversibleAction
{
    private class HistoryDataPoint
    {
        public float Time { get; }
        public Vector2 Position { get; }
        public float Angle { get; }

        public HistoryDataPoint(float time, Vector2 position, float angle)
        {
            Time = time;
            Position = position;
            Angle = angle;
        }

        public static HistoryDataPoint Lerp(HistoryDataPoint p1, HistoryDataPoint p2, float ratio)
        {
            if (ratio < 0 || ratio > 1)
                throw new ArgumentOutOfRangeException($"Interpolation ratio only has meaning in the [0, 1] range ({ratio} given).");
            return new HistoryDataPoint(
                (1 - ratio) * p1.Time + ratio * p2.Time,
                Vector3.Lerp(p1.Position, p2.Position, ratio),
                (1 - ratio) * p1.Angle + ratio * p2.Angle
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
            if (GetTargetRigidbody() == null)
            {
                // Can't apply to an object that does not have a rigidbody
                return false;
            }
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
            if (GetTargetRigidbody() == null)
            {
                // Can't apply to an object that does not have a rigidbody
                return false;
            }
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
    private RigidbodyType2D originalRBType;

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
        // Move actions are supposed to be created when the thing starts moving.
        // So it goes back to zero speed when we're done. This is also a way to prevent
        // new spurious MoveActions to be spawned because we left the owner moving just a little bit.
        ownerVelocity = Vector2.zero;
        ownerAngularVelocity = 0;
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
                // Exit condition = we have played back the whole history.
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
        Rigidbody2D targetRB = GetTargetRigidbody();
        originalRBType = targetRB.bodyType;
        targetRB.bodyType = RigidbodyType2D.Kinematic;
    }

    private void ConfigureNormalPhysics()
    {
        Rigidbody2D targetRB = GetTargetRigidbody();
        targetRB.bodyType = originalRBType;
        if (!targetRB.isKinematic)
        {
            // Apply speeds to the thing as it goes back to the realm of normal physics
            targetRB.velocity = ownerVelocity;
            targetRB.angularVelocity = ownerAngularVelocity;
        }
    }

    private Rigidbody2D GetTargetRigidbody()
    {
        return Owner.GetComponent<Rigidbody2D>();
    }

    private void RecordHistory()
    {
        Transform ownerTransform = Owner.transform;
        var historyPoint = new HistoryDataPoint(
            LocalTime.Value,
            ownerTransform.position,
            ownerTransform.rotation.eulerAngles.z
        );
        if (history.Count < 2)
        {
            // We added a first point when the recording started. Add a second one.
            history.Add(historyPoint);
        }
        else
        {
            HistoryDataPoint point_n_1 = history[history.Count - 1];
            HistoryDataPoint point_n_2 = history[history.Count - 1];
            if (Mathf.Abs(point_n_1.Time - point_n_2.Time) >= TimeResolution)
            {
                // There is enough gap between the last two measures ; add one more.
                history.Add(historyPoint);
            }
            else
            {
                // The last two points were very close; replace the most recent one.
                history[history.Count - 1] = historyPoint;
            }
        }
    }

    private void DoPlayback()
    {
        int nPoints = history.Count;
        int ivMinIndex = 0;
        int ivMaxIndex = nPoints -2; // -1 because indices; -1 because there is one less intervals than points.

        if (ivMaxIndex >= 0)
        {
            float timeNow = LocalTime.Value;
            int? properIntervalIndex = null;
            while (true)
            {
                int pivotIndex = (ivMinIndex + ivMaxIndex) / 2;
                HistoryDataPoint pivotBound1 = history[ivMinIndex];
                HistoryDataPoint pivotBound2 = history[ivMinIndex + 1];

                if (IsCurrentInterval(pivotBound1, pivotBound2))
                {
                    // Check the pivot interval. If it matches, exit the loop
                    properIntervalIndex = pivotIndex;
                    break;
                }
                else if (ivMinIndex != ivMaxIndex)
                {
                    // There are more intervals to explore, let's split the search space either on the left or right
                    bool checkLeft = (TimeDirectionSign * (timeNow - pivotBound1.Time) > 0);
                    if (checkLeft && ivMinIndex == pivotIndex || !checkLeft && ivMaxIndex == pivotIndex)
                    {
                        Debug.LogError($"Failed to locate the interval for local time {timeNow} in the history.");
                        break;
                    }
                    else
                    {
                        if (checkLeft)
                            ivMaxIndex = pivotIndex - 1;
                        else
                            ivMinIndex = pivotIndex + 1;
                    }
                }
                else
                {
                    Debug.LogError("Unable to locate the interval where a value should be taken in the history.");
                    break;
                }
            }

            if (properIntervalIndex.HasValue)
            {
                // Get the position and rotation to apply
                HistoryDataPoint p1 = history[properIntervalIndex.Value];
                HistoryDataPoint p2 = history[properIntervalIndex.Value + 1];
                float intervalRatio = GetUnboundedRatioInInterval(p1, p2);
                HistoryDataPoint interpolation = HistoryDataPoint.Lerp(p1, p2, intervalRatio);

                // Apply these to the object
                Transform ownerTransform = Owner.transform;
                ownerTransform.position = interpolation.Position;
                ownerTransform.rotation = Quaternion.Euler(0,0,interpolation.Angle);
            }
        }
        else
        {
            Debug.LogError(
                "Unable to playback an (almost) empty history. This action should have maybe be completed by now.\n" +
                $"  * History times = [{history.Aggregate("", (s, p) => (string.IsNullOrEmpty(s) ? s : s + ", ") + p.Time.ToString())}], " +
                $"time now = {LocalTime}.");
        }
    }

    private bool IsCurrentInterval(HistoryDataPoint p1, HistoryDataPoint p2)
    {
        float timeNow = LocalTime.Value;
        return (p1.Time - timeNow) * (p2.Time - timeNow) <= 0;
    }

    /// <summary>
    /// Ratio of the current time in the specified interval. May go beyond [0, 1]
    /// </summary>
    private float GetUnboundedRatioInInterval(HistoryDataPoint p1, HistoryDataPoint p2)
    {
        float p1Time = p1.Time;
        float p2Time = p2.Time;
        if (p1Time != p2Time) {
            return Mathf.Sign(p2Time - p2Time) * (LocalTime.Value - p1Time) / (p2Time - p1Time);
        }
        else if (LocalTime.Value == p1Time) {
            // Anything between 0 and 1 would do.
            return 0.5f;
        }
        else
        {
            // What is the ratio if we're outside of a zero-width interval !?
            throw new ArgumentException("Invalid ratio computation using a zero-width interval");
        }
    }

}
