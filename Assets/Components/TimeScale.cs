using UnityEngine;

public class TimeScale : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [Tooltip("Duration of a day in seconds")]
    public float dayVelocity = 1;
    [HideInInspector]
    public float timeElapsed;
    [HideInInspector]
    public bool newDay = false;
    [HideInInspector]
    public int daysGone = 0;

    public void setDayVelocity(float newVelocity)
    {
        dayVelocity = newVelocity;
    }
}