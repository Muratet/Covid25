using UnityEngine;
using FYFY;
using System;

/// <summary>
/// This system is in charge to control the game speed
/// </summary>
public class DaySystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));

    /// <summary></summary>
    public TimeScale time;
    /// <summary></summary>
    public Localization localization;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static DaySystem instance;

    private DateTime date;

    /// <summary>
    /// Construct this system
    /// </summary>
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
        // Calculation of a new day if the simulation step is exceeded
        if (time.timeElapsed > time.dayVelocity)
        {
            time.timeElapsed = time.timeElapsed - time.dayVelocity;

            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                //////////////////////////////////////
                // DAILY SLIDING
                //////////////////////////////////////
                // So we move on to the next day
                for (int day = territoryData.numberOfInfectedPeoplePerDays.Length - 1; day > 0; day--) // Be careful to stop on 1 to get the 0
                    territoryData.numberOfInfectedPeoplePerDays[day] = territoryData.numberOfInfectedPeoplePerDays[day - 1];
                territoryData.numberOfInfectedPeoplePerDays[0] = 0;
                // Including for each age group
                for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                {
                    // Consideration of the number of people cured
                    int treated = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1];
                    territoryData.popTreated[age] += treated;
                    territoryData.nbTreated += treated;
                    // now we can move on to the next day
                    for (int day = territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1; day > 0; day--) // Be careful to stop on 1 to get the 0
                        territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day - 1];
                    territoryData.numberOfInfectedPeoplePerAgesAndDays[age][0] = 0;
                }
            }

            time.newDay = true;
            time.daysGone++;
            date = date.AddDays(1);
        }
    }

    /// <summary>
    /// Callback to update the time in UI
    /// </summary>
    /// <param name="textUI"></param>
    public void UpdateTimeUI(TMPro.TMP_Text textUI)
    {
        textUI.text = localization.getFormatedText(localization.date, date.ToString("d", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo), time.daysGone);
    }
}