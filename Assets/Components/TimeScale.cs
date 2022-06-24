using UnityEngine;

/// <summary>
/// This component is used to manage the speed of the gaùe
/// </summary>
public class TimeScale : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// Duration of a day in second grade
    /// </summary>
    [Tooltip("Duration of a day in second grade")]
    public float dayVelocity = 1;
    /// <summary>Time elapsed since the game started</summary>
    [HideInInspector]
    public float timeElapsed;
    /// <summary>True if the current frame is a new day</summary>
    [HideInInspector]
    public bool newDay = false;
    /// <summary>Number of days gone since the game started</summary>
    [HideInInspector]
    public int daysGone = 0;

    /// <summary>
    /// Change game speed
    /// </summary>
    /// <param name="newVelocity"></param>
    public void setDayVelocity(float newVelocity)
    {
        dayVelocity = newVelocity;
    }
}