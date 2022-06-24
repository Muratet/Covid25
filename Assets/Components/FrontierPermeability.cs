using UnityEngine;

/// <summary>
/// This component models the permeability of frontiers (country level only)
/// </summary>
public class FrontierPermeability : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    /// <summary>
    /// permeability should be between 0 and 1, 0 means no population moving, 1 no restrictions
    /// </summary>
    [HideInInspector]
    public float permeability = 1f;

    /// <summary>
    /// The frontier state: 0-No restriction; 1-only global/european freight; 2-only european freight; 3-fully closed
    /// </summary>
    [HideInInspector]
    public int currentState = 0;


    /// <summary>
    /// The last time the player change restrictions on frontiers
    /// </summary>
    [HideInInspector]
    public int lastUpdate = -1;

    /// <summary>
    /// Rate of civilian flights (vs freight)
    /// </summary>
    [Tooltip("Rate of civilian flights (vs freight)")]
    public float travellerRatio = 0.7f; // 70% of flight concerns civil travels


    /// <summary>
    /// Rate of Inter-European freight
    /// </summary>
    [Tooltip("Rate of Inter-European freight")]
    public float europeanFreight = 0.59f; // 59% of freight traffic is inter-European

    private TimeScale time;

    private void Start()
    {
        time = GetComponent<TimeScale>();
    }

    /// <summary>
    /// Compute permeability depending on item selected
    /// </summary>
    /// <param name="newValue">The item selected</param>
    public void OnFrontierChange(ItemSelector newValue)
    {
        currentState = newValue.currentItem;
        // 0 => No restriction; 1-only global/european freight; 2-only european freight; 3-fully closed
        // 1 => Closure of borders for civilians / authorized business flow (world market)
        // 2 => Closure of borders for civilians / authorized business flow (only european market)
        // 3 => Total border closure
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