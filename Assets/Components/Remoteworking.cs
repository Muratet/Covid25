using UnityEngine;

public class Remoteworking : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    [HideInInspector]
    public int lastUpdate = -1;

    [HideInInspector]
    public bool currentState;

    private TimeScale time;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    public void OnRemoteworkingChange(bool newState)
    {
        lastUpdate = time.daysGone;
        currentState = newState;
        SyncUISystem.needUpdate = true;
    }

}