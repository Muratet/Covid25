using UnityEngine;
using System.Collections.Generic;

public class Finances : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [HideInInspector]
    public float dailySpending = 0;

    [Tooltip("Prix d'une journée de réanimation")]
    public int oneDayReanimationCost = 4628; // Prix d'une journée de réanimation

    public List<float> historySpent = new List<float>();
}