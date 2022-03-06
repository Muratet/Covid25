using UnityEngine;

public class FrontierPermeability : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [HideInInspector]
    public float permeability = 1f;
    [HideInInspector]
    public int currentState = 0;
    [HideInInspector]
    public int lastUpdate = -1;

    // 70% du traffic aérien concerne les voyages privés de civil
    [Tooltip("Taux du traffic aérien concernant les vols de voyageurs")]
    public float travellerRatio = 0.7f;
    // 59% du traffic de marchandise est inter-européen
    [Tooltip("Taux du traffic de marchandise inter-européen")]
    public float europeanFreight = 0.59f;

    private TimeScale time;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    public void OnFrontierChange(ItemSelector newValue)
    {
        currentState = newValue.currentItem;
        // 0 => Aucune fermeture
        // 1 => Fermeture des frontières à titre privé / commercial autorisé (niveau mondial)
        // 2 => Fermeture des frontières à titre privé / commercial autorisé (niveau européen)
        // 3 => Fermeture des frontières total (privé et commercial)
        if (currentState == 0)
            permeability = 1f;
        else if (currentState == 1)
            permeability = 1f - travellerRatio;
        else if (currentState == 2)
            permeability = (1f - travellerRatio) * europeanFreight;
        else
            permeability = 0.1f;

        lastUpdate = time.daysGone;
    }
}