using UnityEngine;
using UnityEngine.UI;
using FYFY;

/// <summary>
/// This system calculates the debt each day
/// </summary>
public class FinanceSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    /// <summary></summary>
    public GameObject countrySimData;
    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Finances finances;
    private TerritoryData countryPopData;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Tax tax;
    private Beds beds;

    /// <summary></summary>
    public Localization localization;

    private float taxProgress = 0f;

    private float nextStepNotif = 10000000f;
    private bool stability = true;
    private bool bedsNotif = false;

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Recovery borders permeability
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Recovery of financial data
        finances = countrySimData.GetComponent<Finances>();
        // Recovery of homeworking data
        remoteworking = countrySimData.GetComponent<Remoteworking>();
        // Recovery of partial unemployment data
        shortTimeWorking = countrySimData.GetComponent<ShortTimeWorking>();
        // Recovery of business charges
        tax = countrySimData.GetComponent<Tax>();
        // Recovery of ICU beds
        beds = countrySimData.GetComponent<Beds>();
        finances.historySpent.Add(0);
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Check if a new day should be generated
        if (time.newDay)
        {
            float newSpent = 0;

            // Border closure management
            if (frontierPermeability.currentState >= 1)
                newSpent += Random.Range(finances.costLostTourismPerDay*0.7f, finances.costLostTourismPerDay*1.3f);
            if (frontierPermeability.currentState >= 2)
                newSpent += Random.Range(finances.costLostFreightPerDay*0.7f, finances.costLostFreightPerDay*1.3f);

            float remoteworkingImpact = (remoteworking.currentState ? 0.75f : 1f); // if home working is enabled, the financial impact of working from home is reduced
            float shortTimeWorkingImpact = (shortTimeWorking.currentState ? 100f : 1f); // if partial unemployment is activated, the financial impact due to social charges explodes

            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                // French INSEE figures : 30 Billions in 2 months for a scenario with homeworking (0.75f) + partial unemployment (10f) => so per day it gives : 30 Billions / 2 months / 30 days / (100 * 0.75) = 6 666 666 € => rounded to 6 000 000 € and to be weighted according to the proportion of the population in the region

                // Management of the closing of the stores
                if (territoryData.closeShop)
                {
                    territoryData.closeShopDynamic = Mathf.Min(1, territoryData.closeShopDynamic + 0.1f);
                    newSpent += territoryData.closeShopDynamic * territoryData.populationRatio * Random.Range(4000000f, 8000000f) * remoteworkingImpact * shortTimeWorkingImpact; // 6 Million loss per day due to economic downturn modulated by social actions. 
                } else
                    territoryData.closeShopDynamic = Mathf.Max(0, territoryData.closeShopDynamic - 0.1f);

                // Containment management by age (if the age range concerns workers)
                if (territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && int.Parse(territoryData.ageDependentMin) < 62 && int.Parse(territoryData.ageDependentMax) >= 62)
                {
                    int ageMin = int.Parse(territoryData.ageDependentMin);
                    int ageMax = int.Parse(territoryData.ageDependentMax);
                    // A part of the population in total confinement is of working age => Calculation of the proportion of the population of the region concerned
                    int workerConfined = 0;
                    for (int age = ageMin; age <= ageMax && age < 62; age++)
                        workerConfined += territoryData.popNumber[age] - territoryData.popDeath[age];
                    newSpent += workerConfined / (countryPopData.nbPopulation - countryPopData.nbDeath) * Random.Range(500000f, 1500000f) * remoteworkingImpact * shortTimeWorkingImpact; // 1 Million loss per day due to economic downturn modulated by social actions
                }
            }

            // Corporate tax management
            if (tax.currentState)
            {
                taxProgress = Mathf.Min(1, taxProgress + 0.1f);
                newSpent += taxProgress * Random.Range(tax.compensateTaxesCanceled*0.7f, tax.compensateTaxesCanceled*1.3f); 
            } else
                taxProgress = Mathf.Max(0, taxProgress - 0.1f);

            // Calculation of the financial cost of a day in a ICU bed
            newSpent += Mathf.Min(beds.intensiveBeds_need, beds.intensiveBeds_current) * finances.oneDayReanimationCost;
            if (!bedsNotif && beds.intensiveBeds_need > 0)
            {
                bedsNotif = true;
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = localization.advisorTitleHospital, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorHospitalTexts[1], finances.oneDayReanimationCost.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo)+finances.money) });
            }


            // taking into account the external financing of this system (purchase of masks and vaccines in this case)
            newSpent += finances.dailySpending;
            finances.dailySpending = 0;

            if (newSpent != 0)
                stability = false;

            float newDebt = finances.historySpent[finances.historySpent.Count - 1] + newSpent;
            if (newDebt > nextStepNotif)
            {
                string messageChosen = "";
                switch(Random.Range(0, 4))
                {
                    case 0: messageChosen = localization.advisorEconomyTexts[0]; break;
                    case 1: messageChosen = localization.getFormatedText(localization.advisorEconomyTexts[1], newDebt.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo)+finances.money); break;
                    case 2: messageChosen = localization.advisorEconomyTexts[2]; break;
                    case 3: messageChosen = localization.advisorEconomyTexts[3]; break;
                }
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = localization.advisorTitleEconomy, timeStamp = "" + time.daysGone, messageBody = messageChosen });
                nextStepNotif *= 10;
            }

            finances.historySpent.Add(newDebt);

            if (!stability && finances.historySpent[finances.historySpent.Count - 1] == finances.historySpent[Mathf.Max(0, finances.historySpent.Count - 10)])
            {
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = localization.advisorTitleEconomy, timeStamp = "" + time.daysGone, messageBody = localization.advisorEconomyTexts[4] });
                stability = true;
            }
        }
    }

    /// <summary>
    /// update of the debt in the UI
    /// </summary>
    /// <param name="textUI">New value</param>
    public void UpdateFinanceUI(TMPro.TMP_Text textUI)
    {
        float financeAmount = 0;
        if (finances.historySpent.Count > 1)
            financeAmount = finances.historySpent[finances.historySpent.Count - 1];
        SyncUISystem.formatStringUI(textUI, financeAmount, localization);
    }
}