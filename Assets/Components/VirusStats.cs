using UnityEngine;

public class VirusStats : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    // Viser en moyenne 3 infections par personne sur les X jours de contagiosité. Donc pour un cluster de 10 personnes sur les 10 jours il faut environ contaminer 30 autres personnes.
    [Tooltip("Modélise le R0 courrant du virus")]
    public float contagiosity = 3f;
    // Taux de population infectée à partir duquel la pandémie se résorbe par elle même sans mesure particulière
    [Tooltip("Taux de population infectée à partir duquel la pandémie se résorbe par elle même sans mesure particulière")]
    public float populationRatioImmunity = 0.6f;

    // Taille de la fenêtre glissante pour stocker le nombre de personnes infectées
    [Tooltip("Taille de la fenêtre glissante pour stocker le nombre de personnes infectées")]
    public int windowSize = 21;
    // Données pour calculer la gaussienne sur la taille de la fenêtre glissante
    // ... pour la contagiosité
    [Tooltip("Moyenne du pic d'apparition des symptomes")]
    public float contagiousnessPeak = 10; // Moyenne du pic d'apparition des symptomes
    [Tooltip("Ecart type d'apparition des symptomes")]
    public float contagiousnessDeviation = 3f;
    // ... pour la létalité
    [Tooltip("Moyenne du pic de mortalité")]
    public float deadlinessPeak = 12; // Décallé de 2 jours après la moyenne de l'apparition des premiers symptomes...
    [Tooltip("Ecart type de mortalité")]
    public float deadlinessDeviation = 1.8f;

    // Indique l'age à partir duquel on peut commencer à avoir des morts
    [Tooltip("Age à partir duquel on peut commencer à avoir des morts")]
    public int firstSensitiveAge = 30;
    // Taux de mortalité maximum pour la tranche d'age la plus agée
    [Tooltip("Taux de mortalité maximum pour la tranche d'age la plus agée")]
    public float maxDeadlinessRatio = 0.148f;
    // Permet de contrôler la courbure de l'exponentielle
    // 1 => exponentielle classique
    // < 1 => accelération plus rapide
    // > 1 => accelération plus lente
    [Tooltip("Contrôle de la courbure de l'exponentielle, 1 => exponentielle classique, < 1 => accelération plus rapide, > 1 => accelération plus lente")]
    public int curveStrenght = 12;

    [Tooltip("Taux de cas sérieux")]
    public float seriousRatio = 0.138f; // 13.8% des car sont sérieux
    [Tooltip("Taux de cas critiques")]
    public float criticRatio = 0.047f; // 4.7% des cas sont critiques et requiert une hospitatlisation

    [Tooltip("Nombre de mois requis pour découvrir le vaccin")]
    public int vaccineMounthDelay = 18; // 18 mois
}