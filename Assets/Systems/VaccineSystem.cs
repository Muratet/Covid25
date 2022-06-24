using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Globalization;
using System.Collections.Generic;

/// <summary>
/// This system manages the vaccination
/// </summary>
public class VaccineSystem : FSystem {
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    /// <summary></summary>
    public GameObject countrySimData;
    private VirusStats virusStats;
    private TimeScale time;
    private Vaccine vaccine;
    private Finances finances;
    private FrontierPermeability frontierPermeability;
    private Revolution revolution;
    private TerritoryData countryPopData;

    /// <summary></summary>
    public Localization localization;

    private int notifVaccine = 0;
    private bool firstVaccineDelivered = false;
    private int lastVaccineDelivery = -1;

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery of virus data
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Recovery of vaccine data
        vaccine = countrySimData.GetComponent<Vaccine>();
        // Recovery of financial data
        finances = countrySimData.GetComponent<Finances>();
        // Recovery borders permeability
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Recovery of dissatisfaction data
        revolution = countrySimData.GetComponent<Revolution>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay)
        {
            // Progress research
            if (vaccine.UI_researchBar.value < 100)
            {
                string continent = localization.continents[Random.Range(0, 5)];

                vaccine.UI_researchBar.value = 100 * Mathf.Min(1f, (float)time.daysGone / (virusStats.vaccineMounthDelay * 30f));
                if (vaccine.UI_researchBar.value > 1 && notifVaccine == 0)
                {
                    notifVaccine++;
                    GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleForeignAffairs, timeStamp = "" + time.daysGone, messageBody = localization.advisorForeignAffairsTexts[0] });
                }
                if (vaccine.UI_researchBar.value > 14 && notifVaccine == 1)
                {
                    notifVaccine++;
                    GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleForeignAffairs, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorForeignAffairsTexts[1], continent) });
                }
                if (vaccine.UI_researchBar.value > 28 && notifVaccine == 2)
                {
                    notifVaccine++;
                    GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleForeignAffairs, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorForeignAffairsTexts[2], continent) });
                }
                if (vaccine.UI_researchBar.value > 64 && notifVaccine == 3)
                {
                    notifVaccine++;
                    GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleForeignAffairs, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorForeignAffairsTexts[3], continent) });
                }
            }
            else if (notifVaccine == 4)
            {
                notifVaccine++;
                vaccine.UI_vaccineQuantity.GetComponents<TooltipContent>()[0].enabled = false;
                vaccine.UI_vaccineCommand.GetComponents<TooltipContent>()[0].enabled = false;
                // Call the function that defines if the fields are interactive or not
                _OnFrontierChange(frontierPermeability.currentState);
                GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleEconomy, timeStamp = "" + time.daysGone, messageBody = localization.advisorEconomyTexts[5] });
                // Addition of a one-time bonus on stress
                revolution.stress -= 5f;
            }

            
            if (vaccine.UI_researchBar.value >= 100)
            {
                CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
                // Simulation of the fluctuation of vaccine prices
                int newPrice = vaccine.vaccinePrice + Random.Range(-1, 2) * Random.Range(0, 4);
                vaccine.vaccinePrice = Mathf.Max(vaccine.vaccineMinPrice, Mathf.Min(vaccine.vaccineMaxPrice, newPrice));
                vaccine.UI_vaccineUnitPrice.text = vaccine.vaccinePrice.ToString("C0", cultureInfo); // monetary style

                // Vaccine delivery simulation
                vaccine.nextDelivery--;
                if (vaccine.nextDelivery <= 0 && vaccine.commands > 0 && frontierPermeability.currentState < 2)
                {
                    vaccine.nextDelivery = vaccine.deliveryTime + Random.Range(-2, 3); // random addition in the delivery plus or minus 2 days
                    float incomingVaccines = Mathf.Min(vaccine.meanDeliveryPack * Random.Range(0.5f, 1.5f), vaccine.commands); // Receipt of an order at plus or minus 25% of the reference value
                    vaccine.nationalStock += incomingVaccines;
                    vaccine.commands -= incomingVaccines;
                    // Update UI
                    vaccine.UI_vaccinePendingCommand.text = vaccine.commands.ToString("N0", cultureInfo);
                    if (!firstVaccineDelivered)
                    {
                        firstVaccineDelivered = true;
                        GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorHealthTexts[18], incomingVaccines.ToString("N0", cultureInfo)) });
                    }
                    lastVaccineDelivery = time.daysGone;
                }

                // The bottleneck for immunization is related to the production of the vaccine rather than the ability to immunize many people at once. Therefore, the daily capacity of vaccine over 8 days is smoothed according to the average delivery.
                int newVaccination = (int)Mathf.Min(vaccine.nationalStock, Random.Range(0.75f, 1.25f) * vaccine.meanDeliveryPack / 8);
                vaccine.nationalStock = vaccine.nationalStock - newVaccination;

                List<int> availableAge = new List<int>(); // list of the 20 oldest ages to target for vaccination
                List<int> availableCount = new List<int>(); // population quantity for each target age
                for (int age = 100; age >= 0 && availableAge.Count < 20; age--)
                {
                    int ageAvailableCount = (int)(countryPopData.popNumber[age] * vaccine.vaccineTrust - countryPopData.popInfected[age]);
                    if (ageAvailableCount > 0)
                    {
                        availableAge.Add(age);
                        availableCount.Add(ageAvailableCount);
                    }
                }

                while (newVaccination > 0 && availableAge.Count > 0)
                {
                    int ageId = Random.Range(0, Random.Range(0, availableAge.Count)); // A random age A is drawn from the 20 most at-risk age groups and then a random age is drawn again between A and the most at-risk age to increase the probability of obtaining a more at-risk age
                    int age = availableAge[ageId];
                    TerritoryData territory;
                    do territory = f_territories.getAt(Random.Range(0, f_territories.Count)).GetComponent<TerritoryData>();
                    while ((float)territory.popInfected[age] / territory.popNumber[age] >= vaccine.vaccineTrust);
                    int localVaccination = Mathf.Max(1, Mathf.Min(newVaccination, availableCount[ageId], (int)(territory.popNumber[age] * vaccine.vaccineTrust - territory.popInfected[age]), 100));
                    territory.nbInfected += localVaccination;
                    territory.popInfected[age] += localVaccination;
                    territory.nbTreated += localVaccination;
                    territory.popTreated[age] += localVaccination;
                    countryPopData.nbInfected += localVaccination;
                    countryPopData.popInfected[age] += localVaccination;
                    countryPopData.nbTreated += localVaccination;
                    countryPopData.popTreated[age] += localVaccination;
                    availableCount[ageId] -= localVaccination;
                    // Check that there are still people to vaccinate for that age
                    if (availableCount[ageId] <= 0)
                    {
                        availableAge.RemoveAt(ageId);
                        availableCount.RemoveAt(ageId);
                    }
                    newVaccination -= localVaccination;
                }
            }

            vaccine.historyStock.Add((int)vaccine.nationalStock);

            // check if the stock is at 0 over the last 8 days
            bool emptyStock = true;
            for (int i = vaccine.historyStock.Count - 1; i >= 0 && i >= vaccine.historyStock.Count - 8 && emptyStock; i--)
                emptyStock = vaccine.historyStock[i] == 0;
            // display a notification if the stock is empty when an order has already been received
            if (lastVaccineDelivery != -1 && emptyStock)
            {
                string msgBody = localization.advisorHealthTexts[19];
                if (vaccine.commands == 0)
                {
                    msgBody += localization.advisorHealthTexts[15];
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[16];
                }
                else
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[17];

                GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = msgBody });
                lastVaccineDelivery = -1;
            }
        }
    }

    /// <summary>
    /// Call when a new order is placed
    /// </summary>
    /// <param name="input">The amount of vaccine ordered</param>
    public void NewCommand(TMP_InputField input)
    {
        if (input.text != "")
        {
            if (vaccine.commands == 0)
                vaccine.nextDelivery = vaccine.deliveryTime + Random.Range(-2, 3); // random addition in the delivery plus or minus 2 days
            int newCommand = int.Parse(input.text);
            input.text = "";
            finances.dailySpending += newCommand * vaccine.vaccinePrice;
            vaccine.commands += newCommand;
            vaccine.UI_vaccinePendingCommand.text = vaccine.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
            vaccine.lastOrderPlaced = time.daysGone;
            vaccine.lastOrderAmount = newCommand;
        }
    }

    /// <summary>
    /// Callback when border is toggled
    /// </summary>
    /// <param name="newValue"></param>
    public void OnFrontierChange(ItemSelector newValue)
    {
        _OnFrontierChange(newValue.currentItem);
    }

    /// <summary>
    /// Callback when border is toggled
    /// </summary>
    /// <param name="newValue"></param>
    public void _OnFrontierChange(int newValue)
    {
        vaccine.UI_vaccineQuantity.interactable = (newValue == 0 || newValue == 1) && vaccine.UI_researchBar.value >= 100 && notifVaccine >= 4;
        vaccine.UI_vaccineCommand.interactable = vaccine.UI_vaccineQuantity.interactable;
        vaccine.UI_vaccineQuantity.GetComponents<TooltipContent>()[1].enabled = notifVaccine >= 4 && !vaccine.UI_vaccineQuantity.interactable;
        vaccine.UI_vaccineCommand.GetComponents<TooltipContent>()[1].enabled = notifVaccine >= 4 && !vaccine.UI_vaccineQuantity.interactable;
    }

    /// <summary>
    /// Updates in the IU the amount of vaccine stock
    /// </summary>
    /// <param name="textUI"></param>
    public void UpdateVaccineUI(TMPro.TMP_Text textUI)
    {
        SyncUISystem.formatStringUI(textUI, vaccine.nationalStock, localization);
    }
}