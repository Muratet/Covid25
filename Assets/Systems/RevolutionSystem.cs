using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Globalization;

public class RevolutionSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));
    private FrontierPermeability frontierPermeability;
    private TimeScale time;
    private Revolution revolution;
    private TerritoryData countryPopData;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Tax tax;
    private Masks masks;

    private int firstNotifStep = 10;
    private bool firstNotifStepFlag = false;
    private int secondNotifStep = 25;
    private bool secondNotifStepFlag = false;
    private int thirdNotifStep = 50;
    private bool thirdNotifStepFlag = false;
    private int fourthNotifStep = 70;
    private bool fourthNotifStepFlag = false;
    private int fifthNotifStep = 80;
    private bool fifthNotifStepFlag = false;

    public RevolutionSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();
        // Récupération de données de la frontière
        frontierPermeability = simu.GetComponent<FrontierPermeability>();
        // Récupération du stress de la population
        revolution = simu.GetComponent<Revolution>();
        // Récupération des données de la population
        countryPopData = simu.GetComponent<TerritoryData>();
        // Récupération des données du télétravail
        remoteworking = simu.GetComponent<Remoteworking>();
        // Récupération des données du chômage partiel
        shortTimeWorking = simu.GetComponent<ShortTimeWorking>();
        // Récupération des données du soutien aux entreprises
        tax = simu.GetComponent<Tax>();
        // Récupération des données des masques
        masks = simu.GetComponent<Masks>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Vérifier s'il faut générer une nouvelle journée
        if (time.newDay)
        {
            float newStress = 0;
            float timeMalus = 1;// +((float)time.daysGone / 30); // population de plus en plus critique tous les 15 jours
            // on ferme les écoles/collèges/lycées/université
            TerritoryData territory;
            float criticRatio = 0;
            foreach (GameObject territory_go in f_territories)
            {
                territory = territory_go.GetComponent<TerritoryData>();
                float territoryRatio = (float)territory.nbPopulation / countryPopData.nbPopulation;
                bool isCritic = territory.nbInfected - territory.nbTreated - territory.nbDeath > revolution.criticThreshold * territoryRatio;
                if (isCritic)
                    criticRatio++;
                // fermeture des écoles
                if (territory.closePrimarySchool && !isCritic)
                {
                    newStress += Random.Range(0f, revolution.closePrimSchoolPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name+ " 1 " + newStress);
                }
                if (territory.closeSecondarySchool && !isCritic)
                {
                    newStress += Random.Range(0f, revolution.closeScdSchoolPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 2 " + newStress);
                }
                if (territory.closeHighSchool && !isCritic) {
                    newStress += Random.Range(0f, revolution.closeHighSchoolPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 3 " + newStress);
                }
                if (territory.closeUniversity && !isCritic) {
                    newStress += Random.Range(0f, revolution.closeUniversityPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 4 " + newStress);
                }
                // combo fermeture des école + télétravail => stress de tout gérer à la maison
                if ((territory.closePrimarySchool || territory.closeSecondarySchool) && remoteworking.currentState) {
                    newStress += Random.Range(0f, revolution.comboSchoolRemoteworkPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 5 " + newStress);
                }

                if (territory.callCivicism && !isCritic)
                {
                    newStress += Random.Range(0f, revolution.callCivicPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 6 " + newStress);
                }

                // fermeture des commerces
                if (territory.closeShop && !isCritic) {
                    newStress += Random.Range(0f, revolution.closeShopPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 6 bis " + newStress);
                }
                // Restriction des libertés
                if (territory.certificateRequired && !isCritic) {
                    newStress += Random.Range(0f, revolution.certifRequiredPenalty * territoryRatio) * timeMalus;
//                    Debug.Log(territory.name + " 7 " + newStress);
                }
                // Interdiction age
                if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "" && !isCritic)
                {
                    // calcul de la population consernée sur ce territoire
                    int amount = 0;
                    int min = int.Parse(territory.ageDependentMin);
                    int max = int.Parse(territory.ageDependentMax);
                    for (int i = min; i < max; i++)
                        amount += territory.popNumber[i];
                    newStress += Random.Range(0f, revolution.ageRestrictionPenalty * amount/countryPopData.nbPopulation) * timeMalus;
//                    Debug.Log(territory.name + " 8");
                }
            }
            criticRatio = criticRatio / f_territories.Count;

            // inquiétude globale de la population
            bool newCritic = (countryPopData.nbInfected - countryPopData.nbDeath - countryPopData.nbTreated) > revolution.criticThreshold;
            if (newCritic != revolution.nationalInfectionIsCritic)
            {
                revolution.nationalInfectionIsCritic = newCritic;
                revolution.currentStressOnCriticToggled = revolution.stress;
            }
            // fermeture des frontières
            if (frontierPermeability.currentState > 0 && !revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.closeFrontierPenalty) * timeMalus;
//                Debug.Log("9");
            }
            else if (frontierPermeability.currentState == 0 && revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.openFrontierPenalty) * timeMalus;
//                Debug.Log("10");
            }

            // télétravail => réduit le stress
            if (remoteworking.currentState && revolution.nationalInfectionIsCritic) {
                newStress -= Random.Range(0f, revolution.remoteworkingBonus);
//                Debug.Log("11");
            }

            // chômage partiel => réduit le stress
            if (shortTimeWorking.currentState && revolution.nationalInfectionIsCritic) {
                newStress -= Random.Range(0f, revolution.shortTimeWorkBonus);
//                Debug.Log("12");
            }

            // soutien entreprise
            if (!tax.currentState && revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.taxPenalty) * timeMalus;
//                Debug.Log("13");
            }
            else if (tax.currentState && revolution.nationalInfectionIsCritic) {
                newStress -= Random.Range(0f, revolution.taxSupportRequiredBonus);
//                Debug.Log("14");
            }
            else if (tax.currentState && !revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.taxSupportNotRequiredPenalty);
//                Debug.Log("15");
            }

            // réquisition des masques
            if (masks.requisition && revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.maskRequisitionPenalty) * timeMalus * (2f - Mathf.Min(masks.nationalStock / revolution.maskStockThreshold, 1));
//                Debug.Log("16");
            }
            if (masks.boostProduction && revolution.nationalInfectionIsCritic) {
                newStress -= Random.Range(0f, revolution.maskBoostProdBonus);
//                Debug.Log("17");
            }
            if (masks.selfProtectionPromoted && !revolution.nationalInfectionIsCritic) {
                newStress += Random.Range(0f, revolution.maskSelfProtectPenalty) * timeMalus;
//                Debug.Log("18 " + newStress);
            }

            // Adaptation du stress en fonction de la pente de décés
            int size = countryPopData.numberOfInfectedPeoplePerDays.Length;
            float coeffDir = Mathf.Max(-1, (float)(countryPopData.numberOfInfectedPeoplePerDays[Mathf.Max(0, size - 8)] - countryPopData.numberOfInfectedPeoplePerDays[size - 1]) / revolution.deathGradient);
            if (coeffDir > 0.2f)
            {
                newStress += Random.Range(0f, revolution.deathIncreasePenalty * coeffDir);
//                Debug.Log("19");
            }
            else
            {
                // Prise en compte du bonus de non nouveau mort
                newStress -= Random.Range(0f, revolution.deathStagnationBonus);
                // Prise ne compte du bonus si baisse du nombre de morts
                if (coeffDir < 0)
                {
                    newStress -= Random.Range(0f, revolution.deathDecreaseBonus * (-1 * coeffDir));
//                    Debug.Log("20");
                }
            }

            // prise en compte du nouveau stress
            revolution.stress += newStress;
            // maintenir le stress entre [0, 100]
            revolution.stress = Mathf.Max(0f, Mathf.Min(100f, revolution.stress));

            if (revolution.stress > firstNotifStep && !firstNotifStepFlag)
            {
                firstNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = "Une partie de la population est mécontente de votre politique. "+((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? "Ils ne comprennent pas pourquoi vous ne tentez pas de mieux contrôler l'épidémie." : "Ils ne comprennent pas pourquoi vous maintenez des mesures aussi dures.") });
            }
            else if (revolution.stress > secondNotifStep && !secondNotifStepFlag)
            {
                secondNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = "Plus de "+ secondNotifStep.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "% de la population critique vos choix. " + ((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? "Ils attendent des mesures plus strictes pour contrôler l'épidémie." : "Ils pensent que vous devriez relacher les contraintes.") });
            }
            else if (revolution.stress > thirdNotifStep && !thirdNotifStepFlag)
            {
                thirdNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = "Vous devriez adapter vos mesures pour rassurer la population. " + thirdNotifStep.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "% des Français sont mécontents. " + ((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? "Ils se sentent abandonnés et attendent des mesures plus fortes." : "Ils trouvent que vos choix sont trop stricts.") });
            }
            else if (revolution.stress > fourthNotifStep && !fourthNotifStepFlag)
            {
                fourthNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = "Des troubles à l'ordre public se manifestent dans plusieurs régions françaises. Si le mécontentement continue à croître nous allons droit à la crise sociale." });
            }
            else if (revolution.stress > fifthNotifStep && !fifthNotifStepFlag)
            {
                fifthNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = "Nous avons dépassé le seuil critique. Vous devez rapidement faire redescendre le stress de la population en dessous de "+ fifthNotifStep.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "%." });
            }
            if (revolution.stress < firstNotifStep - 5 && firstNotifStepFlag)
                firstNotifStepFlag = false;
            if (revolution.stress < secondNotifStep - 5 && secondNotifStepFlag)
                secondNotifStepFlag = false;
            if (revolution.stress < thirdNotifStep - 5 && thirdNotifStepFlag)
                thirdNotifStepFlag = false;
            if (revolution.stress < fourthNotifStep - 5 && fourthNotifStepFlag)
                fourthNotifStepFlag = false;
            if (revolution.stress < fifthNotifStep - 5 && fifthNotifStepFlag)
                fifthNotifStepFlag = false;

            revolution.historyStress.Add(revolution.stress);
        }
    }

    public void UpdateRevolutionUI(TMPro.TMP_Text textUI)
    {
        textUI.text = revolution.stress.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "%";
        textUI.color = new Color(1f, 1f - revolution.stress/80, 1f - revolution.stress/80);
    }
}