using UnityEngine;
using FYFY;
using System;

public class DaySystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));

    public TimeScale time;
    public Localization localization;

    public static DaySystem instance;

    private DateTime date;

    public DaySystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        date = new DateTime(2025, 1, 1);
    }

    public void setPause (bool newState)
    {
        this.Pause = newState;
        time.newDay = false;
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        time.newDay = false;
        time.timeElapsed += Time.deltaTime;
        // Calcul d'une nouvelle journée si le pas de simulation est dépassé
        if (time.timeElapsed > time.dayVelocity)
        {
            time.timeElapsed = time.timeElapsed - time.dayVelocity;

            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                //////////////////////////////////////
                // GLISSEMENT JOURNALIER
                //////////////////////////////////////
                // On passe donc au jour suivant
                for (int day = territoryData.numberOfInfectedPeoplePerDays.Length - 1; day > 0; day--) // Attention s'arrêter sur 1 pour pouvoir aller chercher le 0
                    territoryData.numberOfInfectedPeoplePerDays[day] = territoryData.numberOfInfectedPeoplePerDays[day - 1];
                territoryData.numberOfInfectedPeoplePerDays[0] = 0;
                // Y compris pour chaque tranche d'age
                for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                {
                    // Prise en compte du nombre de personnes guéries
                    int treated = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1];
                    territoryData.popTreated[age] += treated;
                    territoryData.nbTreated += treated;
                    // maintenant on peu passer au jour suivant
                    for (int day = territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1; day > 0; day--) // Attention s'arrêter sur 1 pour pouvoir aller chercher le 0
                        territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day - 1];
                    territoryData.numberOfInfectedPeoplePerAgesAndDays[age][0] = 0;
                }
            }

            time.newDay = true;
            time.daysGone++;
            date = date.AddDays(1);
        }
    }

    public void UpdateTimeUI(TMPro.TMP_Text textUI)
    {
        textUI.text = localization.getFormatedText(localization.date, date.ToString("d", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo), time.daysGone);
    }
}