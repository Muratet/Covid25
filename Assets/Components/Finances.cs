using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This component is used to store financial data
/// </summary>
public class Finances : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    /// <summary>
    /// The amount of spent for the current day
    /// </summary>
    [HideInInspector]
    public float dailySpending = 0;

    /// <summary>
    /// Price of one day of ICU bed occupency
    /// </summary>
    [Tooltip("Price of one day of ICU bed occupency")]
    public int oneDayReanimationCost = 4628;

    /// <summary>
    /// The history of spent
    /// </summary>
    public List<float> historySpent = new List<float>();
}