using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Vaccine : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    public Slider UI_researchBar;
    public TMP_InputField UI_vaccineQuantity;
    public Button UI_vaccineCommand;
    public TMP_Text UI_vaccineUnitPrice;
    public TMP_Text UI_vaccinePendingCommand;

    [HideInInspector]
    public float commands = 0; // quantité de vaccin commandés
    [Tooltip("Délai moyen entre deux livraisons")]
    public int deliveryTime = 8;
    [Tooltip("Estimation du nombre de vaccins par livraison")]
    public float meanDeliveryPack = 400000; // Sanofi produit par exemple 200 Millions de doses pour la grippe par an (Sanofi étant le plus gros dans ce domaine). Ca nous donne 200 M / 365j ~= 650 000 doses par jours. Soit tous les 8 jours ~=> 4,5 Millions de doses à distribuer dans le monde entier. La population française représentant 0,87% de la population mondiale (67M / 7,7Md). La France serait en droit de recevoir 40 000 doses par semaine... Avec les autres laboratoire et le boost de la production, on dit que ça pourrait être multiplier par 10.
    [HideInInspector]
    public int nextDelivery = 0; // nombre de jour avant la prochaine livraison

    [Tooltip("Prix minimal d'une dose de vaccin sur le marché")]
    public int vaccineMinPrice = 10;
    [Tooltip("Prix courrant d'une dose de vaccin sur le marché")]
    public int vaccinePrice = 20; // en euros, varie entre 10 et 30 euro l'unité
    [Tooltip("Prix maximal d'une dose de vaccin sur le marché")]
    public int vaccineMaxPrice = 30;
    [HideInInspector]
    public int lastOrderPlaced = -1;
    [HideInInspector]
    public int lastOrderAmount = -1;

    [Tooltip("Stock national de vaccins")]
    public float nationalStock = 0;

    [Tooltip("Proportion de la population qui fait confiance au vaccin (exclusion des anti-vaccins, ceux qui n'ont pas de moyens de se déplacer, les non sensibilisés...)")]
    public float vaccineTrust = 0.8f; // proportion de la population qui fait confiance au vaccin (exclusion des anti-vaccins, ceux qui n'ont pas de moyens de se déplacer, les non sensibilisés...)

    public List<float> historyStock = new List<float>();
}