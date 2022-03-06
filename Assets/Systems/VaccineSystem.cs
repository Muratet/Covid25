using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Globalization;
using System.Collections.Generic;

public class VaccineSystem : FSystem {
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    public GameObject countrySimData;
    private VirusStats virusStats;
    private TimeScale time;
    private Vaccine vaccine;
    private Finances finances;
    private FrontierPermeability frontierPermeability;
    private Revolution revolution;
    private TerritoryData countryPopData;

    public Localization localization;

    private int notifVaccine = 0;
    private bool firstVaccineDelivered = false;
    private int lastVaccineDelivery = -1;

    protected override void onStart()
    {
        // Récupération de l'échelle de temps
        time = countrySimData.GetComponent<TimeScale>();
        // Récupération des stats du virus
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Récupération des données du vaccin
        vaccine = countrySimData.GetComponent<Vaccine>();
        // Récupération des finances
        finances = countrySimData.GetComponent<Finances>();
        // Récupération de données de la frontière
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Récupération du stress de la population
        revolution = countrySimData.GetComponent<Revolution>();
        // Récupération des données de la population
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
                // Appeler la fonction qui défini si les champs sont intéractifs ou pas
                _OnFrontierChange(frontierPermeability.currentState);
                GameObjectManager.addComponent<ChatMessage>(vaccine.gameObject, new { sender = localization.advisorTitleEconomy, timeStamp = "" + time.daysGone, messageBody = localization.advisorEconomyTexts[5] });
                // Ajout d'un bonus ponctuel sur le stress
                revolution.stress -= 5f;
            }

            
            if (vaccine.UI_researchBar.value >= 100)
            {
                CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
                // Simulation de la fluctuation des prix des vaccins
                int newPrice = vaccine.vaccinePrice + Random.Range(-1, 2) * Random.Range(0, 4);
                vaccine.vaccinePrice = Mathf.Max(vaccine.vaccineMinPrice, Mathf.Min(vaccine.vaccineMaxPrice, newPrice));
                vaccine.UI_vaccineUnitPrice.text = vaccine.vaccinePrice.ToString("C0", cultureInfo); // style monétaire

                // Simulation de la livraison des vaccins
                vaccine.nextDelivery--;
                if (vaccine.nextDelivery <= 0 && vaccine.commands > 0 && frontierPermeability.currentState < 2)
                {
                    vaccine.nextDelivery = vaccine.deliveryTime + Random.Range(-2, 3); // ajout d'aléatoire dans la livraison plus ou moins 2 jours
                    float incomingVaccines = Mathf.Min(vaccine.meanDeliveryPack * Random.Range(0.5f, 1.5f), vaccine.commands); // Reception d'une commande à plus ou moins 25% de la valeur de référence
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

                // Le goulot d'étrangelement pour la vaccination est lié à la production du vaccin plus que par la capacité à vacciner beaucoup de gens en même temps. Donc on lisse le capacité journalière de vaccin sur 8 jours en fonction de la moyenne de livraison.
                int newVaccination = (int)Mathf.Min(vaccine.nationalStock, Random.Range(0.75f, 1.25f) * vaccine.meanDeliveryPack / 8);
                vaccine.nationalStock = vaccine.nationalStock - newVaccination;

                List<int> availableAge = new List<int>(); // liste des 20 ages les plus anciens à cibler pour vacciner
                List<int> availableCount = new List<int>(); // quantité de population pour chaque age ciblé
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
                    int ageId = Random.Range(0, Random.Range(0, availableAge.Count)); // On tire un age aléatoire A les 20 classes d'âges les plus à risque puis à nouveau un age aléatoire entre A et l'âge le plus à risque pour augmenter la probabilité d'obtenir un âge plus à risque
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
                    // Vérifier qu'il reste des personnes à vacciner pour cet âge là
                    if (availableCount[ageId] <= 0)
                    {
                        availableAge.RemoveAt(ageId);
                        availableCount.RemoveAt(ageId);
                    }
                    newVaccination -= localVaccination;
                }
            }

            vaccine.historyStock.Add((int)vaccine.nationalStock);

            // vérifier si le stock est à 0 sur les 8 derniers jours
            bool emptyStock = true;
            for (int i = vaccine.historyStock.Count - 1; i >= 0 && i >= vaccine.historyStock.Count - 8 && emptyStock; i--)
                emptyStock = vaccine.historyStock[i] == 0;
            // afficher une notification si le stock est vide alors qu'on a déjà reçu une commande
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

    public void NewCommand(TMP_InputField input)
    {
        if (input.text != "")
        {
            if (vaccine.commands == 0)
                vaccine.nextDelivery = vaccine.deliveryTime + Random.Range(-2, 3); // ajout d'aléatoire dans la livraison plus ou moins 2 jours
            int newCommand = int.Parse(input.text);
            input.text = "";
            finances.dailySpending += newCommand * vaccine.vaccinePrice;
            vaccine.commands += newCommand;
            vaccine.UI_vaccinePendingCommand.text = vaccine.commands.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
            vaccine.lastOrderPlaced = time.daysGone;
            vaccine.lastOrderAmount = newCommand;
        }
    }

    public void OnFrontierChange(ItemSelector newValue)
    {
        _OnFrontierChange(newValue.currentItem);
    }
    public void _OnFrontierChange(int newValue)
    {
        vaccine.UI_vaccineQuantity.interactable = (newValue == 0 || newValue == 1) && vaccine.UI_researchBar.value >= 100 && notifVaccine >= 4;
        vaccine.UI_vaccineCommand.interactable = vaccine.UI_vaccineQuantity.interactable;
        vaccine.UI_vaccineQuantity.GetComponents<TooltipContent>()[1].enabled = notifVaccine >= 4 && !vaccine.UI_vaccineQuantity.interactable;
        vaccine.UI_vaccineCommand.GetComponents<TooltipContent>()[1].enabled = notifVaccine >= 4 && !vaccine.UI_vaccineQuantity.interactable;
    }

    public void UpdateMasksUI(TMPro.TMP_Text textUI)
    {
        SyncUISystem.formatStringUI(textUI, vaccine.nationalStock);
    }
}