using UnityEngine;
using UnityEngine.UI;
using FYFY;

/// <summary>
/// This system simulated the population dissatisfaction
/// </summary>
public class RevolutionSystem : FSystem
{
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));
    
    /// <summary></summary>
    public GameObject countrySimData;
    private FrontierPermeability frontierPermeability;
    private TimeScale time;
    private Revolution revolution;
    private TerritoryData countryPopData;
    private Remoteworking remoteworking;
    private ShortTimeWorking shortTimeWorking;
    private Tax tax;
    private Masks masks;

    /// <summary></summary>
    public int firstNotifStep;
    private bool firstNotifStepFlag = false;
    /// <summary></summary>
    public int secondNotifStep;
    private bool secondNotifStepFlag = false;
    /// <summary></summary>
    public int thirdNotifStep;
    private bool thirdNotifStepFlag = false;
    /// <summary></summary>
    public int fourthNotifStep;
    private bool fourthNotifStepFlag = false;
    /// <summary></summary>
    public int fifthNotifStep;
    private bool fifthNotifStepFlag = false;

    /// <summary></summary>
    public Localization localization;

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery of border restrictions data
        frontierPermeability = countrySimData.GetComponent<FrontierPermeability>();
        // Recovery of dissatisfaction data
        revolution = countrySimData.GetComponent<Revolution>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();
        // Recovery of home working data
        remoteworking = countrySimData.GetComponent<Remoteworking>();
        // Recovery of partial unemployment data
        shortTimeWorking = countrySimData.GetComponent<ShortTimeWorking>();
        // Recovery of business charges
        tax = countrySimData.GetComponent<Tax>();
        // Recovery masks data
        masks = countrySimData.GetComponent<Masks>();
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Check if a new day should be generated
        if (time.newDay)
        {
            float newStress = 0;
            float timeMalus = 1;// +((float)time.daysGone / 30); // more and more critical population every 15 days
            // We close primary/middle/high scools and universities
            TerritoryData territory;
            float criticRatio = 0;
            foreach (GameObject territory_go in f_territories)
            {
                territory = territory_go.GetComponent<TerritoryData>();
                float territoryRatio = (float)territory.nbPopulation / countryPopData.nbPopulation;
                bool isCritic = territory.nbInfected - territory.nbTreated - territory.nbDeath > revolution.criticThreshold * territoryRatio;
                if (isCritic)
                    criticRatio++;
                // closing schools and university
                if (territory.closePrimarySchool && !isCritic)
                    newStress += Random.Range(0f, revolution.closePrimSchoolPenalty * territoryRatio) * timeMalus;
                if (territory.closeSecondarySchool && !isCritic)
                    newStress += Random.Range(0f, revolution.closeScdSchoolPenalty * territoryRatio) * timeMalus;
                if (territory.closeHighSchool && !isCritic) 
                    newStress += Random.Range(0f, revolution.closeHighSchoolPenalty * territoryRatio) * timeMalus;
                if (territory.closeUniversity && !isCritic) 
                    newStress += Random.Range(0f, revolution.closeUniversityPenalty * territoryRatio) * timeMalus;
                // primary/middle school closure + home working => stress of managing everything at home
                if ((territory.closePrimarySchool || territory.closeSecondarySchool) && remoteworking.currentState) 
                    newStress += Random.Range(0f, revolution.comboSchoolRemoteworkPenalty * territoryRatio) * timeMalus;

                if (territory.callCivicism && !isCritic)
                    newStress += Random.Range(0f, revolution.callCivicPenalty * territoryRatio) * timeMalus;

                // closing shops
                if (territory.closeShop && !isCritic)
                    newStress += Random.Range(0f, revolution.closeShopPenalty * territoryRatio) * timeMalus;
                // Restriction of freedoms
                if (territory.certificateRequired && !isCritic)
                    newStress += Random.Range(0f, revolution.certifRequiredPenalty * territoryRatio) * timeMalus;
                // Age restrictions
                if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "" && !isCritic)
                {
                    // calculation of the population in this territory
                    int amount = 0;
                    int min = int.Parse(territory.ageDependentMin);
                    int max = int.Parse(territory.ageDependentMax);
                    for (int i = min; i < max; i++)
                        amount += territory.popNumber[i];
                    newStress += Random.Range(0f, revolution.ageRestrictionPenalty * amount/countryPopData.nbPopulation) * timeMalus;
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
            // border closure
            if (frontierPermeability.currentState > 0 && !revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.closeFrontierPenalty) * timeMalus;
            else if (frontierPermeability.currentState == 0 && revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.openFrontierPenalty) * timeMalus;

            // home working => reduces stress
            if (remoteworking.currentState && revolution.nationalInfectionIsCritic)
                newStress -= Random.Range(0f, revolution.remoteworkingBonus);

            // partial unemployment => reduces stress
            if (shortTimeWorking.currentState && revolution.nationalInfectionIsCritic)
                newStress -= Random.Range(0f, revolution.shortTimeWorkBonus);

            // shop support
            if (!tax.currentState && revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.taxPenalty) * timeMalus;
            else if (tax.currentState && revolution.nationalInfectionIsCritic)
                newStress -= Random.Range(0f, revolution.taxSupportRequiredBonus);
            else if (tax.currentState && !revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.taxSupportNotRequiredPenalty);

            // requisition of masks
            if (masks.requisition && revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.maskRequisitionPenalty) * timeMalus * (2f - Mathf.Min(masks.nationalStock / (masks.medicalRequirementPerDay_current / 7), 1)); // 7 is for: do we have enough mask for one week
            if (masks.boostProduction && revolution.nationalInfectionIsCritic)
                newStress -= Random.Range(0f, revolution.maskBoostProdBonus);
            if (masks.selfProtectionPromoted && !revolution.nationalInfectionIsCritic)
                newStress += Random.Range(0f, revolution.maskSelfProtectPenalty) * timeMalus;

            // Adaptation of stress according to the slope of death
            int size = countryPopData.numberOfInfectedPeoplePerDays.Length;
            float coeffDir = Mathf.Max(-1, (float)(countryPopData.numberOfInfectedPeoplePerDays[Mathf.Max(0, size - 8)] - countryPopData.numberOfInfectedPeoplePerDays[size - 1]) / revolution.deathGradient);
            if (coeffDir > 0.2f)
                newStress += Random.Range(0f, revolution.deathIncreasePenalty * coeffDir);
            else
            {
                // Consideration of the no new death bonus
                newStress -= Random.Range(0f, revolution.deathStagnationBonus);
                // Taking into account the bonus if the number of deaths decreases
                if (coeffDir < 0)
                    newStress -= Random.Range(0f, revolution.deathDecreaseBonus * (-1 * coeffDir));
            }

            // consideration of the new stress
            revolution.stress += newStress;
            // maintain the stress between [0, 100]
            revolution.stress = Mathf.Max(0f, Mathf.Min(100f, revolution.stress));

            if (revolution.stress > firstNotifStep && !firstNotifStepFlag)
            {
                firstNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = localization.advisorInteriorTexts[4]+((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? localization.advisorInteriorTexts[5] : localization.advisorInteriorTexts[6]) });
            }
            else if (revolution.stress > secondNotifStep && !secondNotifStepFlag)
            {
                secondNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorInteriorTexts[7], thirdNotifStep.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo)) + ((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? localization.advisorInteriorTexts[8] : localization.advisorInteriorTexts[9]) });
            }
            else if (revolution.stress > thirdNotifStep && !thirdNotifStepFlag)
            {
                thirdNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorInteriorTexts[10], thirdNotifStep.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo)) + ((revolution.nationalInfectionIsCritic || criticRatio > 0.5f) ? localization.advisorInteriorTexts[11] : localization.advisorInteriorTexts[12]) });
            }
            else if (revolution.stress > fourthNotifStep && !fourthNotifStepFlag)
            {
                fourthNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = localization.advisorInteriorTexts[13] });
            }
            else if (revolution.stress > fifthNotifStep && !fifthNotifStepFlag)
            {
                fifthNotifStepFlag = true;
                GameObjectManager.addComponent<ChatMessage>(revolution.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = localization.getFormatedText(localization.advisorInteriorTexts[14], fifthNotifStep.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo)) });
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

    /// <summary>
    /// update of the dissatisfaction level in the UI
    /// </summary>
    /// <param name="textUI">New value</param>
    public void UpdateRevolutionUI(TMPro.TMP_Text textUI)
    {
        textUI.text = revolution.stress.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo) + "%";
        textUI.color = new Color(1f, 1f - revolution.stress/80, 1f - revolution.stress/80);
    }
}