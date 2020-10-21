using UnityEngine;
using System.Collections.Generic;

public class Revolution : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    [HideInInspector]
    public float stress = 0f; // Modélise le stress de la population [0, 100]

    public List<float> historyStress = new List<float>();

    [Tooltip("Si fermeture des écoles non justifié ajoute un maximum de stress par jour au rapport de la population de la région")]
    public float closePrimSchoolPenalty = 0.1f; // Si fermeture des écoles non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("Si fermeture des collèges non justifié ajoute un maximum de stress par jour au rapport de la population de la région")]
    public float closeScdSchoolPenalty = 0.1f; // Si fermeture des collèges non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("Si fermeture des lycées non justifié ajoute un maximum de stress par jour au rapport de la population de la région")]
    public float closeHighSchoolPenalty = 0.1f; // Si fermeture des lycées non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("Si fermeture des universités non justifié ajoute un maximum de stress par jour au rapport de la population de la région")]
    public float closeUniversityPenalty = 0.1f; // Si fermeture des universités non justifié ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("Si fermeture des écoles/collèges combiné avec le télétravail ajoute un maximum de stress par jour au rapport de la population de la région")]
    public float comboSchoolRemoteworkPenalty = 0.1f; // Si fermeture des écoles/collèges combiné avec le télétravail ajoute un maximum de stress par jour au rapport de la population de la région
    [Tooltip("Si appel civique non justifié ajoute un maximum de stress par jour")]
    public float callCivicPenalty = 0.5f; // Si appel civique non justifié ajoute un maximum de stress par jour
    [Tooltip("Si fermeture des commerces non justifié ajoute un maximum de stress par jour")]
    public float closeShopPenalty = 0.3f; // Si fermeture des commerces non justifié ajoute un maximum de stress par jour
    [Tooltip("Si restriction libertés non justifié ajoute un maximum de stress par jour")]
    public float certifRequiredPenalty = 0.3f; // Si restriction libertés non justifié ajoute un maximum de stress par jour
    [Tooltip("Si restriction age non justifié ajoute un maximum de stress par jour")]
    public float ageRestrictionPenalty = 0.8f; // Si restriction age non justifié ajoute un maximum de stress par jour
    [Tooltip("Si fermeture des frontières non justifiée ajoute un maximum de stress par jour")]
    public float closeFrontierPenalty = 0.2f; // Si fermeture des frontières non justifiée ajoute un maximum de stress par jour
    [Tooltip("Si ouverture des frontières en période de crise ajoute un maximum de stress par jour")]
    public float openFrontierPenalty = 0.1f; // Si ouverture des frontières en période de crise ajoute un maximum de stress par jour
    [Tooltip("Si télétravail en période de crise réduit un maximum de stress par jour")]
    public float remoteworkingBonus = 0.5f; // Si télétravail en période de crise réduit un maximum de stress par jour
    [Tooltip("Si chômage partiel en période de crise réduit un maximum de stress par jour")]
    public float shortTimeWorkBonus = 1f; // Si chômage partiel en période de crise réduit un maximum de stress par jour
    [Tooltip("Si aucune aide aux entreprises en période de crise ajoute un maximum de stress par jour")]
    public float taxPenalty = 0.3f; // Si aucune aide aux entreprises en période de crise ajoute un maximum de stress par jour
    [Tooltip("Si soutient aux entreprises en période de crise réduit un maximum de stress par jour")]
    public float taxSupportRequiredBonus = 0.1f; // Si soutient aux entreprises en période de crise réduit un maximum de stress par jour
    [Tooltip("Si soutient aux entreprises hors période de crise réduit un maximum de stress par jour")]
    public float taxSupportNotRequiredPenalty = 0.2f; // Si soutient aux entreprises hors période de crise réduit un maximum de stress par jour
    [Tooltip("Si réquisition des masques en période de pénurie ajoute un maximum de stress par jour")]
    public float maskRequisitionPenalty = 0.4f; // Si réquisition des masques en période de pénurie ajoute un maximum de stress par jour
    [Tooltip("Stock de masque minimal à partir duquel on est en pénurie")]
    public float maskStockThreshold = 30000000f; // Stock de masque minimal à partir duquel on est en pénurie
    [Tooltip("Si boost de la production en période de crise réduit un maximum de stress par jour")]
    public float maskBoostProdBonus = 0.05f; // Si boost de la production en période de crise réduit un maximum de stress par jour
    [Tooltip("Si demande d'auto protection non justifié augmente un maximum de stress par jour")]
    public float maskSelfProtectPenalty = 0.05f; // Si demande d'auto protection non justifié augmente un maximum de stress par jour
    [Tooltip("Définit le nombre de mort requis pour obtenir un coeff directeur à 1")]
    public float deathGradient = 10000f; // coeff directeur à 1 pour un nombre de mort de 1 pour "deathGradient"
    [Tooltip("Multiplicateur du stress si la pente d'évolution des morts est positive sur les 8 derniers jours")]
    public float deathIncreasePenalty = 1f; // Si pente de nouveau mort positive sur les 8 derniers jours => ajout de stress en multipliant la pente par "deathIncreasePenalty"
    [Tooltip("Réduction maximale du stress par jour si stagnation du nombre de morts")]
    public float deathStagnationBonus = 0.3f; // Si pas de nouveau mort sur les 8 derniers jours => réduit un maximum de stress par jour
    [Tooltip("Multiplicateur du stress si la pente d'évolution des morts est négative sur les 8 derniers jours")]
    public float deathDecreaseBonus = 0.7f; // Si pente de nouveau mort négative sur les 8 derniers jours => réduit le stress en fonction de la pente pour un maximum de "deathDecreaseBonus"
    [Tooltip("Seuil du nombre de personne en cours d'infection à partir duquel la population inverse son raisonnement (pris en compte sur une fenêtre glissante correspondant à la durée de l'incubation)")]
    public int criticThreshold = 1000; // seuil du nombre de personne en cours d'infection à partir duquel la population inverse son raisonnement (pris en compte sur une fenêtre glissante correspondant à la durée de l'incubation)

    [HideInInspector]
    public bool nationalInfectionIsCritic = false; // permet de savoir si la population de ce territoire perçoit la criticité de la situation
    [HideInInspector]
    public float currentStressOnCriticToggled = 0; // niveau de stress au moment du changement de la perseption de la criticité de la situation par la population
}