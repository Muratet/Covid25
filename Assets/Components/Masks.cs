using UnityEngine;
using System.Collections.Generic;

public class Masks : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [Tooltip("Réserve nationale de masques au début de la crise")]
    public float nationalStock = 140000000; // réserve nationale au début de la crise : 140 Milions de masques

    [Tooltip("Nombre de masques consommés journalièrement hors situation de crise")]
    public float medicalRequirementPerDay_low = 429000; // consomation régulaire : 3 Millions / semaine. Pour info : consomation au pic de la crise : 40 Millions / semaine
    [HideInInspector]
    public float medicalRequirementPerDay_current = 429000;

    [Tooltip("Production nationale de masques par défaut")]
    public float nationalProductionPerDay_low = 429000; // production régulaire : 3 millions / semaine
    [HideInInspector]
    public float nationalProductionPerDay_current = 429000;
    [Tooltip("Production nationale maximale de masques si production boostée")]
    public float nationalProductionPerDay_high = 2429000; // production boostée : 17 millions / semaine

    [Tooltip("Délai moyen entre deux livraisons")]
    public int deliveryTime = 8;
    [Tooltip("Estimation du nombre de masques par livraison")]
    public float maxDeliveryPack = 67000000; // estimation du nombre de masque par livraison avec 67 Millions de masque livré tout les 8 jours on atteind les 1 Milliards de masques commandés à la chine et livré en 4 mois.

    [Tooltip("Prix minimal d'un masque sur le marché")]
    public float maskMinPrice = 0.06f;
    [Tooltip("Prix courrant d'un masque sur le marché")]
    public float maskPrice = 0.06f; // en centimes, varie entre 6 et 40 centimes l'unité
    [Tooltip("Prix maximal d'un masque sur le marché")]
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