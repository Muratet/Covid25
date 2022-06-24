using UnityEngine;

/// <summary>
/// This component models impact of player choice on infection
/// </summary>
public class InfectionImpact : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    /// <summary>
    /// Gives the baseline impact on contagiousness of closing schools. This data is moderated by the age of the students. For example, closing universities does not affect the entire 18-23 year old age group because many are already employed and therefore not sensitive to this measure.
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of closing schools. This data is moderated by the age of the students. For example, closing universities does not affect the entire 18-23 year old age group because many are already employed and therefore not sensitive to this measure.")]
    public float schoolImpact = 0.25f;

    /// <summary>
    /// Gives the baseline impact on the contagiousness of the civic responsability call.
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of the civic responsability call")]
    public float civicismImpact = 0.15f;

    /// <summary>
    /// Gives the baseline impact on contagiousness of closing general public shops. 
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of closing general public shops")]
    public float shopImpact = 0.2f;

    /// <summary>
    /// Gives the baseline impact on contagiousness of reducing freedom of movements. 
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of reducing freedom of movements")]
    public float attestationImpact = 0.25f;

    /// <summary>
    /// Gives the baseline impact on contagiousness of exit ban depending on age restriction 
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of exit ban depending on age restriction")]
    public float ageRestrictionImpact = 0.7f;

    /// <summary>
    /// Gives the baseline impact on contagiousness of encouraging working from home
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of encouraging working from home")]
    public float remoteWorkingImpact = 0.13f;

    /// <summary>
    /// Gives the baseline impact on contagiousness of encouraging the homemade production of masks
    /// </summary>
    [Tooltip("Gives the baseline impact on R0 of encouraging the homemade production of masks")]
    public float selfProtectionImpact = 0.12f;
}