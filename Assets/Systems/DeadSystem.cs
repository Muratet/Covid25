using UnityEngine;
using UnityEngine.UI;
using FYFY;

/// <summary>
/// This system calculates the number of deaths each day
/// </summary>
public class DeadSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    public GameObject countrySimData;
    private VirusStats virusStats;
    private TerritoryData countryPopData;
    private TimeScale time;

    // model deadliness during X days
    private float[] deadlinessPerDays;

    // give deadliness probability for each age
    private float[] deadlinessPerAges;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static DeadSystem instance;

    private bool firstDead = false;
    private int nextDeathNotification = 100;

    /// <summary></summary>
    public Localization localization;

    /// <summary>
    /// Construct this system
    /// </summary>
    public DeadSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Recovery virus data
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();

        // calculation of the mortality curve for a window of days
        deadlinessPerDays = new float[virusStats.windowSize];
        float peak = virusStats.deadlinessPeak;
        float deviation = virusStats.deadlinessDeviation;
        for (int i = 0; i < deadlinessPerDays.Length; i++)
            deadlinessPerDays[i] = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation));

        // Calculation of mortality according to age
        deadlinessPerAges = new float[101];
        // Calculation of the value of the exponential for the first age from which deaths can occur
        float minAgeExpo = Mathf.Exp(virusStats.curveStrenght * ((float)virusStats.firstSensitiveAge / 100 - 1));
        // Calculation of the maximum value of the exponential for the oldest age
        float maxExpo = 1 - minAgeExpo;
        // smoothing of mortality so that it is at 0 at the first sensitive age and at its maximum value for the oldest age
        for (int age = 0; age < deadlinessPerAges.Length; age++)
            deadlinessPerAges[age] = Mathf.Max(0f, (Mathf.Exp(virusStats.curveStrenght * ((float)age / 100 - 1)) - minAgeExpo) / maxExpo);
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        // Check if a new day should be generated
        if (time.newDay)
        {
            // Treating each territory
            TerritoryData territoryData;
            foreach (GameObject territory in f_territories)
            {
                territoryData = territory.GetComponent<TerritoryData>();

                // Counting the number of people who should be able to access ICU beds, we take a window of 9 days around the peak of mortality (same calculation done in BedsSystem)
                int criticAmount = 0;
                for (int day = Mathf.Max(0, (int)virusStats.deadlinessPeak - 4); day < Mathf.Min(virusStats.deadlinessPeak + 5, territoryData.numberOfInfectedPeoplePerDays.Length); day++)
                    criticAmount = (int)(territoryData.numberOfInfectedPeoplePerDays[day] * virusStats.seriousRatio);
                // Calculation of the penalty due to bed occupancy
                // The maximum between 1 and log allows to start adding malus (more mortality) as soon as the number of resuscitation beds starts to be exceeded
                float bedMalus = Mathf.Max(1f, Mathf.Log10((float)criticAmount / territory.GetComponent<Beds>().intensiveBeds_current) + 1);

                //////////////////////////////////////
                // MORTALITY
                //////////////////////////////////////
                for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                {
                    for (int day = territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1; day >= 0; day--) // reverse the days to treat those who have been sick the longest first
                    {
                        // recover how many people are infected for this age group and for the current day
                        int infectedNumber = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day];
                        // Calculate the number of deaths according to the number of infected, the age mortality, the risk of day mortality and the possible lack of ICU beds
                        // Default rates for the number of available ICU beds are applied
                        float nbDead_float = infectedNumber * deadlinessPerAges[age] * deadlinessPerDays[day] * bedMalus;
                        int nbDead = Mathf.RoundToInt(nbDead_float);
                        // recovery and accumulation of the floating part
                        territoryData.popPartialDeath[age] += nbDead_float - (int)nbDead_float;
                        // addition of an extra dead person if the previous accumulations exceed the unit
                        int cumulatedDeath = (int)territoryData.popPartialDeath[age];
                        // taking into account the possible additional death
                        nbDead += cumulatedDeath;
                        // remove from the accumulation the possible additional death
                        territoryData.popPartialDeath[age] -= cumulatedDeath;

                        if (nbDead > 0)
                        {
                            // remove these deaths from the territory
                            territoryData.nbDeath += nbDead;
                            territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= nbDead;
                            territoryData.popDeath[age] += nbDead;
                            territoryData.numberOfInfectedPeoplePerDays[day] -= nbDead;
                            // remove these deaths at the national level
                            countryPopData.nbDeath += nbDead;
                            countryPopData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= nbDead;
                            countryPopData.popDeath[age] += nbDead;
                            countryPopData.numberOfInfectedPeoplePerDays[day] -= nbDead;
                        }
                    }
                }
                // Recording for the history of new deaths for the region
                territoryData.cumulativeDeath.Add(territoryData.nbDeath);
                if (territoryData.cumulativeDeath.Count > 1)
                    territoryData.historyDeath.Add(territoryData.nbDeath - territoryData.cumulativeDeath[territoryData.cumulativeDeath.Count - 2]);
                else
                    territoryData.historyDeath.Add(territoryData.nbDeath);
            }
            // Recording for the history of new deaths for the nation
            countryPopData.cumulativeDeath.Add(countryPopData.nbDeath);
            if (countryPopData.cumulativeDeath.Count > 1)
                countryPopData.historyDeath.Add(countryPopData.nbDeath - countryPopData.cumulativeDeath[countryPopData.cumulativeDeath.Count - 2]);
            else
                countryPopData.historyDeath.Add(countryPopData.nbDeath);
            if (countryPopData.historyDeath[countryPopData.historyDeath.Count - 1] >= 1 && !firstDead)
            {
                GameObjectManager.addComponent<ChatMessage>(countryPopData.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = localization.advisorHealthTexts[1] });
                firstDead = true;
            }
            if (countryPopData.historyDeath[countryPopData.historyDeath.Count - 1] > nextDeathNotification)
            {
                string msgBody = localization.getFormatedText(localization.advisorHealthTexts[2], nextDeathNotification.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo));
                if (nextDeathNotification > 200000)
                    msgBody += localization.advisorHealthTexts[3];
                else if (nextDeathNotification > 100000)
                    msgBody += localization.advisorHealthTexts[4];
                else if (nextDeathNotification > 50000)
                    msgBody += localization.advisorHealthTexts[5];
                else if (nextDeathNotification > 24000)
                    msgBody += localization.advisorHealthTexts[6];
                else if (nextDeathNotification > 12000)
                    msgBody += localization.advisorHealthTexts[7];
                else if (nextDeathNotification > 6000)
                    msgBody += localization.advisorHealthTexts[8];
                else if (nextDeathNotification > 3000)
                    msgBody += localization.advisorHealthTexts[9];
                else if (nextDeathNotification > 1500)
                    msgBody += localization.advisorHealthTexts[10];
                else if (nextDeathNotification > 750)
                    msgBody += localization.advisorHealthTexts[11];
                else if (nextDeathNotification > 325)
                    msgBody += localization.advisorHealthTexts[12];

                GameObjectManager.addComponent<ChatMessage>(countryPopData.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = msgBody });
                nextDeathNotification *= 2;
            }
        }
    }

    /// <summary>
    /// update of the number of deaths in the UI
    /// </summary>
    /// <param name="textUI">New value</param>
    public void UpdateDailyDeadUI (TMPro.TMP_Text textUI)
    {
        int dailyDead = 0;
        if (MapSystem.territorySelected.historyDeath.Count > 1)
            dailyDead = MapSystem.territorySelected.historyDeath[MapSystem.territorySelected.historyDeath.Count - 1];
        SyncUISystem.formatStringUI(textUI, dailyDead, localization);
    }

    /// <summary>
    /// update of the number of cumulative deaths in the UI
    /// </summary>
    /// <param name="textUI">New value</param>
    public void UpdateCumulDeadUI(TMPro.TMP_Text textUI)
    {
        int cumulDead = 0;
        if (MapSystem.territorySelected.cumulativeDeath.Count > 1)
            cumulDead = MapSystem.territorySelected.cumulativeDeath[MapSystem.territorySelected.cumulativeDeath.Count - 1];
        SyncUISystem.formatStringUI(textUI, cumulDead, localization);
    }
}