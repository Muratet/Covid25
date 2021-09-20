using UnityEngine;
using System.Collections.Generic;

public class Finances : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [HideInInspector]
    public float dailySpending = 0;

    [Tooltip("Price of a day of intensive care")]
    public int oneDayReanimationCost = 4628; // Prix d'une journée de réanimation

    public List<float> historySpent = new List<float>();
}