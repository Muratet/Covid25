using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;

public class MaskSystem : FSystem {
    public GameObject countrySimData;
    private TimeScale time;
    private TerritoryData countryPopData;
    private Masks masks;
    private VirusStats virusStats;
    private Revolution revolution;
    private Finances finances;
    private FrontierPermeability frontierPermeability;

    public TMP_InputField UI_InputFieldCommand;
    public Button UI_ButtonCommand;
    public TMP_Text UI_UnitPriceValue;
    public TMP_Text UI_PendingCommand;
    private int lastMasksDelivery = 0;

    public Localization localization;

    protected override void onStart()
    {
        // Récupération de l'échelle de temps
        time = countrySimData.GetComponent<TimeScale>();
        // Récupération des données de la population
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Récupération des stats du virus
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Récupération des masques
        masks = countrySimData.GetComponent<Masks>();
        // Récupération de risque de révolution
        revolution = countrySimData.GetComponent<Revolution>();
        // Récupération des finances
        finances = countrySimData.GetComponent<Finances>();
        // Récupération de données de la frontière
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        if (time.newDay)
        {
            // Mise à jour de la capacité de production
            if (masks.boostProduction)
                masks.nationalProductionPerDay_current += (masks.nationalProductionPerDay_high - masks.nationalProductionPerDay_current) / 20; // Le 20 permet de ralentir la croissance de la production (le temps de construire d'autres lignes, de lancer des startup...)
            else
                masks.nationalProductionPerDay_current -= (masks.nationalProductionPerDay_current - masks.nationalProductionPerDay_low) / 2; // arrêt beaucoup plus rapide que le démarrage de la production

            // calcul de la consommation de masque
            // nombre de personne actuellement infectées
            int currentlyInfected = Mathf.Max(0, countryPopData.nbInfected - countryPopData.nbTreated - countryPopData.nbDeath);
            // On considère que le nombre de masque requis correspond au nombre de cas sérieux * 5. En gros chaque patient mobilise également des soignants qui doivent changer réguliairement leur masque donc un malade sérieux génère l'utilisation de 5 masques par jours. Avec ce paramètre le stock de masques de la réserve d'état est mangé en 1 mois
            masks.medicalRequirementPerDay_current = Mathf.Max(masks.medicalRequirementPerDay_low, currentlyInfected * virusStats.seriousRatio * 5);

            // Prise en compte du non réquisitionnement, le stress augmente la consommation, les masques artisanaux réduisent la consommation
            if (!masks.requisition)
                masks.medicalRequirementPerDay_current += (revolution.nationalInfectionIsCritic ? (Mathf.Max(0, revolution.stress-revolution.currentStressOnCriticToggled))/100 : 0) * (countryPopData.nbPopulation - countryPopData.nbTreated - countryPopData.nbDeath) * (masks.selfProtectionPromoted ? (float)2/3 : 1);

            masks.nationalStock = masks.nationalStock - masks.medicalRequirementPerDay_current + masks.nationalProductionPerDay_current;

            // Achat des masques du jour de la production nationale
            finances.dailySpending += (masks.medicalRequirementPerDay_current- masks.medicalRequirementPerDay_low) * masks.maskMinPrice;

            masks.nationalStock = (masks.nationalStock < 0) ? 0 : masks.nationalStock; // borner à 0

            // Simulation de la livraison des masques
            masks.nextDelivery--;
            if (masks.nextDelivery <= 0 && masks.commands > 0 && frontierPermeability.currentState < 2) {
                masks.nextDelivery = masks.deliveryTime + Random.Range(-2, 3); // ajout d'aléatoire dans la livraison plus ou moins 2 jours
                float incomingMasks = Mathf.Min(masks.maxDeliveryPack*Random.Range(0.75f, 1.25f), masks.commands); // Reception d'une commande à plus ou moins 25% de la valeur de référence
                masks.nationalStock += incomingMasks;
                masks.commands -= incomingMasks;
                // Update UI
                UI_PendingCommand.text = masks.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
                lastMasksDelivery = time.daysGone;
            }

            masks.historyStock.Add(masks.nationalStock);

            // vérifier si le stock est à 0 sur les 8 derniers jours
            bool emptyStock = true;
            for (int i = masks.historyStock.Count - 1; i >= 0 && i >= masks.historyStock.Count - 8 && emptyStock; i--)
                emptyStock = masks.historyStock[i] == 0;
            // afficher une notification si le stock est vide alors qu'on a déjà reçu une commande
            if (lastMasksDelivery != -1 && emptyStock)
            {
                string msgBody = localization.advisorHealthTexts[14];
                if (masks.commands == 0)
                {
                    msgBody += localization.advisorHealthTexts[15];
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[16];
                }
                else
                    if (frontierPermeability.currentState >= 2)
                        msgBody += localization.advisorHealthTexts[17];

                GameObjectManager.addComponent<ChatMessage>(masks.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = msgBody });
                lastMasksDelivery = -1;
            }

            // Simulation la fluctuation des prix des masques
            float newPrice = masks.maskPrice + Random.Range(-1, 2) * Random.Range(0f, 0.1f);
            masks.maskPrice = Mathf.Max(masks.maskMinPrice, Mathf.Min(masks.maskMaxPrice, newPrice));
            UI_UnitPriceValue.text = masks.maskPrice.ToString("C", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo); // style monétaire

        }
    }

    public void OnMaskRequisitionChange(bool newState)
    {
        masks.requisition = newState;
        masks.lastRequisitionUpdate = time.daysGone;
    }

    public void OnBoostProductionChange(bool newState)
    {
        masks.boostProduction = newState;
        masks.lastBoostProductionUpdate = time.daysGone;
    }

    public void NewCommand(TMP_InputField input)
    {
        if (input.text != "")
        {
            if (masks.commands == 0)
                masks.nextDelivery = masks.deliveryTime + Random.Range(-2, 3); // ajout d'aléatoire dans la livraison plus ou moins 2 jours
            float newCommand = float.Parse(input.text);
            input.text = "";
            finances.dailySpending += newCommand * masks.maskPrice;
            masks.commands += newCommand;
            UI_PendingCommand.text = masks.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
            masks.lastOrderPlaced = time.daysGone;
            masks.lastOrderAmount = newCommand;
        }
    }

    public void OnArtisanalProductionChange(bool newState)
    {
        masks.selfProtectionPromoted = newState;
        masks.lastArtisanalProductionUpdate = time.daysGone;
        SyncUISystem.needUpdate = true;
    }

    public void UpdateMasksUI(TMPro.TMP_Text textUI)
    {
        SyncUISystem.formatStringUI(textUI, masks.nationalStock);
    }

    public void OnFrontierChange(int newValue)
    {
        UI_InputFieldCommand.interactable = newValue == 0 || newValue == 1;
        UI_ButtonCommand.interactable = UI_InputFieldCommand.interactable;
        UI_InputFieldCommand.GetComponent<TooltipContent>().enabled = !UI_InputFieldCommand.interactable;
        UI_ButtonCommand.GetComponent<TooltipContent>().enabled = !UI_InputFieldCommand.interactable;
    }
}