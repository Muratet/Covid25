using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Collections.Generic;

/// <summary>
/// This system calculates the infection between peoples
/// </summary>
public class InfectionSystem : FSystem
{
    private Family f_territoriesAndCountry = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData)));
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    /// <summary></summary>
    public GameObject countrySimData;
    private VirusStats virusStats;
    private TerritoryData countryPopData;
    private TimeScale time;
    private FrontierPermeability frontierPermeability;
    private Remoteworking remoteworking;
    private Masks masks;
    private InfectionImpact confinementImpact;

    /// <summary></summary>
    public Localization localization;

    // model contagiousness during X days
    private float[] contagiousnessProbabilityPerDays;

    private float polyA;
    private float polyB;
    private float polyC;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static InfectionSystem instance;

    private bool firstInfection = false;

    /// <summary>
    /// Construct this system
    /// </summary>
    public InfectionSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Recovery of virus data
        virusStats = countrySimData.GetComponent<VirusStats>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery masks data
        masks = countrySimData.GetComponent<Masks>();
        // Recovery of infection impact data
        confinementImpact = countrySimData.GetComponent<InfectionImpact>();
        // Recovery borders permeability
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Recovery of homeworking data
        remoteworking = countrySimData.GetComponent<Remoteworking>();

        // calculation of the contagiousness curve for a window of days
        contagiousnessProbabilityPerDays = new float[virusStats.windowSize];
        float peak = virusStats.contagiousnessPeak;
        float deviation = virusStats.contagiousnessDeviation;
        for (int i = 0; i < contagiousnessProbabilityPerDays.Length; i++)
            contagiousnessProbabilityPerDays[i] = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation));

        foreach (GameObject territory in f_territoriesAndCountry)
            initTerritoryInfectionData(territory);
        f_territories.addEntryCallback(initTerritoryInfectionData);

        // To determine the contagiousness of the virus, we must find the polynomial that passes through three points:
        //   - if % population infected == 0 => contagiousness by default of the virus
        //   - if % population infected == % immunity => contagiousness == 1
        //   - if % population infected == 1 => contagiousness == 0
        // We have to find the values a, b and c of the polynomial aX²+bX + c = Y with X <=> % infected population and Y the final contagiousness
        // So we have to solve the system
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
        // Consideration of borderline cases
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

    private void initTerritoryInfectionData(GameObject go)
    {
        TerritoryData territoryData = go.GetComponent<TerritoryData>();
        // Initialization of the number of infected for each day of the window
        territoryData.numberOfInfectedPeoplePerDays = new int[virusStats.windowSize];
        for (int day = 0; day < virusStats.windowSize; day++)
            territoryData.numberOfInfectedPeoplePerDays[day] = 0;
        // Initialization of the number of infected for each age and for each day of the window
        territoryData.numberOfInfectedPeoplePerAgesAndDays = new int[territoryData.popNumber.Length][];
        for (int age = 0; age < territoryData.popNumber.Length; age++)
        {
            territoryData.numberOfInfectedPeoplePerAgesAndDays[age] = new int[virusStats.windowSize];
            for (int day = 0; day < virusStats.windowSize; day++)
                territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] = 0;
        }
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        // Check if a new day should be generated
        if (time.newDay)
        {
            // Process each territory
            TerritoryData territoryData;

            foreach (GameObject territory in f_territories)
            {
                territoryData = territory.GetComponent<TerritoryData>();
                //////////////////////////////////////
                // EMERGENCE
                //////////////////////////////////////
                // Calculation of the probability of occurrence of a new first patient => depends on the opening of the borders and the density of each region but impossible to make it go down to 0
                // Determine how many new citizens arrive infected from abroad
                float incomingCitizen = Random.Range(0f, 1f);
                if (incomingCitizen < frontierPermeability.permeability * territoryData.populationRatio)
                    AddNewInfections(territoryData, 1);

                //////////////////////////////////////
                // INFECTION
                //////////////////////////////////////
                // Walk through each contagious "cluster" and determine the number of infected people
                // Calculation of the number of infected persons
                int livingInfected = territoryData.nbInfected - territoryData.nbDeath;
                int livingPop = territoryData.nbPopulation - territoryData.nbDeath;
                float infectedRatio = (float)livingInfected / livingPop;

                float newContagiosity = polyA * infectedRatio * infectedRatio + polyB * infectedRatio + polyC;

                if (newContagiosity > 0)
                {
                    // Determining the number of new infections
                    float newInfect = 0;
                    // We calculate the infections made so we see how many people are infected for each day of the window
                    for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                    {
                        // calculates the new R0 (application of bonus and penalties) of the territory according to the age and the theoretical contagiousness
                        float ageR0 = ComputeAgeR0(territoryData, age, newContagiosity);
                        for (int day = 1; day < territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length; day++) // we voluntarily skip the first element since we are building it
                        {
                            // Actual calculation of the number of newly infected:
                            // R0 of the age * the number of infected persons for the age and the current day * the probability of diffusion for the current day
                            newInfect += ageR0 * territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] * contagiousnessProbabilityPerDays[day];
                        }
                    }
                    AddNewInfections(territoryData, (int)Mathf.Round(newInfect));
                }

                //////////////////////////////////////
                // INTER-REGIONAL TRANSFERS
                //////////////////////////////////////
                // Calculating the number of people currently infected
                int currentlyInfected = 0;
                for (int day = 0; day < territoryData.numberOfInfectedPeoplePerDays.Length; day++)
                    currentlyInfected += territoryData.numberOfInfectedPeoplePerDays[day];
                // Determination of the target region
                TerritoryData targetTerritory = f_territories.getAt(Random.Range(0, f_territories.Count)).GetComponent<TerritoryData>();
                // Calculation of the number of people changing regions => depends on the type of containment in the region of departure and the region of arrival
                int movingPeoples = Mathf.Min((int)(Random.Range(0f, 0.001f) * currentlyInfected * (targetTerritory.certificateRequired ? 0.1f : 1f) * (territoryData.certificateRequired ? 0.1f : 1f)), 500); // 0.1% of infected persons move for a maximum of 500
                if (movingPeoples > 0)
                {
                    // remove the number of people from the starting region
                    territoryData.nbInfected -= movingPeoples;
                    territoryData.nbPopulation -= movingPeoples;
                    targetTerritory.nbInfected += movingPeoples;
                    targetTerritory.nbPopulation += movingPeoples;
                    // determination of the possible ages for the transfer
                    List<int> availableAges = new List<int>();
                    for (int age = 0; age < territoryData.popNumber.Length; age++)
                    {
                        // Check to see if there are any infected people in this age group and that this age group is not affected by an exit ban
                        if (territoryData.popInfected[age] > 0 && !(territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && age >= int.Parse(territoryData.ageDependentMin) && age <= int.Parse(territoryData.ageDependentMax)))
                            availableAges.Add(age);
                    }

                    while (movingPeoples > 0 && availableAges.Count > 0)
                    {
                        int age = availableAges[Random.Range(0, availableAges.Count)];
                        int day = Random.Range(0, territoryData.numberOfInfectedPeoplePerAgesAndDays[age].Length);
                        int movingStep = Random.Range(0, Mathf.Min(new int[4] { territoryData.popInfected[age], movingPeoples, territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day], 50 }) + 1); // Maximum of 50 affected at each draw to avoid having too many people of the same age group traveling... not realistic
                        territoryData.popInfected[age] -= movingStep;
                        territoryData.popNumber[age] -= movingStep;
                        territoryData.numberOfInfectedPeoplePerDays[day] -= movingStep;
                        territoryData.numberOfInfectedPeoplePerAgesAndDays[age][day] -= movingStep;
                        targetTerritory.popInfected[age] += movingStep;
                        targetTerritory.popNumber[age] += movingStep;
                        targetTerritory.numberOfInfectedPeoplePerDays[day] += movingStep;
                        targetTerritory.numberOfInfectedPeoplePerAgesAndDays[age][day] += movingStep;
                        movingPeoples -= movingStep;

                        // update of available ages
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
        // consideration of school containment
        if (territoryData.closePrimarySchool || territoryData.closeSecondarySchool || territoryData.closeHighSchool || territoryData.closeUniversity)
        {
            float schoolImpact = 1;
            if (territoryData.closePrimarySchool && age >= 3 && age <= 11)
                schoolImpact *= (!remoteworking.currentState ? 0.5f : 1f); // 100% of a class of age goes to primary school but for the little ones depends also on social measures, without which the effect is reduced due to the need to resort to collective care => 50% of the children will have problems of care
            else if (territoryData.closeSecondarySchool && age > 11 && age <= 15)
                schoolImpact *= (!remoteworking.currentState ? 0.8f : 1f); // 100% of an age group goes to middle school, but for pre-adolescents it also depends on social measures, otherwise the effect is reduced due to the need for collective childcare => 20% of children will have childcare problems (less than for school, because it can be considered that some middle school students can look after themselves)
            else if (territoryData.closeHighSchool && age > 15 && age <= 18)
                schoolImpact *= 0.7f; // 70% of an age group goes to high school
            else if (territoryData.closeUniversity && age > 18 && age <= 20)
                schoolImpact *= 0.5f; // 50% of a class of age has a baccalaureate+2
            else if (territoryData.closeUniversity && age > 20 && age <= 23)
                schoolImpact *= 0.3f; // 30% of the age group has more than 2 years of higher education
            else
                schoolImpact = 0; // age not concerned => no bonus due to school closure
            contextBonus -= confinementImpact.schoolImpact * schoolImpact;
        }

        // Consideration of the civic call
        if (territoryData.callCivicism)
            contextBonus -= confinementImpact.civicismImpact * Mathf.Min(1, (float)age / 25); // moderating the impact according to age: linear progression from 0% efficiency for the very young to 100% from 25 years old

        // Consideration of shop closures
        if (territoryData.closeShop)
            contextBonus -= confinementImpact.shopImpact * Mathf.Min(1, (float)age / 16); // moderating the impact according to age: linear progression from 0% effectiveness for the very young to 100% from age 16

        // Consideration of the certificate
        if (territoryData.certificateRequired)
            contextBonus -= confinementImpact.attestationImpact;

        // consideration of age restriction
        if (territoryData.ageDependent && territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "" && territoryData.ageDependentMin != "--" && territoryData.ageDependentMax != "--" && age >= int.Parse(territoryData.ageDependentMin) && age <= int.Parse(territoryData.ageDependentMax))
            contextBonus -= confinementImpact.ageRestrictionImpact;

        // IMPACT OF NATIONAL MEASURES

        // Taking into account the availability of masks
        // For hospital staff => if shortage: penalty
        if (masks.medicalRequirementPerDay_current > masks.nationalStock)
            contextBonus += 1f - 1 / (masks.medicalRequirementPerDay_current - masks.nationalStock);

        // consideration of home working
        if (remoteworking.currentState && age > 18 && age < 62)
            contextBonus -= confinementImpact.remoteWorkingImpact;

        // Artisanal production of masks
        if (masks.selfProtectionPromoted)
            contextBonus -= confinementImpact.selfProtectionImpact * Mathf.Min(1, 0.1f + (float)age / 10); // moderating the impact according to age: linear progression from 10% efficiency for the very young to 100% from 8 years old

        return theoriticalR0 * contextBonus;
    }

    // Calculates the R0 of a territory
    private float ComputeR0(TerritoryData territoryData)
    {
        // Calculation of the ratio of infected persons among the living
        int livingInfected = territoryData.nbInfected - territoryData.nbDeath;
        int livingPop = territoryData.nbPopulation - territoryData.nbDeath;
        float infectedRatio = (float)livingInfected / livingPop;
        // Calculation of the theoretical R0
        float theoreticalR0 = polyA * infectedRatio * infectedRatio + polyB * infectedRatio + polyC;

        if (theoreticalR0 > 0)
        {
            float r0Cumul = 0;
            // calculates the new R0 (application of bonus and penalty) of the territory according to the age and the theoretical contagiousness
            for (int age = 0; age < territoryData.numberOfInfectedPeoplePerAgesAndDays.Length; age++)
                // Aggregate the value of the R0 weighted by the number of living persons for the age considered
                r0Cumul += ComputeAgeR0(territoryData, age, theoreticalR0) * (territoryData.popNumber[age] - territoryData.popDeath[age]);
            // returns the average of R0 for the territory
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
        // Calculation of the total number of uninfected persons
        int totalLivingAndNotInfected = 0;
        List<int> availableAges = new List<int>();
        for (int age = 0; age < territory.popNumber.Length; age++)
        {
            // Check to see if there are any uninfected people in this age group
            int notInfected = territory.popNumber[age] - territory.popInfected[age];
            if (notInfected > 0) {
                totalLivingAndNotInfected += notInfected;
                availableAges.Add(age);
            }
        }
        // Distribute the pool of infected people to the different age groups available. Do this as long as there are infections to be distributed AND there are uninfected people available
        while (amountOfNewInfections > 0 && totalLivingAndNotInfected > 0 && availableAges.Count > 0)
        {
            int age = availableAges[Random.Range(0, availableAges.Count)];
            int notInfected = territory.popNumber[age] - territory.popInfected[age];

            // 2% increase in people who have not yet been in contact with the virus, with a maximum proportional to the representativeness of the age group
            int gapInfected = Mathf.Max((int)Mathf.Round(Mathf.Min(notInfected * 0.02f, 1000*territory.popNumber[age]/territory.maxNumber)), 1); // be sure to have at least one new infected person
            // The actual number of infected people is a random draw between 0 and the minimum of gapInfected, amountOfNewInfection, totalLivingAndNotInfected and notInfected
            int newInfected = Random.Range(0, Mathf.Min(gapInfected, amountOfNewInfections, totalLivingAndNotInfected, notInfected) +1);

            // Consideration of protection
            float protection = 1f - GetProtection(age, territory);
            int ignoredInfections = Mathf.RoundToInt(newInfected * (1f - protection));
            newInfected = Mathf.RoundToInt(newInfected * protection);

            // Consideration of infections at the territory level
            territory.nbInfected += newInfected;
            territory.popInfected[age] += newInfected;
            territory.numberOfInfectedPeoplePerDays[0] += newInfected;
            territory.numberOfInfectedPeoplePerAgesAndDays[age][0] += newInfected;
            // Consideration of infections at the national level
            countryPopData.nbInfected += newInfected;
            countryPopData.popInfected[age] += newInfected;
            countryPopData.numberOfInfectedPeoplePerDays[0] += newInfected;
            countryPopData.numberOfInfectedPeoplePerAgesAndDays[age][0] += newInfected;

            amountOfNewInfections -= newInfected + ignoredInfections;
            totalLivingAndNotInfected -= newInfected;
            // update of available ages
            availableAges.Clear();
            for (int age2 = 0; age2 < territory.popNumber.Length; age2++)
                if (territory.popNumber[age2] - territory.popInfected[age2] > 0)
                    availableAges.Add(age2);
        }
    }

    private float GetProtection(int age, TerritoryData territory)
    {
        if (territory.closePrimarySchool && age <= 11 || // Check confined primary schools
            territory.closeSecondarySchool && age > 11 && age <= 15) // Check confined middle schools
            return 0.8f;
        else if (territory.closeHighSchool && age > 15 && age <= 18) // Check confined high schools
            return 0.7f; // 70% of high school students => some have already left school
        else if (territory.closeUniversity && age > 18 && age <= 20) // Check confined universities
            return 0.5f; // 50 % of a class of age has a baccalaureate+2
        else if (territory.closeUniversity && age > 20 && age <= 23) // Check confined universities
            return 0.3f; // 30% of the age group has more than 2 years of higher education
        else if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "" && age >= int.Parse(territory.ageDependentMin) && age <= int.Parse(territory.ageDependentMax))
            return 0.8f;
        else
            return 0;
    }

    /// <summary>
    /// Updates in the IU the percentage of population that has been infected by the virus
    /// </summary>
    /// <param name="textUI"></param>
    public void UpdatePopRatioInfectedUI(TMPro.TMP_Text textUI)
    {
        textUI.text = (100f * MapSystem.territorySelected.nbInfected / MapSystem.territorySelected.nbPopulation).ToString("N0") + "%";
    }


    /// <summary>
    /// Updates in the IU the R0
    /// </summary>
    /// <param name="textUI"></param>
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