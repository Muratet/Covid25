using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Globalization;

public class FinanceSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Finances finances;
    private TerritoryData countryPopData;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Tax tax;
    private Beds beds;

    private float taxProgress = 0f;

    private float nextStepNotif = 10000000f;
    private bool stability = true;
    private bool bedsNotif = false;

    public FinanceSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();
        // Récupération des données de la population
        countryPopData = simu.GetComponent<TerritoryData>();
        // Récupération de données de la frontière
        frontierPermeability = simu.GetComponent<FrontierPermeability>();
        // Récupération des finances
        finances = simu.GetComponent<Finances>();
        // Récupération de données du télétravail
        remoteworking = simu.GetComponent<Remoteworking>();
        // Récupération de données du chômage partiel
        shortTimeWorking = simu.GetComponent<ShortTimeWorking>();
        // Récupération de données des impôts de entreprises
        tax = simu.GetComponent<Tax>();
        // Récupération de données des lits de réanimation
        beds = simu.GetComponent<Beds>();
        finances.historySpent.Add(0);
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Vérifier s'il faut générer une nouvelle journée
        if (time.newDay)
        {
            float newSpent = 0;

            // Gestion de la fermeture des frontières
            if (frontierPermeability.currentState >= 1)
                newSpent += Random.Range(50000f, 150000f); // 100 000 € de perte en moyenne du à l'arrêt du tourisme
            if (frontierPermeability.currentState >= 2)
                newSpent += Random.Range(500000f, 1500000f); // 1 000 000 € de perte supplémentaire en moyenne du à l'arrêt du commerce hors zone europe
            if (frontierPermeability.currentState >= 3)
                newSpent += Random.Range(200000f, 600000f); // 400 000 € de perte supplémentaire en moyenne du au confinement total

            float remoteworkingImpact = (remoteworking.currentState ? 0.75f : 1f); // si activation du télétravail baisse l'impact financier dû à une activité à domicile
            float shortTimeWorkingImpact = (shortTimeWorking.currentState ? 100f : 1f); // si activation du chômage partiel fait exploser l'impact financier dû aux charges sociales

            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                // Chiffres INSEE : 30Md en 2 mois pour un scénario avec télétravail (0.75f) + chômage partiel (10f) => donc par jour ça donne : 30Md / 2 mois / 30 jours / (100 * 0.75) = 6 666 666 € => arrondi à 6 000 000 € et à pondérer en fonction de la proportion de la population dans la région

                // Gestion de la fermeture des boutiques
                if (territoryData.closeShop)
                {
                    territoryData.closeShopDynamic = Mathf.Min(1, territoryData.closeShopDynamic + 0.1f);
                    newSpent += territoryData.closeShopDynamic * territoryData.populationRatio * Random.Range(4000000f, 8000000f) * remoteworkingImpact * shortTimeWorkingImpact; // 6 Million de perte par jour dû au ralentissement de l'économie modulé par les actions sociales. 
                } else
                    territoryData.closeShopDynamic = Mathf.Max(0, territoryData.closeShopDynamic - 0.1f);

                // Gestion du confinement par age (si la tranche d'age concerne des travailleurs)
                if (territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && int.Parse(territoryData.ageDependentMin) < 62 && int.Parse(territoryData.ageDependentMax) >= 62)
                {
                    int ageMin = int.Parse(territoryData.ageDependentMin);
                    int ageMax = int.Parse(territoryData.ageDependentMax);
                    // Une partie de la population en confinement total est en age de travailler => Calcul de la proportion de la population de la région concernée
                    int workerConfined = 0;
                    for (int age = ageMin; age <= ageMax && age < 62; age++)
                        workerConfined += territoryData.popNumber[age] - territoryData.popDeath[age];
                    newSpent += workerConfined / (countryPopData.nbPopulation - countryPopData.nbDeath) * Random.Range(500000f, 1500000f) * remoteworkingImpact * shortTimeWorkingImpact; // 1 Million de perte par jour dû au ralentissement de l'économie modulé par les actions sociales
                }
            }

            // Gestion des impôts des entreprises
            if (tax.currentState)
            {
                taxProgress = Mathf.Min(1, taxProgress + 0.1f);
                newSpent += taxProgress * Random.Range(100000000f, 7000000000f); // 42 Milliards en deux mois (chiffre INSEE) => 700 Millions par jour max
            } else
                taxProgress = Mathf.Max(0, taxProgress - 0.1f);

            // Calcul du coup financier d'une journée d'un lit de réa
            newSpent += Mathf.Min(beds.intensiveBeds_need, beds.intensiveBeds_current) * finances.oneDayReanimationCost;
            if (!bedsNotif && beds.intensiveBeds_need > 0)
            {
                bedsNotif = true;
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = "Report from public hospitals", timeStamp = "" + time.daysGone, messageBody = "A day in intensive care costs " + finances.oneDayReanimationCost.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "€ for each patient." });
            }


            // prise en compte des financement extérieurs à ce système (achats de masques et de vaccins en l'occurence)
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
                    case 0: messageChosen = "The opposition is worried about the accumulation of debt!!!"; break;
                    case 1: messageChosen = "The country's debt due to the crisis is reaching " + newDebt.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " €. For information, the national debt before the crisis was 2,500,000,000 €."; break;
                    case 2: messageChosen = "Will we be able to sustain this pace of measures for a long time to come?"; break;
                    case 3: messageChosen = "With this rate of spending, the country will take years to recover."; break;
                }
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = "Economy Minister", timeStamp = "" + time.daysGone, messageBody = messageChosen });
                nextStepNotif *= 10;
            }

            finances.historySpent.Add(newDebt);

            if (!stability && finances.historySpent[finances.historySpent.Count - 1] == finances.historySpent[Mathf.Max(0, finances.historySpent.Count - 10)])
            {
                GameObjectManager.addComponent<ChatMessage>(finances.gameObject, new { sender = "Economy Minister", timeStamp = "" + time.daysGone, messageBody = "Expenses have been under control for several days, so let's continue." });
                stability = true;
            }
        }
    }

    public void UpdateFinanceUI(TMPro.TMP_Text textUI)
    {
        float financeAmount = 0;
        if (finances.historySpent.Count > 1)
            financeAmount = finances.historySpent[finances.historySpent.Count - 1];
        SyncUISystem.formatStringUI(textUI, financeAmount);
    }
}