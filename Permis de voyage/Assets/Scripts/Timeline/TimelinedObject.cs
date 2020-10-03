using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LocalTime))]
public class TimelinedObject : MonoBehaviour
{
    /// <summary>
    /// The local time of this object. It will grow if the object is moving forward in time, and decrease
    /// if the object is moving backwards in time. (compared to the reference "real-world" Time.time).
    /// </summary>
    public LocalTime LocalTime
    {
        get {
            localTime = localTime ?? GetComponent<LocalTime>();
            return localTime;
        }
    }
    private LocalTime localTime;
}
