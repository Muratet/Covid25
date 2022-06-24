using UnityEngine;

/// <summary>
/// This component include data to manage partial unemployment
/// </summary>
public class ShortTimeWorking : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// The last time the player toggle this
    /// </summary>
    [HideInInspector]
    public int lastUpdate = -1;

    private TimeScale time;

    /// <summary>
    /// Current state of this
    /// </summary>
    [HideInInspector]
    public bool currentState;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    /// <summary>
    /// Callback when player toggle this
    /// </summary>
    /// <param name="newState"></param>
    public void OnShortTimeWorkingChange(bool newState)
    {
        lastUpdate = time.daysGone;
        currentState = newState;
    }
}