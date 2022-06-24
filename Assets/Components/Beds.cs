using UnityEngine;

/// <summary>
/// This component include data to manage beds in a territory
/// </summary>
public class Beds : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// Number of ICU beds available before crisis
    /// </summary>
    [Tooltip("Number of ICU beds available before crisis")]
    public int intensiveBeds_default = 5058;

    /// <summary>
    /// Number of ICU beds occupancy
    /// </summary>
    [HideInInspector]
    public int intensiveBeds_current = 5058;

    /// <summary>
    /// Maximum number of ICU beds (models the maximum capacity of ICU beds if we cancel non emergency operations, requisition of private hospitals...)
    /// </summary>
    [Tooltip("Nombre maximum de lits de réanimation disponibles")]
    public int intensiveBeds_high = 14523;

    /// <summary>
    /// The need of ICU beds at a given time
    /// </summary>
    [HideInInspector]
    public int intensiveBeds_need = 0;

    /// <summary>
    /// True if the player ask to boost the number of ICU beds, False else
    /// </summary>
    [HideInInspector]
    public bool boostBeds = false;

    /// <summary>
    /// The last time the player toggle "boostBeds"
    /// </summary>
    [HideInInspector]
    public int boostBedsLastUpdate = -1;

    /// <summary>
    /// Number of consecutive days since the last advisor notification 
    /// </summary>
    [HideInInspector]
    public int advisorNotification = -1;
}