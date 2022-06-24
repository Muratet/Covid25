using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This component is used to store territory data
/// </summary>
public class TerritoryData : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// Territory name
    /// </summary>
    [Tooltip("Territory name")]
    public string TerritoryName;

    /// <summary>
    /// Territory id
    /// </summary>
    public int id;

    /// <summary>
    /// Proportion of the population of this region in relation to the total population of the country
    /// </summary>
    [Tooltip("Proportion of the population of this region in relation to the total population of the country")]
    public float populationRatio;

    // INSEE data for the french population
    // public int nbPopulation = 67063703;
    // public int[] popNumber = new int[101] { 706382, 716159, 729139, 749142, 770897, 795049, 801336, 818973, 824266, 844412, 836610, 841774, 833484, 847250, 828874, 828224, 825535, 824243, 830859, 832135, 778595, 767419, 738255, 741493, 731720, 709814, 710229, 747365, 762740, 783278, 793756, 805709, 809462, 824388, 823154, 817616, 809113, 860183, 868514, 876362, 830619, 812560, 815529, 795012, 818506, 859407, 905508, 925828, 921091, 900389, 888940, 878137, 872944, 891913, 893796, 901416, 889289, 857860, 858184, 852627, 845836, 827046, 818270, 809103, 799407, 795066, 776073, 784280, 760998, 783527, 766434, 759622, 739203, 692884, 518955, 502516, 483835, 443448, 389310, 397453, 408011, 390052, 372609, 362050, 336284, 325338, 293641, 280250, 250255, 226053, 186015, 160562, 132403, 110466, 89330, 69801, 53201, 39728, 29030, 20035, 21860 };

    /// <summary>
    /// Quantity of the population of this region
    /// </summary>
    [Tooltip("Quantity of the population of this region")]
    public int nbPopulation = 0;
    /// <summary>
    /// Distribution of the population by age
    /// </summary>
    [Tooltip("Distribution of the population by age")]
    public int[] popNumber = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    /// <summary>
    /// Number of people in the most populated age group
    /// </summary>
    [HideInInspector]
    public int maxNumber = 0;

    /// <summary>
    /// Quantity of the infected population in this region
    /// </summary>
    [Tooltip("Quantity of the infected population in this region")]
    public int nbInfected = 0;
    /// <summary>
    /// Distribution of the infected population by age
    /// </summary>
    [Tooltip("Distribution of the infected population by age")]
    public int[] popInfected = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    /// <summary>
    /// Amount of population cared for in this region
    /// </summary>
    [Tooltip("Amount of population cared for in this region")]
    public int nbTreated = 0;
    /// <summary>
    /// Distribution of the population treated by age
    /// </summary>
    [Tooltip("Distribution of the population treated by age")]
    public int[] popTreated = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    // Accumulation de mort pour chaque tranche d'age
    /// <summary>
    /// Quantity of the deceased population in this region
    /// </summary>
    [Tooltip("Quantity of the deceased population in this region")]
    public int nbDeath = 0;
    /// <summary>
    /// Distribution of the deceased population by age
    /// </summary>
    [Tooltip("Distribution of the deceased population by age")]
    public int[] popDeath = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    /// <summary>
    /// In case of non integer dead, keep floating part
    /// </summary>
    [HideInInspector]
    public float[] popPartialDeath = new float[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    /// <summary>
    /// model number of infected people per age during X days
    /// </summary>
    [HideInInspector]
    public int[][] numberOfInfectedPeoplePerAgesAndDays;

    /// <summary>
    /// Window over X days of the number of infected persons for each age group
    /// </summary>
    [HideInInspector]
    public int[] numberOfInfectedPeoplePerDays;

    /// <summary></summary>
    [HideInInspector]
    public bool closePrimarySchool;
    /// <summary></summary>
    [HideInInspector]
    public int closePrimarySchoolLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool closeSecondarySchool;
    /// <summary></summary>
    [HideInInspector]
    public int closeSecondarySchoolLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool closeHighSchool;
    /// <summary></summary>
    [HideInInspector]
    public int closeHighSchoolLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool closeUniversity;
    /// <summary></summary>
    [HideInInspector]
    public int closeUniversityLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool callCivicism;
    /// <summary></summary>
    [HideInInspector]
    public int callCivicismLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool closeShop;
    /// <summary></summary>
    [HideInInspector]
    public int closeShopLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public float closeShopDynamic = 0f;
    /// <summary></summary>
    [HideInInspector]
    public bool certificateRequired;
    /// <summary></summary>
    [HideInInspector]
    public int certificateRequiredLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public bool ageDependent;
    /// <summary></summary>
    [HideInInspector]
    public int ageDependentLastUpdate = -1;
    /// <summary></summary>
    [HideInInspector]
    public string ageDependentMin;
    /// <summary></summary>
    [HideInInspector]
    public string ageDependentMax;

    /// <summary>
    /// The history of cumulative death
    /// </summary>
    public List<int> cumulativeDeath = new List<int>();
    /// <summary>
    /// The history of daily death
    /// </summary>
    public List<int> historyDeath = new List<int>();

}