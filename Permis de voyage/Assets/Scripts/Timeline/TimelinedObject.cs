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
    public bool IsMoving { get; private set; }

    private GameObject actionsRoot;
    private static readonly string actionsRootName = "Actions";

    private Vector3 lastTargetPosition;
    private Quaternion lastTargetOrientation;

    /// <summary>
    /// Each registered action is given a private GameObject where it can store its stuff, and make it easier to clean.
    /// This field remembers which goes where.
    /// </summary>
    private List<Tuple<TimeInversibleAction, GameObject>> actionsStorage = new List<Tuple<TimeInversibleAction, GameObject>>();
    int nextActionID = 0;

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
            // There may be a global LocalTime instance or something like this which should be given to the object.
            // This design question has not yet been solved, but in then end this LocalTime will have to make its way here.
            Debug.LogWarning("The local time of this object has not been set before its Start. This is not gonna end well!");
        }
    }

    protected virtual void Update()
    {
        IsMoving = (
            lastTargetPosition != transform.position ||
            lastTargetOrientation != transform.rotation);
        lastTargetPosition = transform.position;
        lastTargetOrientation = transform.rotation;
    }

    /// <summary>
    /// Adds a new action of the specified type to this object. Its base properties such as local time
    /// and parent game object are set automatically by this object.
    /// </summary>
    public TAction CreateAction<TAction>() where TAction: TimeInversibleAction
    {

        int newActionID = nextActionID++;
        var actionRoot = new GameObject(newActionID.ToString());
        var newAction = actionRoot.AddComponent<TAction>();
        newAction.LocalTime = LocalTime;
        newAction.Owner = this;

        actionRoot.transform.SetParent(actionsRoot.transform);
        actionsStorage.Add( new Tuple<TimeInversibleAction, GameObject>(newAction, actionRoot));

        return newAction;
    }

    public void SignalActionStartFailed(TimeInversibleAction action)
    {
        Debug.Log($"The following action could not start and will be discarded: {action}");
        DestroyAction(action);
    }

    private void DestroyAction(TimeInversibleAction action)
    {
        int storageIndex = actionsStorage.FindIndex(t => t.Item1 == action);
        if (storageIndex >= 0)
        {
            actionsStorage.RemoveAt(storageIndex);
            GameObject actionObject = actionsStorage[storageIndex].Item2;
            Destroy(actionObject);
        }
        else
        {
            Debug.LogError($"The following action was not destroyed since it's not one of this object's actions: {action}");
        }
    }
}
