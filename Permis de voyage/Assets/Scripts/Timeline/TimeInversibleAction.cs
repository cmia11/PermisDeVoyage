using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TimeInversibleAction : MonoBehaviour
{
    /// <summary>
    /// The local time which this action uses to update its state.
    /// </summary>
    public LocalTime LocalTime
    {
        get
        {
            return timeline ?? throw new NullReferenceException(
                $"The {nameof(LocalTime)} property of this object should have been set before using it.");
        }
        set
        {
            timeline = value ?? throw new ArgumentNullException();
        }
    }
    [SerializeField]
    private LocalTime timeline;

    /// <summary>
    /// The time when the action entered the running state (undefined outside this state).
    /// </summary>
    public float? EntryTime { get; private set; }

    /// <summary>
    /// The local time at which this action started.
    /// It is defined iff StartForward() or CompleteBackwards() have been called at least once.
    /// </summary>
    public float? StartTime { get; private set; }

    /// <summary>
    /// The local time at which this action started.
    /// It is defined iff CompleteForward() or StartBackwards() have been called at least once.
    /// </summary>
    public float? EndTime { get; private set; }

    /// <summary>
    /// Is this action instantaneous?
    /// </summary>
    public abstract bool IsIntantaneous { get; }

    /// <summary>
    /// Position 
    /// </summary>
    public enum State
    {
        Unknown,
        Before,
        Running,
        After,
    }

    /// <summary>
    /// Etat actuel de cette action.
    /// </summary>
    public State CurrentState { get; private set; }

    protected virtual void Awake()
    {
        // In case we add an action on the same GameObject as the TimelinedObject it belongs to, it will
        // share its local time automatically.
        timeline = timeline ?? GetComponent<LocalTime>();
    }

    protected virtual void Update()
    {
        switch (CurrentState)
        {
            case State.Unknown:
                break;
            case State.Before:
                UpdateInBefore();
                break;
            case State.Running:
                UpdateInRunning();
                break;
            case State.After:
                UpdateInAfter();
                break;
            default:
                throw new NotImplementedException($"Unsupported state {CurrentState}");
        }
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
    protected virtual void OnCompletedBackwards() { }

    #endregion

    protected virtual void UpdateInBefore()
    {
        // If we know our start time and it's behind us then it's time to start.
        // This is the only transition we can take.
        if (LocalTime.Value >= StartTime)
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
    }

    protected virtual void UpdateInRunning()
    {
        // The action implementation will tell us if it has completed in a direction or another
        bool isDone = OnMakingProgress();
        if (isDone)
            if (LocalTime.DeltaTime > 0)
                CompleteForward();
            else if (LocalTime.DeltaTime < 0)
                CompleteBackwards();
            else
                throw new InvalidOperationException(
                    "Since the local time did not make progress this frame, it is ambiguous to try completing the action " +
                    "because it's not clear whever it completed in the forward or backwards time direction.");
    }

    protected virtual void UpdateInAfter()
    {
        // If we know our start time and it's behind us then it's time to start.
        // This is the only transition we can take.
        if (LocalTime.Value <= EndTime)
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
    }

    protected void StartForward()
    {
        AssertValidTransition(
            CurrentState == State.Unknown || CurrentState == State.Before,
            "This operation can only be performed if the action never started, or its local time is before its last start time.");
        StartTime = LocalTime.Value;
        EntryTime = LocalTime.Value;
        CurrentState = State.Running;
        OnStartedForward();
    }

    protected void CompleteForward()
    {
        AssertValidTransition(CurrentState == State.Running, "This operation can only be performed if the action is running.");
        EndTime = LocalTime.Value;
        CurrentState = State.After;
        OnCompletedForward();
    }

    protected void StartBackwards()
    {
        AssertValidTransition(
            CurrentState == State.Unknown || CurrentState == State.After,
            "This operation can only be performed if the action never started, or its local time is after its last end time.");
        EndTime = LocalTime.Value;
        EntryTime = LocalTime.Value;
        CurrentState = State.Running;
        OnStartedBackwards();
    }

    protected void CompleteBackwards()
    {
        AssertValidTransition(CurrentState == State.Running, "This operation can only be performed if the action is running.");
        StartTime = LocalTime.Value;
        CurrentState = State.Before;
        OnCompletedBackwards();
    }

    private void AssertValidTransition(bool isValid, string errorMessage)
    {
        if (!isValid)
        {
            throw new InvalidOperationException(
                errorMessage + $"\nCurrent state = {CurrentState}, new local time = {LocalTime.Value}, last start time = {StartTime}, last complete time = {EndTime}.");
        }
    }
}
