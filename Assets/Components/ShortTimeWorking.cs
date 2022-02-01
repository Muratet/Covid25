using UnityEngine;

public class ShortTimeWorking : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [HideInInspector]
    public int lastUpdate = -1;

    private TimeScale time;

    [HideInInspector]
    public bool currentState;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    public void OnShortTimeWorkingChange(bool newState)
    {
        lastUpdate = time.daysGone;
        currentState = newState;
    }
}