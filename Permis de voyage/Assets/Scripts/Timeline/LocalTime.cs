using UnityEngine;

/// <summary>
/// Time value for a timeline whose speed may differ from the base Unity engine time.
/// </summary>
public class LocalTime: MonoBehaviour
{
    /// <summary>
    /// Value of this local time. Starts at zero by default.
    /// </summary>
    public float Value
    {
        get => value;
        set { this.value = value; }
    }
    [SerializeField]
    private float value = 0;

    /// <summary>
    /// Value of this local time. Starts at zero by default.
    /// </summary>
    public float DeltaTime => Time.deltaTime * relativeSpeed;

    /// <summary>
    /// Speed at which this time flows, relative to the Unity engine time (Time.time).
    /// 1.0 is like the base time, 0 is frozen, -1.0 goes backards at normal speed, etc.
    /// </summary>
    public float RelativeSpeed
    {
        get => relativeSpeed;
        set { relativeSpeed = value; }
    }
    [SerializeField]
    private float relativeSpeed = 1.0f;

    protected virtual void Update()
    {
        value += DeltaTime;
    }

    public override string ToString()
    {
        return $"[Local time: {Value} (speed: {RelativeSpeed}, delta: {DeltaTime})]";
    }
}