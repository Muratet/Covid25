using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections.Generic;
using System.Globalization;

public class InfectionSystem : FSystem
{
    private Family f_territoriesAndCountry = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    public GameObject countrySimData;
    private VirusStats virusStats;
    private TerritoryData countryPopData;
    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Remoteworking remoteworking;
    private Masks masks;
    private InfectionImpact confinementImpact;

    public Localization localization;

    // model contagiousness during X days
    private float[] contagiousnessProbabilityPerDays;

    private float polyA;
    private float polyB;
    private float polyC;

    public static InfectionSystem instance;

    private bool firstInfection = false;

    public InfectionSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Récupération des stats du virus
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Récupération des données de la population
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Récupération de l'échelle de temps
        time = countrySimData.GetComponent<TimeScale>();
        // Récupération des masques
        masks = countrySimData.GetComponent<Masks>();
        // Récupération de l'impact du confinement
        confinementImpact = countrySimData.GetComponent<InfectionImpact>();
        // Récupération de données de la frontière
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Récupération de données du télétravail
        remoteworking = countrySimData.GetComponent<Remoteworking>();

        // calcul de la courbe de contagiosité pour une fenêtre de jours
        contagiousnessProbabilityPerDays = new float[virusStats.windowSize];
        float peak = virusStats.contagiousnessPeak;
        float deviation = virusStats.contagiousnessDeviation;
        for (int i = 0; i < contagiousnessProbabilityPerDays.Length; i++)
            contagiousnessProbabilityPerDays[i] = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation));

        TerritoryData territoryData;
        foreach (GameObject territory in f_territoriesAndCountry)
        {
            territoryData = territory.GetComponent<TerritoryData>();
            // Initialisation du nombre d'infectés pour chaque jour de la fenêtre
            territoryData.numberOfInfectedPeoplePerDays = new int[virusStats.windowSize];
            for (int day = 0; day < virusStats.windowSize; day++)
                territoryData.numberOfInfectedPeoplePerDays[day] = 0;
            // Initialisation du nombre d'infectés pour chaque age et pour chaque jour de la fenêtre
            territoryData.numberOfInfectedPeoplePerAgesAndDays = new int[territoryData.popNumber.Length][];
            for (int age = 0; age < territoryData.popNumber.Length; age++)
            {
                territoryData.numberOfInfectedPeoplePerAgesAndDays[age] = new int[virusStats.windowSize];
                for (int day = 0; day < virusStats.windowSize; day++)
                    territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] = 0;
            }
        }

        // Pour déterminer la contagiosité du virus on doit trouver le polynome qui passe par trois points :
        //   - si % population infecté == 0 => contagiosité par défaut du virus
        //   - si % population infecté == % d'immunité => contagiosité == 1
        //   - si % population infecté == 1 => contagiosité == 0
        // On doit donc trouver les valeurs a, b et c du polynome aX²+bX+c=Y avec X <=> % de population infecté et Y la contagiosité finale
        // Donc on doit résoudre le système
        //   --
        //   | a*0² + b*0 + c = contagVirus
        //   | a*immu² + b*immu + c = 1
        //   | a*1² + b*1 + c = 0
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² + b*immu + contagVirus = 1
        //   | a + b = -contagVirus
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² + b*immu + contagVirus = 1
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² + (-contagVirus - a)*immu + contagVirus = 1
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² - contagVirus*immu -a*immu + contagVirus = 1
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² - a*immu = 1 + contagVirus*immu - contagVirus
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a*immu² - a*immu = 1 + (immu - 1) * contagVirus
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a * (immu² - immu) = 1 + (immu - 1) * contagVirus
        //   | b = -contagVirus - a
        //   --
        //   --
        //   | c = contagVirus
        //   | a = (1 + (immu - 1) * contagVirus) / (immu² - immu)
        //   | b = -contagVirus - a
        //   --
        // Prise en compte des cas limites
        if (virusStats.populationRatioImmunity <= 0)
        {
            polyA = 0;
            polyC = 0;
            polyB = 0;
        }
        else if (virusStats.populationRatioImmunity >= 1)
        {
            polyA = 0;
            polyC = virusStats.contagiosity;
            polyB = 0;
        }
        else
        {
            polyC = virusStats.contagiosity;
            polyA = (1 + (virusStats.populationRatioImmunity - 1) * virusStats.contagiosity) / (virusStats.populationRatioImmunity * virusStats.populationRatioImmunity - virusStats.populationRatioImmunity);
            polyB = -polyC - polyA;
        }
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
                //////////////////////////////////////
                // EMERGENCE
                //////////////////////////////////////
                // Calcul de la probabilité d'apparition d'un nouveau patient zéro => dépend de l'ouverture des frontières et de la densité de chaque région mais impossible de le faire descendre à 0
                // Déterminer combien de nouveau citoyen arrivent infectés de l'étranger
                float incomingCitizen = Random.Range(0f, 1f);
                if (incomingCitizen < frontierPermeability.permeability * territoryData.populationRatio)
                    AddNewInfections(territoryData, 1);

                //////////////////////////////////////
                // INFECTION
                //////////////////////////////////////
                // Parcourir chaque "cluster" contagieux et déterminer le nombre de personnes contaminées
                // Calcul du nombre de taux de personnes infectées
                int livingInfected = territoryData.nbInfected - territoryData.nbDeath;
                int livingPop = territoryData.nbPopulation - territoryData.nbDeath;
                float infectedRatio = (float)livingInfected / livingPop;

                float newContagiosity = polyA * infectedRatio * infectedRatio + polyB * infectedRatio + polyC;

                if (newContagiosity > 0)
                {
                    // Détermination du nombre de nouveaux infectés
                    float newInfect = 0;
                    // On calcule les infections faites donc on regarde combien on a de personne infectée pour chaque jour de la fenêtre
                    for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                    {
                        // calcule le nouveau R0 (application de bonus et malus) du territoire en fonction de l'age et de la contagiosité théorique
                        float ageR0 = ComputeAgeR0(territoryData, age, newContagiosity);
                        for (int day = 1; day < territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length; day++) // on saute volontairement le premier élément puisqu'on est en train de le construire
                        {
                            // Calcul effectif du nombre de nouveaux infectés :
                            // R0 en fonction de l'age * le nombre de personne infecté pour l'age et le jour courrant * la probabilité de diffusion pour le jour courrant
                            newInfect += ageR0 * territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] * contagiousnessProbabilityPerDays[day];
                        }
                    }
                    AddNewInfections(territoryData, (int)Mathf.Round(newInfect));
                }

                //////////////////////////////////////
                // TRANSFERTS INTER-REGIONS
                //////////////////////////////////////
                // Calcul du nombre de personnes actuellement infectées
                int currentlyInfected = 0;
                for (int day = 0; day < territoryData.numberOfInfectedPeoplePerDays.Length; day++)
                    currentlyInfected += territoryData.numberOfInfectedPeoplePerDays[day];
                // Détermination de la région cible
                TerritoryData targetTerritory = f_territories.getAt(Random.Range(0, f_territories.Count)).GetComponent<TerritoryData>();
                // Calcul du nombre de personne qui change de région => dépend du type de confinement de la région de départ et de la région d'arrivée
                int movingPeoples = Mathf.Min((int)(Random.Range(0f, 0.001f) * currentlyInfected * (targetTerritory.certificateRequired ? 0.1f : 1f) * (territoryData.certificateRequired ? 0.1f : 1f)), 500); // 0.1% des personnes infectées bougent pour un maximum de 500
                if (movingPeoples > 0)
                {
                    // retirer le nombre de personne de la région de départ
                    territoryData.nbInfected -= movingPeoples;
                    territoryData.nbPopulation -= movingPeoples;
                    targetTerritory.nbInfected += movingPeoples;
                    targetTerritory.nbPopulation += movingPeoples;
                    // détermination des ages possibles pour le transfert
                    List<int> availableAges = new List<int>();
                    for (int age = 0; age < territoryData.popNumber.Length; age++)
                    {
                        // Vérifier s'il y a bien des personnes infectées pour cette tranche d'age et que cet age n'est pas concerné par une interdiction de sortie
                        if (territoryData.popInfected[age] > 0 && !(territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && age >= int.Parse(territoryData.ageDependentMin) && age <= int.Parse(territoryData.ageDependentMax)))
                            availableAges.Add(age);
                    }

                    while (movingPeoples > 0 && availableAges.Count > 0)
                    {
                        int age = availableAges[Random.Range(0, availableAges.Count)];
                        int day = Random.Range(0, territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length);
                        int movingStep = Random.Range(0, Mathf.Min(new int[4] { territoryData.popInfected[age], movingPeoples, territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day], 50 }) + 1); // Maximum de 50 affecté à chaque tirage pour éviter d'avoir trop de personnes d'une même classe d'age qui bouge... pas réaliste
                        territoryData.popInfected[age] -= movingStep;
                        territoryData.popNumber[age] -= movingStep;
                        territoryData.numberOfInfectedPeoplePerDays[day] -= movingStep;
                        territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= movingStep;
                        targetTerritory.popInfected[age] += movingStep;
                        targetTerritory.popNumber[age] += movingStep;
                        targetTerritory.numberOfInfectedPeoplePerDays[day] += movingStep;
                        targetTerritory.numberOfInfectedPeoplePerAgesAndDays[age][day] += movingStep;
                        movingPeoples -= movingStep;

                        // mise à jour des ages disponibles
                        availableAges.Clear();
                        for (int i = 0; i < territoryData.popNumber.Length; i++)
                            if (territoryData.popInfected[i] > 0)
                                availableAges.Add(i);
                    }
                }
            }
        }
    }

    private float ComputeAgeR0(TerritoryData territoryData, int age, float theoriticalR0)
    {

        float contextBonus = 1;
        // prise en compte du confinement des écoles
        if (territoryData.closePrimarySchool || territoryData.closeSecondarySchool || territoryData.closeHighSchool || territoryData.closeUniversity)
        {
            float schoolImpact = 1;
            if (territoryData.closePrimarySchool && age >= 3 && age <= 11)
                schoolImpact *= (!remoteworking.currentState ? 0.5f : 1f); // 100% d'une classe d'age va à l'école primaire mais pour les petits dépend aussi des mesures sociales, sans quoi l'effet est minoré dû à la nécessite de recourir à des gardes collectives => 50% des enfants auront des problèmes de garde
            else if (territoryData.closeSecondarySchool && age > 11 && age <= 15)
                schoolImpact *= (!remoteworking.currentState ? 0.8f : 1f); // 100% d'une classe d'age va au collège mais pour les pré ados dépend aussi des mesures sociales, sans quoi l'effet est minoré dû à la nécessite de recourir à des gardes collectives => 20% des enfants auront des problèmes de garde (moins que pour l'école car on peut considérer que certains collégiens peuvent se garder seuls)
            else if (territoryData.closeHighSchool && age > 15 && age <= 18)
                schoolImpact *= 0.7f; // 70% d'une classe d'age va au lycée
            else if (territoryData.closeUniversity && age > 18 && age <= 20)
                schoolImpact *= 0.5f; // 50% d'une classe d'age a un bac+2
            else if (territoryData.closeUniversity && age > 20 && age <= 23)
                schoolImpact *= 0.3f; // 30% d'une classe d'age a plus d'un bac+2
            else
                schoolImpact = 0; // age non concerné => pas de bonus dû à la fermetures des écoles
            contextBonus -= confinementImpact.schoolImpact * schoolImpact;
        }

        // Prise en compte de l'appel civique
        if (territoryData.callCivicism)
            contextBonus -= confinementImpact.civicismImpact * Mathf.Min(1, (float)age / 25); // modérer l'impact en fonction de l'age : progression linéaire de 0% d'efficacité pour les tous petits jusqu'à 100% à partir de 25 ans

        // Prise en compte de la fermeture des commerces
        if (territoryData.closeShop)
            contextBonus -= confinementImpact.shopImpact * Mathf.Min(1, (float)age / 16); // modérer l'impact en fonction de l'age : progression linéaire de 0% d'efficacité pour les tous petits jusqu'à 100% à partir de 16 ans

        // Prise en compte de l'attestation
        if (territoryData.certificateRequired)
            contextBonus -= confinementImpact.attestationImpact;

        // prise en compte de la restriction d'age
        if (territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && territoryData.ageDependentMin != "--" && territoryData.ageDependentMax != "--" && age >= int.Parse(territoryData.ageDependentMin) && age <= int.Parse(territoryData.ageDependentMax))
            contextBonus -= confinementImpact.ageRestrictionImpact;

        // IMPACT DES MESURES NATIONALES

        // Prise en compte de la disponibilité des masques
        // Pour le personnel hospitalier => si pénurie: malus
        if (masks.medicalRequirementPerDay_current > masks.nationalStock)
            contextBonus += 1f - 1 / (masks.medicalRequirementPerDay_current - masks.nationalStock);

        // prise en compte du télétravail
        if (remoteworking.currentState && age > 18 && age < 62)
            contextBonus -= confinementImpact.remoteWorkingImpact;

        // Production artisanale de masques
        if (masks.selfProtectionPromoted)
            contextBonus -= confinementImpact.selfProtectionImpact * Mathf.Min(1, 0.1f + (float)age / 10); // modérer l'impact en fonction de l'age : progression linéaire de 10% d'efficacité pour les tous petits jusqu'à 100% à partir de 8 ans

        return theoriticalR0 * contextBonus;
    }

    // Calcule le R0 d'un territoire
    private float ComputeR0(TerritoryData territoryData)
    {
        // Calcul du ratio de personnes infectés parmis les vivants
        int livingInfected = territoryData.nbInfected - territoryData.nbDeath;
        int livingPop = territoryData.nbPopulation - territoryData.nbDeath;
        float infectedRatio = (float)livingInfected / livingPop;
        // Calcul du R0 théorique
        float theoreticalR0 = polyA * infectedRatio * infectedRatio + polyB * infectedRatio + polyC;

        if (theoreticalR0 > 0)
        {
            float r0Cumul = 0;
            // calcule le nouveau R0 (application de bonus et malus) du territoire en fonction de l'age et de la contagiosité théorique
            for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                // aggréger la valeur du R0 pondéré par le nombre de personnes vivante pour l'age considéré
                r0Cumul += ComputeAgeR0(territoryData, age, theoreticalR0) * (territoryData.popNumber[age] - territoryData.popDeath[age]);
            // retourne la moyenne de R0 pour le territoire
            return Mathf.Max(0, r0Cumul / livingPop);
        }
        else
            return 0;
    }

    private void AddNewInfections(TerritoryData territory, int amountOfNewInfections)
    {
        if (amountOfNewInfections >= 1 && !firstInfection)
        {
            GameObjectManager.addComponent<ChatMessage>(countryPopData.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorHealthTexts[13], territory.TerritoryName) });
            firstInfection = true;
        }
        // Calcul du nombre total de pesonnes non infectées
        int totalLivingAndNotInfected = 0;
        List<int> availableAges = new List<int>();
        for (int age = 0; age < territory.popNumber.Length; age++)
        {
            // Vérifier si il reste des personnes non infectées pour cette tranche d'age
            int notInfected = territory.popNumber[age] - territory.popInfected[age];
            if (notInfected > 0) {
                totalLivingAndNotInfected += notInfected;
                availableAges.Add(age);
            }
        }
        // Répartir le capital de personnes infectées sur les différentes tranches d'ages disponible. Faire ça tant qu'il reste des infections à répartir ET qu'il y a des personnes non infectées disponibles
        while (amountOfNewInfections > 0 && totalLivingAndNotInfected > 0 && availableAges.Count > 0)
        {
            int age = availableAges[Random.Range(0, availableAges.Count)];
            int notInfected = territory.popNumber[age] - territory.popInfected[age];
            int gapInfected = 0;

            // croissance de 2% des personnes qui n'ont pas encore été en contact avec le virus avec un maximum proportionnel à la représentativité de la tranche d'age
            gapInfected = Mathf.Max((int)Mathf.Round(Mathf.Min(notInfected * 0.02f, 1000*territory.popNumber[age]/territory.maxNumber)), 1); // être sûr d'avoir au moins un nouvel infecté
            // Le nombre réel de personnes infectées et donc un tirage aléatoire entre 0 et le minimum de gapInfected, amountOfNewInfection, totalLivingAndNotInfected et notInfected
            int newInfected = Random.Range(0, Mathf.Min(gapInfected, amountOfNewInfections, totalLivingAndNotInfected, notInfected) +1);

            // Prise en compte de la protection
            float protection = 1f - GetProtection(age, territory);
            int ignoredInfections = Mathf.RoundToInt(newInfected * (1f - protection));
            newInfected = Mathf.RoundToInt(newInfected * protection);

            // Prise en compte des infections au niveau du territoire
            territory.nbInfected += newInfected;
            territory.popInfected[age] += newInfected;
            territory.numberOfInfectedPeoplePerDays[0] += newInfected;
            territory.numberOfInfectedPeoplePerAgesAndDays[age][0] += newInfected;
            // Prise en compte des infections au niveau national
            countryPopData.nbInfected += newInfected;
            countryPopData.popInfected[age] += newInfected;
            countryPopData.numberOfInfectedPeoplePerDays[0] += newInfected;
            countryPopData.numberOfInfectedPeoplePerAgesAndDays[age][0] += newInfected;

            amountOfNewInfections -= newInfected + ignoredInfections;
            totalLivingAndNotInfected -= newInfected;
            // mise à jour des ages disponibles
            availableAges.Clear();
            for (int age2 = 0; age2 < territory.popNumber.Length; age2++)
                if (territory.popNumber[age2] - territory.popInfected[age2] > 0)
                    availableAges.Add(age2);
        }
    }

    private float GetProtection(int age, TerritoryData territory)
    {
        if (territory.closePrimarySchool && age <= 11 || // Vérifier écoles confinées
            territory.closeSecondarySchool && age > 11 && age <= 15) // Vérifier collèges confinés
            return 0.8f;
        else if (territory.closeHighSchool && age > 15 && age <= 18) // Vérifier lycées confinés 
            return 0.7f; // 70 % des lycéens => certains ont déjà quitté le circuit scolaire
        else if (territory.closeUniversity && age > 18 && age <= 20) // Vérifier universités confinées
            return 0.5f; // 50 % d'une classe d'age a un bac+2
        else if (territory.closeUniversity && age > 20 && age <= 23) // Vérifier universités confinées
            return 0.3f; // 30% d'une classe d'age a plus d'un bac+2
        else if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "" && age >= int.Parse(territory.ageDependentMin) && age <= int.Parse(territory.ageDependentMax))
            return 0.8f;
        else
            return 0;
    }

    public void UpdatePopRatioInfectedUI(TMPro.TMP_Text textUI)
    {
        textUI.text = (100f * MapSystem.territorySelected.nbInfected / MapSystem.territorySelected.nbPopulation).ToString("N0") + "%";
    }

    public void UpdateR0UI(TMPro.TMP_Text textUI)
    {
        float R0 = 0;
        if (MapSystem.territorySelected.TerritoryName == countryPopData.TerritoryName)
        {
            // Calcul du R0 national à partir des R0 territoriaux
            float r0Cumul_national = 0;
            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                // aggréger la valeur du R0 pondéré par le nombre de personnes vivante dans ce territoire
                r0Cumul_national += ComputeR0(territoryData) * (territoryData.nbPopulation - territoryData.nbDeath) / (countryPopData.nbPopulation - countryPopData.nbDeath);
            }
            // calcul de la moyenne des R0 au niveau national
            R0 = r0Cumul_national ;
        }
        else
            R0 = ComputeR0(MapSystem.territorySelected);
        textUI.text = "R0 : "+R0.ToString("N2", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
    }
}