using UnityEngine;
using System.Collections.Generic;

public class Masks : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [Tooltip("National mask reserve at the start of the crisis")]
    public float nationalStock = 140000000; // réserve nationale au début de la crise : 140 Milions de masques

    [Tooltip("Number of masks consumed daily outside of a crisis situation")]
    public float medicalRequirementPerDay_low = 429000; // consomation régulaire : 3 Millions / semaine. Pour info : consomation au pic de la crise : 40 Millions / semaine
    [HideInInspector]
    public float medicalRequirementPerDay_current = 429000;

    [Tooltip("National production of masks by default")]
    public float nationalProductionPerDay_low = 429000; // production régulaire : 3 millions / semaine
    [HideInInspector]
    public float nationalProductionPerDay_current = 429000;
    [Tooltip("Maximum national production of masks if production boosted")]
    public float nationalProductionPerDay_high = 2429000; // production boostée : 17 millions / semaine

    [Tooltip("Average time between two deliveries")]
    public int deliveryTime = 8;
    [Tooltip("Estimated number of masks per delivery")]
    public float maxDeliveryPack = 67000000; // estimation du nombre de masque par livraison avec 67 Millions de masque livré tout les 8 jours on atteind les 1 Milliards de masques commandés à la chine et livré en 4 mois.

    [Tooltip("Minimum price of a mask on the market")]
    public float maskMinPrice = 0.06f;
    [Tooltip("Current price of a mask in the market")]
    public float maskPrice = 0.06f; // en centimes, varie entre 6 et 40 centimes l'unité
    [Tooltip("Maximum price of a mask on the market")]
    public float maskMaxPrice = 0.4f;

    public List<float> historyStock = new List<float>();

    [HideInInspector]
    public bool selfProtectionPromoted = false;

    [HideInInspector]
    public float commands = 0;

    [HideInInspector]
    public int nextDelivery = 0;

    [HideInInspector]
    public bool boostProduction = false;

    [HideInInspector]
    public bool requisition = false;

    [HideInInspector]
    public int lastRequisitionUpdate = -1;

    [HideInInspector]
    public int lastBoostProductionUpdate = -1;

    [HideInInspector]
    public int lastArtisanalProductionUpdate = -1;

    [HideInInspector]
    public int lastOrderPlaced = -1;

    [HideInInspector]
    public float lastOrderAmount = -1;
}