using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// This component contains all the data related to the management of masks
/// </summary>
public class Masks : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// National reserve of masks at the beginning of the crisis
    /// </summary>
    [Tooltip("National reserve of masks at the beginning of the crisis")]
    public float nationalStock = 140000000;

    /// <summary>
    /// Number of masks consumed daily outside of crisis situations
    /// </summary>
    [Tooltip("Number of masks consumed daily outside of crisis situations")]
    public float medicalRequirementPerDay_low = 429000; // In France - regular consumption: 3 Millions / week. For information: consumption at the peak of the crisis : 40 Millions / week
    /// <summary>
    /// The required amount of masks to satisfy daily consumption
    /// </summary>
    [HideInInspector]
    public float medicalRequirementPerDay_current = 429000;

    /// <summary>
    /// Default national production of masks
    /// </summary>
    [Tooltip("Default national production of masks")]
    public float nationalProductionPerDay_low = 429000; // In France - regular production: 3 millions / week
    /// <summary>
    /// The current national production of masks
    /// </summary>
    [HideInInspector]
    public float nationalProductionPerDay_current = 429000;
    /// <summary>
    /// Maximum national production of masks if production is boosted
    /// </summary>
    [Tooltip("Maximum national production of masks if production is boosted")]
    public float nationalProductionPerDay_high = 2429000; // In France - boosted production: 17 millions / week

    /// <summary>
    /// Average time between shipments
    /// </summary>
    [Tooltip("Average time between shipments")]
    public int deliveryTime = 8;
    /// <summary>
    /// Estimated number of masks per shipments
    /// </summary>
    [Tooltip("Estimated number of masks per shipments")]
    public float maxDeliveryPack = 67000000; // In France - estimation of the number of masks per delivery with 67 Millions of masks delivered every 8 days we reach the 1 Billion masks ordered to China and delivered in 4 months.

    /// <summary>
    /// Minimum price of a mask on the market
    /// </summary>
    [Tooltip("Minimum price of a mask on the market")]
    public float maskMinPrice = 0.06f;
    /// <summary>
    /// Current price of a mask on the market
    /// </summary>
    [Tooltip("Current price of a mask on the market")]
    public float maskPrice = 0.06f;
    /// <summary>
    /// Maximum price of a mask on the market
    /// </summary>
    [Tooltip("Maximum price of a mask on the market")]
    public float maskMaxPrice = 0.4f;

    /// <summary>
    /// The history of masks stock
    /// </summary>
    public List<float> historyStock = new List<float>();

    /// <summary></summary>
    [HideInInspector]
    public bool selfProtectionPromoted = false;

    /// <summary></summary>
    [HideInInspector]
    public float commands = 0;

    /// <summary></summary>
    [HideInInspector]
    public int nextDelivery = 0;

    /// <summary></summary>
    [HideInInspector]
    public bool boostProduction = false;

    /// <summary></summary>
    [HideInInspector]
    public bool requisition = false;

    /// <summary></summary>
    [HideInInspector]
    public int lastRequisitionUpdate = -1;

    /// <summary></summary>
    [HideInInspector]
    public int lastBoostProductionUpdate = -1;

    /// <summary></summary>
    [HideInInspector]
    public int lastArtisanalProductionUpdate = -1;

    /// <summary></summary>
    [HideInInspector]
    public int lastOrderPlaced = -1;

    /// <summary></summary>
    [HideInInspector]
    public float lastOrderAmount = -1;

    /// <summary></summary>
    public BaseEventData e;
}