using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This component include data to manage population dissatisfaction
/// </summary>
public class Revolution : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// Models the population stress [0, 100]
    /// </summary>
    [HideInInspector]
    public float stress = 0f;

    /// <summary>
    /// The history of population dissatisfaction
    /// </summary>
    public List<float> historyStress = new List<float>();

    /// <summary>
    /// If unjustified primary school closures then add penalty depending on territory population importance
    /// </summary>
    [Tooltip("If unjustified school closures then add penalty depending on territory population importance")]
    public float closePrimSchoolPenalty = 0.1f;
    /// <summary>
    /// If unjustified college closure then adds penalty depending on territory population importance
    /// </summary>
    [Tooltip("If unjustified secondary school closure adds penalty depending on territory population importance")]
    public float closeScdSchoolPenalty = 0.1f;
    /// <summary>
    /// If unjustified college closure then adds penalty depending on territory population importance
    /// </summary>
    [Tooltip("If unjustified college closure adds penalty depending on territory population importance")]
    public float closeHighSchoolPenalty = 0.1f;
    /// <summary>
    /// If unjustified university closure then adds penalty depending on territory population importance
    /// </summary>
    [Tooltip("If unjustified university closure then adds penalty depending on territory population importance")]
    public float closeUniversityPenalty = 0.1f;
    /// <summary>
    /// If primary/secondary scholl closure is combined with home working then adds penalty depending on territory population importance
    /// </summary>
    [Tooltip("If primary/secondary scholl closure is combined with home working then adds penalty depending on territory population importance")]
    public float comboSchoolRemoteworkPenalty = 0.1f;
    /// <summary>
    /// If unjustified civic call then adds penalty
    /// </summary>
    [Tooltip("If unjustified civic call then adds penalty")]
    public float callCivicPenalty = 0.5f;
    /// <summary>
    /// If unjustified shop closing then adds penalty
    /// </summary>
    [Tooltip("If unjustified shop closing then adds penalty")]
    public float closeShopPenalty = 0.3f;
    /// <summary>
    /// If unjustified reducing freedom of movement then adds penalty
    /// </summary>
    [Tooltip("If unjustified reducing freedom of movement then adds penalty")]
    public float certifRequiredPenalty = 0.3f;
    /// <summary>
    /// If unjustified age restriction then adds penalty
    /// </summary>
    [Tooltip("If unjustified age restriction then adds penalty")]
    public float ageRestrictionPenalty = 0.8f;
    /// <summary>
    /// If unjustified border closure then adds penalty
    /// </summary>
    [Tooltip("If unjustified border closure then adds penalty")]
    public float closeFrontierPenalty = 0.2f;
    /// <summary>
    /// If borders openned during a crisis then adds penalty
    /// </summary>
    [Tooltip("If borders openned during a crisis then adds penalty")]
    public float openFrontierPenalty = 0.1f;
    /// <summary>
    /// If home working during a crisis then adds bonus
    /// </summary>
    [Tooltip("If home working during a crisis then adds bonus")]
    public float remoteworkingBonus = 0.5f; 
    /// <summary>
    /// If partial unemployment during crisis then adds bonus
    /// </summary>
    [Tooltip("If partial unemployment during crisis then adds bonus")]
    public float shortTimeWorkBonus = 1f;
    /// <summary>
    /// If no help for companies during crisis then adds penalty
    /// </summary>
    [Tooltip("If no help for companies during crisis then adds penalty")]
    public float taxPenalty = 0.3f;
    /// <summary>
    /// If help companies during crisis then adds penalty
    /// </summary>
    [Tooltip("If help companies during crisis then adds penalty")]
    public float taxSupportRequiredBonus = 0.1f;
    /// <summary>
    /// If help comanies without crisis then adds bonus
    /// </summary>
    [Tooltip("If help comanies without crisis then adds bonus")]
    public float taxSupportNotRequiredPenalty = 0.2f;
    /// <summary>
    /// if masks requisition without stock then add penalty
    /// </summary>
    [Tooltip("if masks requisition without stock then add penalty")]
    public float maskRequisitionPenalty = 0.4f;
    /// <summary>
    /// Minimum mask stock at which there is a shortage
    /// </summary>
    [Tooltip("Minimum mask stock at which there is a shortage")]
    public float maskStockThreshold = 30000000f;
    /// <summary>
    /// If mask production boosted during crisis then add nobus
    /// </summary>
    [Tooltip("If mask production boosted during crisis then add nobus")]
    public float maskBoostProdBonus = 0.05f;
    /// <summary>
    /// If unjustified self-protection request then adds penalty
    /// </summary>
    [Tooltip("If unjustified self-protection request then adds penalty")]
    public float maskSelfProtectPenalty = 0.05f;
    /// <summary>
    /// Defines the number of deaths required to obtain a steering coefficient of 1
    /// </summary>
    [Tooltip("Defines the number of deaths required to obtain a steering coefficient of 1")]
    public float deathGradient = 10000f;
    /// <summary>
    /// Stress multiplier if the slope of the evolution of deaths is positive over the last 8 days
    /// </summary>
    [Tooltip("Stress multiplier if the slope of the evolution of deaths is positive over the last 8 days")]
    public float deathIncreasePenalty = 1f;
    /// <summary>
    /// Maximum reduction of stress per day if the number of deaths stagnates
    /// </summary>
    [Tooltip("Maximum reduction of stress per day if the number of deaths stagnates")]
    public float deathStagnationBonus = 0.3f;
    /// <summary>
    /// Stress multiplier if the slope of the evolution of deaths is negative over the last 8 days
    /// </summary>
    [Tooltip("Stress multiplier if the slope of the evolution of deaths is negative over the last 8 days")]
    public float deathDecreaseBonus = 0.7f;
    /// <summary>
    /// Threshold of the number of infected people at which the population reverses its reasoning (taken into account on a sliding window corresponding to the duration of the incubation)
    /// </summary>
    [Tooltip("Threshold of the number of infected people at which the population reverses its reasoning (taken into account on a sliding window corresponding to the duration of the incubation)")]
    public int criticThreshold = 1000;

    /// <summary>
    /// Allows to know if the population of this territory perceives the criticality of the situation
    /// </summary>
    [HideInInspector]
    public bool nationalInfectionIsCritic = false;
    /// <summary>
    /// Current population stress at the time of the change in the population's perception of the criticality of the situation
    /// </summary>
    [HideInInspector]
    public float currentStressOnCriticToggled = 0;
}