using UnityEngine;

public class InfectionImpact : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    // Donne l'impact de base sur la contagiosité de la fermeture des écoles (par défaut réduit la contagiosité de 20%). Cette donnée est modérée en fontion de l'age des élèves. Si on ferme les Universités par exemple on ne touche pas toute la tranche d'age des 18-23 ans car un bon nombre est déjà sur le marcher de l'emploie et donc non sensible à cette mesure
    [Tooltip("Gives the base impact on the R0 of school closures. This data is moderate depending on the age of the students. If we close Universities, for example, we do not touch the entire age group of 18-23 years old because a good number are already in the employment market and therefore not sensitive to this measure.")]
    public float schoolImpact = 0.25f;

    // Donne l'impact de base sur la contagiosité de l'appel civique à respecter les gestes barrières. 
    [Tooltip("Gives the basic impact on the R0 of the civic call to respect barrier gestures")]
    public float civicismImpact = 0.15f;

    // Donne l'impact de base sur la contagiosité de la fermeture des commerces (par défaut réduit la contagiosité de 10%). 
    [Tooltip("Gives the base impact on the R0 of the closing of businesses")]
    public float shopImpact = 0.2f;

    // Donne l'impact de base sur la contagiosité de l'attestation (par défaut réduit la contagiosité de 15%). 
    [Tooltip("Gives the basic impact on the R0 of the certificate")]
    public float attestationImpact = 0.25f;

    // Donne l'impact de base sur la contagiosité de la restriction sur l'age (par défaut réduit la contagiosité de 70%). 
    [Tooltip("Gives the basic impact on the R0 of the restriction on the age")]
    public float ageRestrictionImpact = 0.7f;

    // Donne l'impact de base sur la contagiosité d'inciter au télétravail
    [Tooltip("Gives the basic impact on the R0 of encouraging teleworking")]
    public float remoteWorkingImpact = 0.13f;

    // Donne l'impact de base sur la contagiosité de promouvoir les masques artisanaux
    [Tooltip("Gives the basic impact on the R0 to promote handmade masks")]
    public float selfProtectionImpact = 0.12f;
}