using UnityEngine;
using System.Collections.Generic;

public class Revolution : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    [HideInInspector]
    public float stress = 0f; // Modélise le stress de la population [0, 100]

    public List<float> historyStress = new List<float>();

    [Tooltip("If the closure of schools without justification adds a maximum of stress per day to the ratio of the population of the region")]
    public float closePrimSchoolPenalty = 0.1f; // Si fermeture des écoles non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("If the closure of middle schools not justified adds a maximum of stress per day to the ratio of the population of the region")]
    public float closeScdSchoolPenalty = 0.1f; // Si fermeture des collèges non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("If the closure of high schools without justification adds a maximum of stress per day to the ratio of the population of the region")]
    public float closeHighSchoolPenalty = 0.1f; // Si fermeture des lycées non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("If unjustified closure of universities adds a maximum of stress per day to the ratio of the population of the region")]
    public float closeUniversityPenalty = 0.1f; // Si fermeture des universités non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("If closing of schools / middle school combined with teleworking adds maximum stress per day to the ratio of the population of the region")]
    public float comboSchoolRemoteworkPenalty = 0.1f; // Si fermeture des écoles/collèges combiné avec le télétravail ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("If civic appeal not justified adds a maximum of stress per day")]
    public float callCivicPenalty = 0.5f; // Si appel civique non justifié ajoute un maximum de stress par jour
    [Tooltip("If the unjustified closure of shops adds a maximum of stress per day")]
    public float closeShopPenalty = 0.3f; // Si fermeture des commerces non justifié ajoute un maximum de stress par jour
    [Tooltip("If restriction of freedoms not justified adds a maximum of stress per day")]
    public float certifRequiredPenalty = 0.3f; // Si restriction libertés non justifié ajoute un maximum de stress par jour
    [Tooltip("If age restriction not justified adds a maximum of stress per day")]
    public float ageRestrictionPenalty = 0.8f; // Si restriction age non justifié ajoute un maximum de stress par jour
    [Tooltip("If border closure not justified adds a maximum of stress per day")]
    public float closeFrontierPenalty = 0.2f; // Si fermeture des frontières non justifiée ajoute un maximum de stress par jour
    [Tooltip("If opening borders in times of crisis adds a maximum of stress per day")]
    public float openFrontierPenalty = 0.1f; // Si ouverture des frontières en période de crise ajoute un maximum de stress par jour
    [Tooltip("If teleworking in times of crisis reduces a maximum of stress per day")]
    public float remoteworkingBonus = 0.5f; // Si télétravail en période de crise réduit un maximum de stress par jour
    [Tooltip("If partial unemployment in times of crisis reduces a maximum of stress per day")]
    public float shortTimeWorkBonus = 1f; // Si chômage partiel en période de crise réduit un maximum de stress par jour
    [Tooltip("If no help to businesses in times of crisis adds maximum stress per day")]
    public float taxPenalty = 0.3f; // Si aucune aide aux entreprises en période de crise ajoute un maximum de stress par jour
    [Tooltip("If supporting companies in times of crisis reduces maximum stress per day")]
    public float taxSupportRequiredBonus = 0.1f; // Si soutient aux entreprises en période de crise réduit un maximum de stress par jour
    [Tooltip("If support for companies outside the crisis period reduces a maximum of stress per day")]
    public float taxSupportNotRequiredPenalty = 0.2f; // Si soutient aux entreprises hors période de crise réduit un maximum de stress par jour
    [Tooltip("If requisition of masks in times of shortage adds a maximum of stress per day")]
    public float maskRequisitionPenalty = 0.4f; // Si réquisition des masques en période de pénurie ajoute un maximum de stress par jour
    [Tooltip("Minimum mask stock from which there is a shortage")]
    public float maskStockThreshold = 30000000f; // Stock de masque minimal à partir duquel on est en pénurie
    [Tooltip("If boost in production in times of crisis reduces maximum stress per day")]
    public float maskBoostProdBonus = 0.05f; // Si boost de la production en période de crise réduit un maximum de stress par jour
    [Tooltip("If unjustified self-protection request increases maximum stress per day")]
    public float maskSelfProtectPenalty = 0.05f; // Si demande d'auto protection non justifié augmente un maximum de stress par jour
    [Tooltip("Defines the number of deaths required to obtain a directing coefficient at 1")]
    public float deathGradient = 10000f; // coeff directeur à 1 pour un nombre de mort de 1 pour "deathGradient"
    [Tooltip("Stress multiplier if the slope of the evolution of deaths is positive over the last 8 days")]
    public float deathIncreasePenalty = 1f; // Si pente de nouveau mort positive sur les 8 derniers jours => ajout de stress en multipliant la pente par "deathIncreasePenalty"
    [Tooltip("Maximum reduction in stress per day if the number of deaths stagnates")]
    public float deathStagnationBonus = 0.3f; // Si pas de nouveau mort sur les 8 derniers jours => réduit un maximum de stress par jour
    [Tooltip("Stress multiplier if the slope of the evolution of deaths is negative over the last 8 days")]
    public float deathDecreaseBonus = 0.7f; // Si pente de nouveau mort négative sur les 8 derniers jours => réduit le stress en fonction de la pente pour un maximum de "deathDecreaseBonus"
    [Tooltip("Threshold of the number of people during infection from which the population reverses its reasoning (taken into account on a sliding window corresponding to the duration of the incubation)")]
    public int criticThreshold = 1000; // seuil du nombre de personne en cours d'infection à partir duquel la population inverse son raisonnement (pris en compte sur une fenêtre glissante correspondant à la durée de l'incubation)

    [HideInInspector]
    public bool nationalInfectionIsCritic = false; // permet de savoir si la population de ce territoire perçoit la criticité de la situation
    [HideInInspector]
    public float currentStressOnCriticToggled = 0; // niveau de stress au moment du changement de la perseption de la criticité de la situation par la population
}