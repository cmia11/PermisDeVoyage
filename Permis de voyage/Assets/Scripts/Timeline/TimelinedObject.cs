using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class TimelinedObject : MonoBehaviour
{
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
    /// The set of actions which this object has taken.
    /// </summary>
    public IList<TimeInversibleAction> Actions => actionsStorage.Select(t => t.Item1).ToArray();

    /// <summary>
    /// Tests whever this object is moving.
    /// </summary>
    public bool IsMoving => (
        velocity.sqrMagnitude > moveSpeedSqrThreshold ||
        Mathf.Abs(angularVelocity) > moveAngleSpeedThreshold
    );

    private GameObject actionsRoot;
    private static readonly string actionsRootName = "Actions";

    // Movement parameters in the Unity reference time
    private Vector3? lastPosition;
    private Vector3 velocity;
    private float? lastAngle;
    private float angularVelocity;

    float moveSpeedSqrThreshold = 0.01f;
    float moveAngleSpeedThreshold = 60;

/// <summary>
/// Each registered action is given a private GameObject where it can store its stuff, and make it easier to clean.
/// This field remembers which goes where.
/// </summary>
private List<Tuple<TimeInversibleAction, GameObject>> actionsStorage = new List<Tuple<TimeInversibleAction, GameObject>>();

    protected virtual void Awake()
    {
        // In case we add an action on the same GameObject as the TimelinedObject it belongs to, it will
        // share its local time automatically.
        localTime = localTime != null ? localTime : GetComponent<LocalTime>();
        actionsRoot = transform.Find(actionsRootName) != null ? transform.Find(actionsRootName).gameObject : null;
        if (actionsRoot == null) {
            actionsRoot = new GameObject(actionsRootName);
            actionsRoot.transform.SetParent(transform);
        }
    }

    protected virtual void Start()
    {
        if (localTime == null)
        {
            localTime = GameManager.Instance.DefaultTime;
            if (localTime == null)
                Debug.LogWarning("The local time of this object has not been set at the end of Start. This is not gonna end well!");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (lastPosition.HasValue && lastAngle.HasValue)
        {
            // Compute the owner new speed and angular speed
            if (LocalTime.DeltaTime != 0)
            {
                velocity = (transform.position - lastPosition.Value) / LocalTime.DeltaTime;
                angularVelocity = (transform.localRotation.eulerAngles.z - lastAngle.Value) / LocalTime.DeltaTime;
            }
            else
            {
                velocity = Vector3.zero;
                angularVelocity = 0;
            }
        }

        // Update for next frame
        lastPosition = transform.position;
        lastAngle = transform.localRotation.eulerAngles.z;
    }

    /// <summary>
    /// Adds a new action of the specified type to this object. Its base properties such as local time
    /// and parent game object are set automatically by this object.
    /// </summary>
    public TAction CreateAction<TAction>() where TAction: TimeInversibleAction
    {
        var actionRoot = new GameObject();
        var newAction = actionRoot.AddComponent<TAction>();
        actionRoot.name = newAction.ID.ToString();
        newAction.LocalTime = LocalTime;
        newAction.Owner = this;

        actionRoot.transform.SetParent(actionsRoot.transform);
        actionsStorage.Add( new Tuple<TimeInversibleAction, GameObject>(newAction, actionRoot));

        return newAction;
    }

    /// <summary>
    /// Signals that an action could not start. This may be due to the player creating time paradoxes.
    /// These failed actions should probably be discarded in such case.
    /// </summary>
    internal void SignalActionStartFailed(TimeInversibleAction action)
    {
        Debug.Log($"The following action could not start and will be discarded: {action}");
        DestroyAction(action);
    }

    /// <summary>
    /// Actions call this method when then completed backwards. It is generally a propert time to discard them
    /// as they will not be played again.
    /// </summary>
    internal void SignalActionCompletedBackwards(TimeInversibleAction action)
    {
        DestroyAction(action);
    }

    private void DestroyAction(TimeInversibleAction action)
    {
        int storageIndex = actionsStorage.FindIndex(t => t.Item1 == action);
        if (storageIndex >= 0)
        {
            GameObject actionObject = actionsStorage[storageIndex].Item2;
            actionsStorage.RemoveAt(storageIndex);
            Destroy(actionObject);
        }
        else
        {
            Debug.LogError($"The following action was not destroyed since it's not one of this object's actions: {action}");
        }
    }
}
