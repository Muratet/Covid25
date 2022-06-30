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
    /// The money of the country
    /// </summary>
    public string money = "€";

    /// <summary>
    /// Price of one day of ICU bed occupency
    /// </summary>
    [Tooltip("Price of one day of ICU bed occupency")]
    public float oneDayReanimationCost = 4628;

    /// <summary>
    /// Losses due to borders closing per day (tourism)
    /// </summary>
    [Tooltip("Losses due to borders closing per day (tourism)")]
    public float costLostTourismPerDay = 100000;

    /// <summary>
    /// Losses due to borders closing per day (freight)
    /// </summary>
    [Tooltip("Losses due to borders closing per day (freight)")]
    public float costLostFreightPerDay = 1400000;

    /// <summary>
    /// The history of spent
    /// </summary>
    public List<float> historySpent = new List<float>();
}