using UnityEngine;

/// <summary>
/// This component contains all the data related to the home working
/// </summary>
public class Remoteworking : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// The last time the player toggle this
    /// </summary>
    [HideInInspector]
    public int lastUpdate = -1;

    /// <summary>
    /// The current state of home working
    /// </summary>
    [HideInInspector]
    public bool currentState;

    private TimeScale time;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    /// <summary>
    /// Called when this is toggle
    /// </summary>
    /// <param name="newState"></param>
    public void OnRemoteworkingChange(bool newState)
    {
        lastUpdate = time.daysGone;
        currentState = newState;
        SyncUISystem.needUpdate = true;
    }

}