using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TerritoryData : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    // Nom de la région
    [Tooltip("Nom du territoire")]
    public string TerritoryName;

    // Proportion de la population de cette région par rapport à la population totale du pays
    [Tooltip("Proportion de la population de cette région par rapport à la population totale du pays")]
    public float populationRatio;

    // donnée INSEE de la population française
    // public int nbPopulation = 67063703;
    // public int[] popNumber = new int[101] { 706382, 716159, 729139, 749142, 770897, 795049, 801336, 818973, 824266, 844412, 836610, 841774, 833484, 847250, 828874, 828224, 825535, 824243, 830859, 832135, 778595, 767419, 738255, 741493, 731720, 709814, 710229, 747365, 762740, 783278, 793756, 805709, 809462, 824388, 823154, 817616, 809113, 860183, 868514, 876362, 830619, 812560, 815529, 795012, 818506, 859407, 905508, 925828, 921091, 900389, 888940, 878137, 872944, 891913, 893796, 901416, 889289, 857860, 858184, 852627, 845836, 827046, 818270, 809103, 799407, 795066, 776073, 784280, 760998, 783527, 766434, 759622, 739203, 692884, 518955, 502516, 483835, 443448, 389310, 397453, 408011, 390052, 372609, 362050, 336284, 325338, 293641, 280250, 250255, 226053, 186015, 160562, 132403, 110466, 89330, 69801, 53201, 39728, 29030, 20035, 21860 };

    // Estimation de la population de cette région
    [Tooltip("Quantité de la population de cette région")]
    public int nbPopulation = 0;
    [Tooltip("Répartition de la population par age")]
    public int[] popNumber = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    [HideInInspector]
    public int maxNumber = 0;
    // Accumulation pour chaque tranche d'age de nombre de personnes infectées
    [Tooltip("Quantité de la population infectée dans cette région")]
    public int nbInfected = 0;
    [Tooltip("Répartition de la population infectée par age")]
    public int[] popInfected = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    // Accumulation des soigés pour chaque tranche d'age
    [Tooltip("Quantité de la population traitée dans cette région")]
    public int nbTreated = 0;
    [Tooltip("Répartition de la population traitée par age")]
    public int[] popTreated = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    // Accumulation de mort pour chaque tranche d'age
    [Tooltip("Quantité de la population décédée dans cette région")]
    public int nbDeath = 0;
    [Tooltip("Répartition de la population décédée par age")]
    public int[] popDeath = new int[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    [HideInInspector]
    public float[] popPartialDeath = new float[101] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    // model number of infected people per age during X days
    [HideInInspector]
    public int[][] numberOfInfectedPeoplePerAgesAndDays;

    // Fenêtre sur X jours du nombre de personnes infectées pour chaque tranche d'age
    [HideInInspector]
    public int[] numberOfInfectedPeoplePerDays;

    public Toggle closePrimarySchool_UIMaps;
    [HideInInspector]
    public bool closePrimarySchool;
    [HideInInspector]
    public int closePrimarySchoolLastUpdate = -1;
    public Toggle closeSecondarySchool_UIMaps;
    [HideInInspector]
    public bool closeSecondarySchool;
    [HideInInspector]
    public int closeSecondarySchoolLastUpdate = -1;
    public Toggle closeHighSchool_UIMaps;
    [HideInInspector]
    public bool closeHighSchool;
    [HideInInspector]
    public int closeHighSchoolLastUpdate = -1;
    public Toggle closeUniversity_UIMaps;
    [HideInInspector]
    public bool closeUniversity;
    [HideInInspector]
    public int closeUniversityLastUpdate = -1;
    public Toggle callCivicism_UIMaps;
    [HideInInspector]
    public bool callCivicism;
    [HideInInspector]
    public int callCivicismLastUpdate = -1;
    public Toggle closeShop_UIMaps;
    [HideInInspector]
    public bool closeShop;
    [HideInInspector]
    public int closeShopLastUpdate = -1;
    [HideInInspector]
    public float closeShopDynamic = 0f;
    public Toggle certificateRequired_UIMaps;
    [HideInInspector]
    public bool certificateRequired;
    [HideInInspector]
    public int certificateRequiredLastUpdate = -1;
    public Toggle ageDependent_UIMaps;
    [HideInInspector]
    public bool ageDependent;
    [HideInInspector]
    public int ageDependentLastUpdate = -1;
    public TMP_InputField ageDependentMin_UIMaps;
    [HideInInspector]
    public string ageDependentMin;
    public TMP_InputField ageDependentMax_UIMaps;
    [HideInInspector]
    public string ageDependentMax;

    public List<int> cumulativeDeath = new List<int>();
    public List<int> historyDeath = new List<int>();

}