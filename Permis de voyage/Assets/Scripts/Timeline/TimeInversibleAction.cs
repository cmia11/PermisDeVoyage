using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TimeInversibleAction : VerboseComponent
{
    /// <summary>
    /// Identifier of this task
    /// </summary>
    public int ID { get; private set; }
    private static int nextID = 0;

    /// <summary>
    /// The local time which this action uses to update its state.
    /// </summary>
    public LocalTime LocalTime
    {
        get
        {
            return localTime != null ? localTime : throw new NullReferenceException(
                $"The {nameof(LocalTime)} property of this object should have been set before using it.");
        }
        set
        {
            localTime = value != null ? value : throw new ArgumentNullException();
        }
    }
    [SerializeField]
    private LocalTime localTime;

    /// <summary>
    /// The owner of this action.
    /// </summary>
    public TimelinedObject Owner
    {
        get
        {
            return owner != null ? owner : throw new NullReferenceException(
                $"The {nameof(Owner)} property of this object should have been set before using it.");
        }
        set
        {
            owner = value != null ? value : throw new ArgumentNullException();
        }
    }
    [SerializeField]
    private TimelinedObject owner;

    /// <summary>
    /// Indicates whever this action may start in its "normal" direction.
    /// </summary>
    public virtual bool CanStartForward => true;

    /// <summary>
    /// Indicates whever this action may start in its "reversed" direction.
    /// </summary>
    public virtual bool CanStartBackwards => true;

    /// <summary>
    /// The local time at which this action started forward (or completed backwards).
    /// It is defined iff StartForward() has been called at least once.
    /// It may be lower than EndTime if the action started when the local time speed was negative.
    /// </summary>
    public float? ForwardStartTime { get; private set; }

    /// <summary>
    /// The local time at which this action completed forward (or started backwards).
    /// It is defined iff CompleteForward() has been called at least once.
    /// </summary>
    public float? ForwardCompleteTime { get; private set; }

    /// <summary>
    /// When in running state, returns the signed local time duration since the action started.
    /// </summary>
    public float? SignedTimeFromStart
    {
        get
        {
            if (CurrentState == State.RunningForward)
            {
                return TimeDirectionSign.Value * (LocalTime.Value - ForwardStartTime.Value);
            }
            else if (CurrentState == State.RunningBackwards)
            {
                return TimeDirectionSign * (ForwardCompleteTime.Value - LocalTime.Value);
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Is this action instantaneous?
    /// </summary>
    public abstract bool IsIntantaneous { get; }

    /// <summary>
    /// Returns 1 if the forward direction of the task is the same as the reference timeline (Time.time),
    /// and -1 if it is the opposite direction. This is undefined until the action has started.
    /// </summary>
    public int? TimeDirectionSign { get; private set; }

    /// <summary>
    /// Position 
    /// </summary>
    public enum State
    {
        Unknown,
        Before,
        RunningForward,
        RunningBackwards,
        After,
    }

    /// <summary>
    /// Etat actuel de cette action.
    /// </summary>
    public State CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState != value)
                Log($"Changing state: {currentState} -> {value}");
            else
                Log($"Remaining in state {currentState}");
            currentState = value;
        }
    }
    private State currentState;

    protected virtual void Awake()
    {
        ID = nextID++;
        // In case we add an action on the same GameObject as the TimelinedObject it belongs to, it will
        // share its local time automatically.
        localTime = localTime  != null ? localTime : GetComponent<LocalTime>();
    }

    protected override void Update()
    {
        base.Update();
        switch (CurrentState)
        {
            case State.Unknown:
                break;
            case State.Before:
                UpdateInBefore();
                break;
            case State.RunningForward:
            case State.RunningBackwards:
                UpdateInRunning();
                break;
            case State.After:
                UpdateInAfter();
                break;
            default:
                throw new NotImplementedException($"Unsupported state {CurrentState}");
        }
    }

    /// <summary>
    /// Indicates whever this action is active at the specified point in its local time or
    /// was active when it crossed this point in time last time.
    /// </summary>
    public bool? IsActiveAt(float localTime)
    {
        bool? res = null;
        switch (CurrentState)
        {
            case State.RunningForward:
                // Active if the looked up time if between the current time and the start time.
                res = (localTime - ForwardStartTime.Value) * (localTime - LocalTime.Value) <= 0;
                break;
            case State.RunningBackwards:
                res = (localTime - ForwardCompleteTime.Value) * (localTime - LocalTime.Value) <= 0;
                break;
            default:
                if (ForwardCompleteTime.HasValue && ForwardCompleteTime.HasValue)
                    res = ForwardStartTime.Value <= localTime && localTime <= ForwardCompleteTime.Value;
                break;
        }
        return res;
    }

    #region "Methods to override in children"

    /// <summary>
    ///  This is called when the action is running, either forward or backwards.
    ///  This method may Use the LocalTime property to determine what it should do.
    ///  It MUST return true if and only if it has completed in either time direction and.
    ///  the local time is not stopped.
    ///  This method MAY NOT call state changing methods such as CompleteForward.
    /// </summary>
    protected virtual bool OnMakingProgress() { return false; }

    /// <summary>
    /// This method is called when action started forward. This may happen if
    /// StartForward() was called directly. It may also happen if the action was running
    /// backwards, ompleted backwards, and then time went forward again and reached the previous completion point.
    /// </summary>
    protected virtual void OnStartedForward() { }

    /// <summary>
    /// See OnStartedForward.
    /// </summary>
    protected virtual void OnCompletedForward() { }

    /// <summary>
    /// See OnStartedForward.
    /// </summary>
    protected virtual void OnStartedBackwards() { }

    /// <summary>
    /// See OnStartedForward.
    /// </summary>
    protected virtual void OnCompletedBackwards() {
        Owner.SignalActionCompletedBackwards(this);
    }

    #endregion

    protected virtual void UpdateInBefore()
    {
        if (TimeDirectionSign.Value * (LocalTime.Value - ForwardStartTime.Value) >= 0)
        {
            if (CanStartForward)
            {
                StartForward();
                if (IsIntantaneous)
                {
                    CompleteForward();
                }
                else
                {
                    bool isDone = OnMakingProgress();
                    if (isDone)
                        CompleteForward();
                }
            }
            else
                Owner.SignalActionStartFailed(this);
        }
    }

    protected virtual void UpdateInRunning()
    {
        // The action implementation will tell us if it has completed in a direction or another
        bool isDone = OnMakingProgress();
        if (isDone)
            if (CurrentState == State.RunningForward)
                CompleteForward();
            else if (CurrentState == State.RunningBackwards)
                CompleteBackwards();
            else
                throw new InvalidOperationException("Invalid state, whould be running forward or backwards.");
    }

    protected virtual void UpdateInAfter()
    {
        if (TimeDirectionSign.Value * (ForwardCompleteTime.Value - LocalTime.Value) >= 0)
        {
            if (CanStartBackwards)
            {
                StartBackwards();
                if (IsIntantaneous)
                {
                    CompleteBackwards();
                }
                else
                {
                    bool isDone = OnMakingProgress();
                    if (isDone)
                        CompleteBackwards();
                }
            }
            else
                Owner.SignalActionStartFailed(this);
        }
    }

    protected void StartForward()
    {
        AssertValidTransition(CanStartForward, "This action cannot start at the moment.");
        AssertValidTransition(
            CurrentState == State.Unknown || CurrentState == State.Before,
            "This operation can only be performed if the action never started, or its local time is before its last start time.");
        ForwardStartTime = LocalTime.Value;
        TimeDirectionSign = (LocalTime.DeltaTime >= 0) ? 1 : -1;
        CurrentState = State.RunningForward;
        OnStartedForward();
    }

    protected void CompleteForward()
    {
        AssertValidTransition(CurrentState == State.RunningForward, "This operation can only be performed if the action is running forward.");
        ForwardCompleteTime = LocalTime.Value;
        CurrentState = State.After;
        OnCompletedForward();
    }

    protected void StartBackwards()
    {
        AssertValidTransition(CanStartBackwards, "This action cannot start in reverse mode at the moment.");
        AssertValidTransition(
            CurrentState == State.Unknown || CurrentState == State.After,
            "This operation can only be performed if the action never started, or its local time is after its last end time.");
        ForwardCompleteTime = LocalTime.Value;
        CurrentState = State.RunningBackwards;
        OnStartedBackwards();
    }

    protected void CompleteBackwards()
    {
        AssertValidTransition(CurrentState == State.RunningBackwards, "This operation can only be performed if the action is running backwards.");
        ForwardStartTime = LocalTime.Value;
        CurrentState = State.Before;
        OnCompletedBackwards();
    }

    private void AssertValidTransition(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new InvalidOperationException(
                errorMessage +
                $"\n  * Current state = {CurrentState}\n  * current local time = {LocalTime.Value}\n" +
                $"  * forward start time = {ForwardCompleteTime}\n  * forward complete time = {ForwardCompleteTime}\n" +
                $"  * CanStartForward = {CanStartForward}\n  * CanStartBackwards = {CanStartBackwards}");
        }
    }

    public override string ToString()
    {
        return $"[Action type: {GetType().Name}, ID: {ID}]";
    }
}
