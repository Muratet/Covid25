using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FrontierPermeability : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [HideInInspector]
    public float permeability = 1f;
    [HideInInspector]
    public int currentState = 0;
    [HideInInspector]
    public int lastUpdate = -1;

    // 70% du traffic aérien concerne les voyages privés de civil
    [Tooltip("Air traffic rate for passenger flights")]
    public float travellerRatio = 0.7f;
    // 59% du traffic de marchandise est inter-européen
    [Tooltip("Inter-European merchandise traffic rate")]
    public float europeanFreight = 0.59f;

    private TimeScale time;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    public void OnFrontierChange(int newValue)
    {
        currentState = newValue;
        // 0 => Aucune fermeture
        // 1 => Fermeture des frontières à titre privé / commercial autorisé (niveau mondial)
        // 2 => Fermeture des frontières à titre privé / commercial autorisé (niveau européen)
        // 3 => Fermeture des frontières total (privé et commercial)
        if (newValue == 0)
            permeability = 1f;
        else if (newValue == 1)
            permeability = 1f - travellerRatio;
        else if (newValue == 2)
            permeability = (1f - travellerRatio) * europeanFreight;
        else
            permeability = 0.1f;

        lastUpdate = time.daysGone;
    }
}