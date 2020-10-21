using UnityEngine;

public class InfectionImpact : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    // Donne l'impact de base sur la contagiosité de la fermeture des écoles (par défaut réduit la contagiosité de 20%). Cette donnée est modérée en fontion de l'age des élèves. Si on ferme les Universités par exemple on ne touche pas toute la tranche d'age des 18-23 ans car un bon nombre est déjà sur le marcher de l'emploie et donc non sensible à cette mesure
    [Tooltip("Donne l'impact de base sur le R0 de la fermeture des écoles. Cette donnée est modérée en fontion de l'age des élèves. Si on ferme les Universités par exemple on ne touche pas toute la tranche d'age des 18-23 ans car un bon nombre est déjà sur le marcher de l'emploie et donc non sensible à cette mesure")]
    public float schoolImpact = 0.25f;

    // Donne l'impact de base sur la contagiosité de l'appel civique à respecter les gestes barrières. 
    [Tooltip("Donne l'impact de base sur le R0 de l'appel civique à respecter les gestes barrières")]
    public float civicismImpact = 0.15f;

    // Donne l'impact de base sur la contagiosité de la fermeture des commerces (par défaut réduit la contagiosité de 10%). 
    [Tooltip("Donne l'impact de base sur le R0 de la fermeture des commerces")]
    public float shopImpact = 0.2f;

    // Donne l'impact de base sur la contagiosité de l'attestation (par défaut réduit la contagiosité de 15%). 
    [Tooltip("Donne l'impact de base sur le R0 de l'attestation")]
    public float attestationImpact = 0.25f;

    // Donne l'impact de base sur la contagiosité de la restriction sur l'age (par défaut réduit la contagiosité de 70%). 
    [Tooltip("Donne l'impact de base sur le R0 de la restriction sur l'age")]
    public float ageRestrictionImpact = 0.7f;

    // Donne l'impact de base sur la contagiosité d'inciter au télétravail
    [Tooltip("Donne l'impact de base sur le R0 d'inciter au télétravail")]
    public float remoteWorkingImpact = 0.13f;

    // Donne l'impact de base sur la contagiosité de promouvoir les masques artisanaux
    [Tooltip("Donne l'impact de base sur le R0 de promouvoir les masques artisanaux")]
    public float selfProtectionImpact = 0.12f;
}