using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This action interpolates the color of an object between two colors.
/// It is possible to specify two couples of color depending on the time direction when the action started.
/// </summary>
public class SetColorAction : TimeInversibleAction
{
    public Renderer targetObject;
    public float globalStartTime = 1;
    public float localTimeDuration = 3;

    public Color forwardColor1 = Color.black;
    public Color forwardColor2 = Color.green;
    public Color backwardsColor1 = Color.black;
    public Color backwardsColor2 = Color.red;

    private Color currentColor1 = Color.black;
    private Color currentColor2 = Color.red;


    protected override void Update()
    {
        base.Update();

        // Action auto-start
        if (Time.time <= globalStartTime && Time.time + Time.deltaTime > globalStartTime)
        {
            Debug.Log("Starting action");
            StartForward();
        }
    }

    public override bool IsIntantaneous => localTimeDuration <= 0;

    protected override void OnStartedForward()
    {
        // When starting forward, use the first couple of colors and set the first one to
        // our target object.
        currentColor1 = forwardColor1;
        currentColor2 = forwardColor2;
        ApplyColor(currentColor1);
    }

    protected override void OnStartedBackwards()
    {
        // When starting backwards, use the other couple of colors.
        currentColor1 = backwardsColor1;
        currentColor2 = backwardsColor2;
        ApplyColor(currentColor2);
    }

    protected override bool OnMakingProgress()
    {
        // If we're in the Running state, we must have localTimeDuration != 0 and EntryTime defined.
        float ratio = localTimeDuration > 0 ? Mathf.Abs(LocalTime.Value - EntryTime.Value) / localTimeDuration : 1.0f;
        ApplyColor(Color.Lerp(currentColor1, currentColor2, ratio));
        return ratio >= 1.0f;
    }

    private void ApplyColor(Color c)
    {
        targetObject.material.color = c;
    }
}
