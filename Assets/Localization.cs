using UnityEngine;

public class Localization : MonoBehaviour
{
    public string date;

    public string advisorTitleDigital;
    public string[] advisorDigitalTexts;

    public string advisorTitleHealth;
    public string[] advisorHealthTexts;

    public string advisorTitlePrimeMinister;
    public string[] advisorPrimeMinisterTexts;

    public string advisorTitleHospital;
    public string[] advisorHospitalTexts;

    public string advisorTitleInterior;
    public string[] advisorInteriorTexts;

    public string advisorTitleEconomy;
    public string[] advisorEconomyTexts;

    public string advisorTitleForeignAffairs;
    public string[] advisorForeignAffairsTexts;

    public string[] continents;

    public string[] territoriesTooltip;

    public string[] borderActions;
    public string[] businessActions;
    public string[] homeWorkingActions;
    public string[] partialUnemploymentActions;
    public string[] maskRequisitionActions;
    public string[] maskNationalActions;
    public string[] maskArtisanalActions;
    public string maskOrderActions;
    public string vaccineOrderActions;
    public string opened;
    public string closed;
    public string primarySchoolActions;
    public string middleSchoolActions;
    public string highSchoolActions;
    public string universityActions;
    public string[] civicActions;
    public string[] closeShopActions;
    public string[] certificateActions;
    public string[] boostBedActions;
    public string[] ageDependentActions;
    public string barYears;

    public string maskTooltip;
    public string financeTooltip;
    public string revolutionTooltip;
    public string vaccineTooltip;

    public string unitThousands;
    public string unitMillions;
    public string unitBillions;

    public string unitThousandsShort;
    public string unitMillionsShort;
    public string unitBillionsShort;

    public string getFormatedText(string expression, params object [] data)
    {
        for (int i = 0; i < data.Length; i++)
            expression = expression.Replace("#" + i + "#", data[i].ToString());
        return expression;
    }
}
