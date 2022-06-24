using UnityEngine;
using FYFY;
using TMPro;

/// <summary>
/// This system is in charge to manage ICU bed occupancy
/// </summary>
public class BedsSystem : FSystem
{
    private Family f_beds = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds)));

    /// <summary></summary>
    public GameObject countrySimData;
    private TimeScale time;
    private VirusStats virusStats;

    /// <summary></summary>
    public Localization localization;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static BedsSystem instance;

    /// <summary>
    /// Construct this system
    /// </summary>
    public BedsSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery of virus data
        virusStats = countrySimData.GetComponent<VirusStats>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay)
        {
            foreach (GameObject bedsGO in f_beds)
            {
                Beds beds = bedsGO.GetComponent<Beds>();
                TerritoryData territory = bedsGO.GetComponent<TerritoryData>();

                if (beds.boostBeds)
                {
                    int newBeds = (beds.intensiveBeds_high - beds.intensiveBeds_current) / 8; // The 8 allows to slow down the growth of the production (the time to build/buy respirators, to reorganize the services, to requisition the hospitals and private clinics...)
                    beds.intensiveBeds_current += newBeds;
                }

                // calculation of the number of used beds: counting of the number of people who should be able to access ICU beds, we take a window of 9 days around the peak of mortality (same calculation done in DeadSystem)
                int criticAmount = 0;
                for (int day = Mathf.Max(0, (int)virusStats.deadlinessPeak - 4); day < Mathf.Min(virusStats.deadlinessPeak + 5, territory.numberOfInfectedPeoplePerDays.Length); day++)
                    criticAmount = (int)(territory.numberOfInfectedPeoplePerDays[day] * virusStats.seriousRatio);

                beds.intensiveBeds_need = criticAmount;

                if (beds.intensiveBeds_need > beds.intensiveBeds_current && beds.advisorNotification == -1 && territory.TerritoryName != countrySimData.GetComponent<TerritoryData>().TerritoryName)
                {
                    GameObjectManager.addComponent<ChatMessage>(beds.gameObject, new { sender = localization.advisorTitleHospital, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorHospitalTexts[0], territory.TerritoryName) });
                    beds.advisorNotification = 0;
                }
                else if (beds.intensiveBeds_need < beds.intensiveBeds_current && beds.advisorNotification > 10)
                    beds.advisorNotification = -1;
                else if (beds.advisorNotification != -1)
                    beds.advisorNotification++;

            }
        }
	}

    /// <summary>
    /// Update UI text of ICU bed occupancy
    /// </summary>
    /// <param name="textUI"></param>
    public void UpdateBedsUI(TMP_Text textUI)
    {
        Beds beds = MapSystem.territorySelected.GetComponent<Beds>();

        // Updated text and color of bed use
        textUI.text = beds.intensiveBeds_need.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo) + " / " + beds.intensiveBeds_current.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
        textUI.color = new Color(2 * (float)beds.intensiveBeds_need / beds.intensiveBeds_current, 2 * (1f - (float)beds.intensiveBeds_need / beds.intensiveBeds_current), 0f);
    }
}