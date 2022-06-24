using UnityEngine;

/// <summary>
/// This component is used to store virus data
/// </summary>
public class VirusStats : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// Models the current R0 of the virus. For example, if equal to 3 it means on average that each person contaminates 3 other persons on X days of contagiousness. So for a cluster of 10 persons over 10 days 30 other persons will be contaminated.
    /// </summary>
    [Tooltip("Models the current R0 of the virus")]
    public float contagiosity = 3f;
    /// <summary>
    /// Rate of infected population at which the pandemic is self-limiting without any specific action
    /// </summary>
    [Tooltip("Rate of infected population at which the pandemic is self-limiting without any specific action")]
    public float populationRatioImmunity = 0.67f;

    /// <summary>
    /// Size of the sliding window to store the number of infected people
    /// </summary>
    [Tooltip("Size of the sliding window to store the number of infected people")]
    public int windowSize = 21;

    // Data to calculate the Gaussian on the sliding window size
    // ... for contagiousness
    /// <summary>
    /// Average peak symptom onset
    /// </summary>
    [Tooltip("Average peak symptom onset")]
    public float contagiousnessPeak = 10;
    /// <summary>
    /// Standard deviation of symptom onset
    /// </summary>
    [Tooltip("Standard deviation of symptom onset")]
    public float contagiousnessDeviation = 3f;
    // ... for lethality
    /// <summary>
    /// Average peak mortality
    /// </summary>
    [Tooltip("Average peak mortality")]
    public float deadlinessPeak = 12; // Staggered by 2 days after the average appearance of the first symptoms...
    /// <summary>
    /// Standard deviation of mortality
    /// </summary>
    [Tooltip("Standard deviation of mortality")]
    public float deadlinessDeviation = 1.8f;

    /// <summary>
    /// Rate of serious cases
    /// </summary>
    [Tooltip("Rate of serious cases")]
    public float seriousRatio = 0.138f;

    /// <summary>
    /// Age at which you can start having deaths
    /// </summary>
    [Tooltip("Age at which you can start having deaths")]
    public int firstSensitiveAge = 30;
    /// <summary>
    /// Control of the curvature of the exponential, 1 => classic exponential, < 1 => faster acceleration, > 1 => slower acceleration
    /// </summary>
    [Tooltip("Control of the curvature of the exponential, 1 => classic exponential, < 1 => faster acceleration, > 1 => slower acceleration")]
    public int curveStrenght = 12;
    /// <summary>
    /// Maximum mortality rate for the oldest age group
    /// </summary>
    [Tooltip("Maximum mortality rate for the oldest age group")]
    public float maxDeadlinessRatio = 0.148f;
    /// <summary>
    /// Number of months required to discover the vaccine
    /// </summary>
    [Tooltip("Number of months required to discover the vaccine")]
    public int vaccineMounthDelay = 18;
}