using UnityEngine;
using System.Collections;

/// <summary>
/// Reverses the direction of the local time clock on the same GameObject every N seconds.
/// </summary>
public class TimePingPong : MonoBehaviour
{
    public float globalTimePeriod = 5;

    protected virtual void Start()
    {
        StartCoroutine(DoPingPongWithTime());
    }

    protected virtual IEnumerator DoPingPongWithTime()
    {
        var strings = new string[] { "Ping", "Pong" };
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(globalTimePeriod);
            Debug.Log(strings[i]);
            i = 1 - i;
            GetComponent<LocalTime>().RelativeSpeed *= -1;
        }
    }
}
