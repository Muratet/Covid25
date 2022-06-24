using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// This component contains data about vaccine
/// </summary>
public class Vaccine : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary></summary>
    public Slider UI_researchBar;
    /// <summary></summary>
    public TMP_InputField UI_vaccineQuantity;
    /// <summary></summary>
    public Button UI_vaccineCommand;
    /// <summary></summary>
    public TMP_Text UI_vaccineUnitPrice;
    /// <summary></summary>
    public TMP_Text UI_vaccinePendingCommand;

    /// <summary>
    /// quantity of vaccine ordered
    /// </summary>
    [HideInInspector]
    public float commands = 0;
    /// <summary>
    /// Average time between two deliveries
    /// </summary>
    [Tooltip("Average time between two deliveries")]
    public int deliveryTime = 8;
    /// <summary>
    /// Estimated number of vaccines per delivery
    /// </summary>
    [Tooltip("Estimated number of vaccines per delivery")]
    public float meanDeliveryPack = 400000; // For example, Sanofi produces 200 million doses for influenza per year (Sanofi being the biggest in this field). This gives us 200 M / 365 days ~= 650 000 doses per day. So every 8 days ~=> 4.5 Million doses to be distributed worldwide. The French population represents 0.87% of the world population (67M / 7.7Md). France would be entitled to receive 40 000 doses per week... With the other laboratories and the production boost, it is said that it could be multiplied by 10.
    /// <summary>
    /// number of days until next delivery
    /// </summary>
    [HideInInspector]
    public int nextDelivery = 0;

    /// <summary>
    /// Minimum price of a vaccine dose on the market
    /// </summary>
    [Tooltip("Minimum price of a vaccine dose on the market")]
    public int vaccineMinPrice = 10;
    /// <summary>
    /// Current price of a vaccine dose on the market
    /// </summary>
    [Tooltip("Current price of a vaccine dose on the market")]
    public int vaccinePrice = 20;
    /// <summary>
    /// Maximum price of a vaccine dose on the market
    /// </summary>
    [Tooltip("Maximum price of a vaccine dose on the market")]
    public int vaccineMaxPrice = 30;
    /// <summary></summary>
    [HideInInspector]
    public int lastOrderPlaced = -1;
    /// <summary></summary>
    [HideInInspector]
    public int lastOrderAmount = -1;

    /// <summary>
    /// National stockpile of vaccines
    /// </summary>
    [Tooltip("National stockpile of vaccines")]
    public float nationalStock = 0;

    /// <summary>
    /// Proportion of the population that trusts the vaccine (excluding anti-vaccines, those who cannot afford to travel, the unaware...)
    /// </summary>
    [Tooltip("Proportion of the population that trusts the vaccine (excluding anti-vaccines, those who cannot afford to travel, the unaware...)")]
    public float vaccineTrust = 0.8f;

    /// <summary>
    /// The history of vaccine stock
    /// </summary>
    public List<float> historyStock = new List<float>();
}