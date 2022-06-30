using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;

/// <summary>
/// This system simulated the consumption of masks
/// </summary>
public class MaskSystem : FSystem {
    /// <summary></summary>
    public GameObject countrySimData;
    private TimeScale time;
    private TerritoryData countryPopData;
    private Masks masks;
    private VirusStats virusStats;
    private Revolution revolution;
    private Finances finances;
    private FrontierPermeability frontierPermeability;

    /// <summary></summary>
    public TMP_InputField UI_InputFieldCommand;
    /// <summary></summary>
    public Button UI_ButtonCommand;
    /// <summary></summary>
    public TMP_Text UI_UnitPriceValue;
    /// <summary></summary>
    public TMP_Text UI_PendingCommand;
    private int lastMasksDelivery = 0;

    /// <summary></summary>
    public Localization localization;

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Recovery of virus data
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Recovery masks data
        masks = countrySimData.GetComponent<Masks>();
        // Recovery of dissatisfaction data
        revolution = countrySimData.GetComponent<Revolution>();
        // Recovery of financial data
        finances = countrySimData.GetComponent<Finances>();
        // Recovery of border restrictions data
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        if (time.newDay)
        {
            // Update of the production capacity
            if (masks.boostProduction)
                masks.nationalProductionPerDay_current += (masks.nationalProductionPerDay_high - masks.nationalProductionPerDay_current) / 20; // The 20 allows to slow down the growth of the production (the time to build other lines, to launch startups...)
            else
                masks.nationalProductionPerDay_current -= (masks.nationalProductionPerDay_current - masks.nationalProductionPerDay_low) / 2; // much faster shutdown than production start-up

            // calculation of mask consumption
            // number of people currently infected
            int currentlyInfected = Mathf.Max(0, countryPopData.nbInfected - countryPopData.nbTreated - countryPopData.nbDeath);
            // We consider that the number of masks required corresponds to the number of serious cases * 5. Basically, each patient also mobilizes caregivers who must regularly change their mask, so one serious patient generates the use of 5 masks per day. In french context, with this parameter, the national stock of masks is used in 1 month
            masks.medicalRequirementPerDay_current = Mathf.Max(masks.medicalRequirementPerDay_low, currentlyInfected * virusStats.seriousRatio * 5);

            // Consideration of non-requisitioning, stress increases consumption, artisanal masks reduce consumption
            if (!masks.requisition)
                masks.medicalRequirementPerDay_current += (revolution.nationalInfectionIsCritic ? (Mathf.Max(0, revolution.stress-revolution.currentStressOnCriticToggled))/100 : 0) * (countryPopData.nbPopulation - countryPopData.nbTreated - countryPopData.nbDeath) * (masks.selfProtectionPromoted ? (float)2/3 : 1);

            masks.nationalStock = masks.nationalStock - masks.medicalRequirementPerDay_current + masks.nationalProductionPerDay_current;

            // Purchase of masks of the day from the national production
            finances.dailySpending += (masks.medicalRequirementPerDay_current- masks.medicalRequirementPerDay_low) * masks.maskMinPrice;

            masks.nationalStock = (masks.nationalStock < 0) ? 0 : masks.nationalStock; // limit to 0

            // Simulation of mask delivery
            masks.nextDelivery--;
            if (masks.nextDelivery <= 0 && masks.commands > 0 && frontierPermeability.currentState < 2) {
                masks.nextDelivery = masks.deliveryTime + Random.Range(-2, 3); // random addition in the delivery plus or minus 2 days
                float incomingMasks = Mathf.Min(masks.maxDeliveryPack*Random.Range(0.75f, 1.25f), masks.commands); // Receipt of an order at plus or minus 25% of the reference value
                masks.nationalStock += incomingMasks;
                masks.commands -= incomingMasks;
                // Update UI
                UI_PendingCommand.text = masks.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
                lastMasksDelivery = time.daysGone;
            }

            masks.historyStock.Add(masks.nationalStock);

            // check if the stock is at 0 over the last 8 days
            bool emptyStock = true;
            for (int i = masks.historyStock.Count - 1; i >= 0 && i >= masks.historyStock.Count - 8 && emptyStock; i--)
                emptyStock = masks.historyStock[i] == 0;
            // display a notification if the stock is empty when an order has already been received
            if (lastMasksDelivery != -1 && emptyStock)
            {
                string msgBody = localization.advisorHealthTexts[13];
                if (masks.commands == 0)
                {
                    msgBody += localization.advisorHealthTexts[14];
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[15];
                }
                else
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[16];

                GameObjectManager.addComponent<ChatMessage>(masks.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = msgBody });
                lastMasksDelivery = -1;
            }

            // Simulation of the price fluctuation of masks
            float newPrice = masks.maskPrice + Random.Range(-1, 2) * Random.Range(0f, 0.1f);
            masks.maskPrice = Mathf.Max(masks.maskMinPrice, Mathf.Min(masks.maskMaxPrice, newPrice));
            UI_UnitPriceValue.text = masks.maskPrice.ToString("C", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo); // monetary style

        }
    }

    /// <summary>
    /// Callback when mask requisition is toggled
    /// </summary>
    /// <param name="newState"></param>
    public void OnMaskRequisitionChange(bool newState)
    {
        masks.requisition = newState;
        masks.lastRequisitionUpdate = time.daysGone;
    }

    /// <summary>
    /// Callback when production boosting is toggled
    /// </summary>
    /// <param name="newState"></param>
    public void OnBoostProductionChange(bool newState)
    {
        masks.boostProduction = newState;
        masks.lastBoostProductionUpdate = time.daysGone;
    }

    /// <summary>
    /// when a new order is placed
    /// </summary>
    /// <param name="newState">The amount of the new order</param>
    public void NewCommand(TMP_InputField input)
    {
        if (input.text != "")
        {
            if (masks.commands == 0)
                masks.nextDelivery = masks.deliveryTime + Random.Range(-2, 3); // random addition in the delivery plus or minus 2 days
            float newCommand = float.Parse(input.text);
            input.text = "";
            finances.dailySpending += newCommand * masks.maskPrice;
            masks.commands += newCommand;
            UI_PendingCommand.text = masks.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
            masks.lastOrderPlaced = time.daysGone;
            masks.lastOrderAmount = newCommand;
        }
    }

    /// <summary>
    /// Callback when artisanal production is toggled
    /// </summary>
    /// <param name="newState"></param>
    public void OnArtisanalProductionChange(bool newState)
    {
        masks.selfProtectionPromoted = newState;
        masks.lastArtisanalProductionUpdate = time.daysGone;
        SyncUISystem.needUpdate = true;
    }

    /// <summary>
    /// Call to update notional stock on UI
    /// </summary>
    /// <param name="textUI"></param>
    public void UpdateMasksUI(TMPro.TMP_Text textUI)
    {
        SyncUISystem.formatStringUI(textUI, masks.nationalStock, localization);
    }

    /// <summary>
    /// Callback when border restriction is toggled
    /// </summary>
    /// <param name="newState"></param>
    public void OnFrontierChange(ItemSelector newValue)
    {
        UI_InputFieldCommand.interactable = newValue.currentItem == 0 || newValue.currentItem == 1;
        UI_ButtonCommand.interactable = UI_InputFieldCommand.interactable;
        UI_InputFieldCommand.GetComponent<TooltipContent>().enabled = !UI_InputFieldCommand.interactable;
        UI_ButtonCommand.GetComponent<TooltipContent>().enabled = !UI_InputFieldCommand.interactable;
    }
}