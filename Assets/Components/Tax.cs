using UnityEngine;

/// <summary>
/// This component is used to manage business charges
/// </summary>
public class Tax : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// The last time the player toggle this
    /// </summary>
    [HideInInspector]
    public int lastUpdate = -1;

    /// <summary>
    /// The cost of canceling taxes per day
    /// </summary>
    [Tooltip("The cost of canceling taxes per day")]
    public float compensateTaxesCanceled = 700000000; // 42 Billion in two months (French INSEE figure) => 700 Million per day max

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
    public void OnTaxChange(bool newValue)
    {
        lastUpdate = time.daysGone;
        currentState = newValue;
    }
}