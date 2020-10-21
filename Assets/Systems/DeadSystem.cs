using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Globalization;

public class DeadSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    private VirusStats virusStats;
    private TerritoryData countryPopData;
    private TimeScale time;

    // model deadliness during X days
    private float[] deadlinessPerDays;

    // give deadliness probability for each age
    private float[] deadlinessPerAges;

    public static DeadSystem instance;

    private bool firstDead = false;
    private int nextDeathNotification = 100;

    public DeadSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération des stats du virus
        virusStats = simu.GetComponent<VirusStats>();
        // Récupération des données de la population
        countryPopData = simu.GetComponent<TerritoryData>();
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();

        // calcul de la courbe de mortalité pour une fenêtre de jours
        deadlinessPerDays = new float[virusStats.windowSize];
        float peak = virusStats.deadlinessPeak;
        float deviation = virusStats.deadlinessDeviation;
        for (int i = 0; i < deadlinessPerDays.Length; i++)
            deadlinessPerDays[i] = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation));

        // Calcul de la mortalité en fonction de l'age
        deadlinessPerAges = new float[101];
        // Calcul de la valeur de l'exponentielle pour le premier age à partir duquel des morts peuvent arriver
        float minAgeExpo = Mathf.Exp(virusStats.curveStrenght * ((float)virusStats.firstSensitiveAge / 100 - 1));
        // Calcul de la valeur maximale de l'exponentielle pour l'age le plus avancé
        float maxExpo = 1 - minAgeExpo;
        // lissage de la mortalité pour quelle soit à 0 au premier age sensible et à sa valeur maximale pour l'age le plus avancé
        for (int age = 0; age < deadlinessPerAges.Length; age++)
            deadlinessPerAges[age] = Mathf.Max(0f, (Mathf.Exp(virusStats.curveStrenght * ((float)age / 100 - 1)) - minAgeExpo) / maxExpo);

        instance = this;
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        // Vérifier s'il faut générer une nouvelle journée
        if (time.newDay)
        {
            // Traiter chaque territoire
            TerritoryData territoryData;
            foreach (GameObject territory in f_territories)
            {
                territoryData = territory.GetComponent<TerritoryData>();

                // Comptabilisation du nombre de personne qui devraient pouvoir accéder à des lits de réanimation, on prend un fenêtre de 9 jours autour du pic de mortalité (même calcul fait dans BedsSystem)
                int criticAmount = 0;
                for (int day = Mathf.Max(0, (int)virusStats.deadlinessPeak - 4); day < Mathf.Min(virusStats.deadlinessPeak + 5, territoryData.numberOfInfectedPeoplePerDays.Length); day++)
                    criticAmount = (int)(territoryData.numberOfInfectedPeoplePerDays[day] * virusStats.seriousRatio);
                // Calcul du malus dû à l'occupation des lits
                // Le Max entre 1 et le log permet de commencer à ajouter du malus (plus de mortalité) dès que le nombre de lit de réanimation commence à être dépassé
                float bedMalus = Mathf.Max(1f, Mathf.Log10((float)criticAmount / territory.GetComponent<Beds>().intensiveBeds_current) + 1);

                //////////////////////////////////////
                // MORTALITE
                //////////////////////////////////////
                for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                {
                    for (int day = territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length - 1; day >= 0; day--) // parcours des jours en sens inverse pour traiter d'abord ceux qui sont malades depuis le plus longtemps
                    {
                        // récupérer combien de personnes sont infectées pour cette tranche d'age et pour le jour courant
                        int infectedNumber = territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day];
                        // Caculer le nombre de mort en fonction du nombre d'infectés, de la mortalité de l'age, du risque de mortalité du jour et du manque éventuel de lit de réanimation
                        // On applique les taux par défaut pour le nombre de lit de réanimation disponible
                        float nbDead_float = infectedNumber * deadlinessPerAges[age] * deadlinessPerDays[day] * bedMalus;
                        int nbDead = Mathf.RoundToInt(nbDead_float);
                        // récupération et accumulation de la partie flotante
                        territoryData.popPartialDeath[age] += nbDead_float - (int)nbDead_float;
                        // ajout d'un mort supplémentaire si les accumulations précédentes dépassent l'unité
                        int cumulatedDeath = (int)territoryData.popPartialDeath[age];
                        // prise en compte de l'éventuel mort supplémentaire
                        nbDead += cumulatedDeath;
                        // retirer de l'accumulation l'éventuel mort supplémentaire
                        territoryData.popPartialDeath[age] -= cumulatedDeath;

                        if (nbDead > 0)
                        {
                            // retirer ces morts du territoire
                            territoryData.nbDeath += nbDead;
                            territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= nbDead;
                            territoryData.popDeath[age] += nbDead;
                            territoryData.numberOfInfectedPeoplePerDays[day] -= nbDead;
                            // retirer ces morts au niveau national
                            countryPopData.nbDeath += nbDead;
                            countryPopData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= nbDead;
                            countryPopData.popDeath[age] += nbDead;
                            countryPopData.numberOfInfectedPeoplePerDays[day] -= nbDead;
                        }
                    }
                }
                // Comptabilisation de l'historique des nouveaux décés pour la région
                territoryData.cumulativeDeath.Add(territoryData.nbDeath);
                if (territoryData.cumulativeDeath.Count > 1)
                    territoryData.historyDeath.Add(territoryData.nbDeath - territoryData.cumulativeDeath[territoryData.cumulativeDeath.Count - 2]);
                else
                    territoryData.historyDeath.Add(territoryData.nbDeath);
            }
            // Comptabilisation de l'historique des nouveaux décés pour la nation
            countryPopData.cumulativeDeath.Add(countryPopData.nbDeath);
            if (countryPopData.cumulativeDeath.Count > 1)
                countryPopData.historyDeath.Add(countryPopData.nbDeath - countryPopData.cumulativeDeath[countryPopData.cumulativeDeath.Count - 2]);
            else
                countryPopData.historyDeath.Add(countryPopData.nbDeath);
            if (countryPopData.historyDeath[countryPopData.historyDeath.Count - 1] >= 1 && !firstDead)
            {
                GameObjectManager.addComponent<ChatMessage>(countryPopData.gameObject, new { sender = "Ministre de la santé", timeStamp = "" + time.daysGone, messageBody = "Un rapport de mon cabinet indique que nous avons eu un premier décès dû à ce nouveau coronavirus." });
                firstDead = true;
            }
            if (countryPopData.historyDeath[countryPopData.historyDeath.Count - 1] > nextDeathNotification)
            {
                string msgBody = "Le nombre de morts journalier a dépassé " + nextDeathNotification.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + ".";
                if (nextDeathNotification > 200000)
                    msgBody += " Vous rendez-vous compte ??? Plus de 200 000 morts rien qu'aujourd'hui...";
                else if (nextDeathNotification > 100000)
                    msgBody += " C'est une catastrophe, vous devrez répondre de vos actes, c'est comme si toute la population de Nanterre ou de Nancy était rayé de la carte... en une seule journée !!!";
                else if (nextDeathNotification > 50000)
                    msgBody += " Vous êtes en train de sacrifier toute une partie de la population.";
                else if (nextDeathNotification > 24000)
                    msgBody += " Vous devez absoluement faire quelque chose.";
                else if (nextDeathNotification > 12000)
                    msgBody += " La situation est très critique.";
                else if (nextDeathNotification > 6000)
                    msgBody += " La courbe de mortalité continue à croitre.";
                else if (nextDeathNotification > 3000)
                    msgBody += " Les morts s'accumulent et nos morgues sont pleines.";
                else if (nextDeathNotification > 1500)
                    msgBody += " Nous avons dépassé le record de la pandémie de 2020.";
                else if (nextDeathNotification > 750)
                    msgBody += " Vous devriez prendre des mesures pour contenir la pandémie.";
                else if (nextDeathNotification > 325)
                    msgBody += " Faites attention à ce que le nombre de morts ne s'embale pas.";

                GameObjectManager.addComponent<ChatMessage>(countryPopData.gameObject, new { sender = "Ministre de la santé", timeStamp = "" + time.daysGone, messageBody = msgBody });
                nextDeathNotification *= 2;
            }
        }
    }

    public void UpdateDailyDeadUI (TMPro.TMP_Text textUI)
    {
        int dailyDead = 0;
        if (MapSystem.territorySelected.historyDeath.Count > 1)
            dailyDead = MapSystem.territorySelected.historyDeath[MapSystem.territorySelected.historyDeath.Count - 1];
        SyncUISystem.formatStringUI(textUI, dailyDead);
    }

    public void UpdateCumulDeadUI(TMPro.TMP_Text textUI)
    {
        int cumulDead = 0;
        if (MapSystem.territorySelected.cumulativeDeath.Count > 1)
            cumulDead = MapSystem.territorySelected.cumulativeDeath[MapSystem.territorySelected.cumulativeDeath.Count - 1];
        SyncUISystem.formatStringUI(textUI, cumulDead);
    }
}