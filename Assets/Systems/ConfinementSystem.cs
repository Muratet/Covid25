using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;

public class ConfinementSystem : FSystem {

    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    public GameObject countrySimData;
    private TerritoryData countryData;
    private Beds countryBeds;
    private TimeScale time;
    public Sprite defaultMark;
    public Sprite customMark;
    private Sprite ageMark;

    public Toggle closePrimarySchool_UIMaps;
    public Toggle closeSecondarySchool_UIMaps;
    public Toggle closeHighSchool_UIMaps;
    public Toggle closeUniversity_UIMaps;
    public Toggle callCivicism_UIMaps;
    public Toggle closeShop_UIMaps;
    public Toggle certificateRequired_UIMaps;
    public Toggle beds_UIMaps;
    public Toggle ageDependent_UIMaps;
    public TMP_InputField ageDependentMin_UIMaps;
    public TMP_InputField ageDependentMax_UIMaps;

    public Localization localization;

    private int noVision = 0;

    public static ConfinementSystem instance;

    public ConfinementSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Récupération des données de la population
        countryData = countrySimData.GetComponent<TerritoryData>();
        // Récupération des données de lits de réa
        countryBeds = countrySimData.GetComponent<Beds>();
        // Récupération de l'échelle de temps
        time = countrySimData.GetComponent<TimeScale>();
    }

    public void updateCountryUI()
    {
        // On initialise le national avec la première région
        TerritoryData territory = f_territories.First().GetComponent<TerritoryData>();
        Beds territoryBeds = f_territories.First().GetComponent<Beds>();
        countryData.closePrimarySchool = territory.closePrimarySchool;
        closePrimarySchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.closeSecondarySchool = territory.closeSecondarySchool;
        closeSecondarySchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.closeHighSchool = territory.closeHighSchool;
        closeHighSchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.closeUniversity = territory.closeUniversity;
        closeUniversity_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.callCivicism = territory.callCivicism;
        callCivicism_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.closeShop = territory.closeShop;
        closeShop_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.certificateRequired = territory.certificateRequired;
        certificateRequired_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        countryData.ageDependent = territory.ageDependent;
        ageDependent_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        ageDependent_UIMaps.interactable = true;
        ageMark = defaultMark;
        countryData.ageDependentMin = territory.ageDependentMin;
        ageDependentMin_UIMaps.text = territory.ageDependentMin;
        countryData.ageDependentMax = territory.ageDependentMax;
        ageDependentMax_UIMaps.text = territory.ageDependentMax;
        countryBeds.boostBeds = territoryBeds.boostBeds;
        beds_UIMaps.GetComponentsInChildren<Image>()[1].sprite = defaultMark;
        // On fusionne avec les autres territoires
        for (int i = 1; i < f_territories.Count; i++)
        {
            territory = f_territories.getAt(i).GetComponent<TerritoryData>();
            territoryBeds = f_territories.getAt(i).GetComponent<Beds>();
            if (countryData.closePrimarySchool != territory.closePrimarySchool)
            {
                countryData.closePrimarySchool = true;
                setToggleUI(closePrimarySchool_UIMaps, true, customMark);
            }
            if (countryData.closeSecondarySchool != territory.closeSecondarySchool)
            {
                countryData.closeSecondarySchool = true;
                setToggleUI(closeSecondarySchool_UIMaps, true, customMark);
            }
            if (countryData.closeHighSchool != territory.closeHighSchool)
            {
                countryData.closeHighSchool = true;
                setToggleUI(closeHighSchool_UIMaps, true, customMark);
            }
            if (countryData.closeUniversity != territory.closeUniversity)
            {
                countryData.closeUniversity = true;
                setToggleUI(closeUniversity_UIMaps, true, customMark);
            }
            if (countryData.callCivicism != territory.callCivicism)
            {
                countryData.callCivicism = true;
                setToggleUI(callCivicism_UIMaps, true, customMark);
            }
            if (countryData.closeShop != territory.closeShop)
            {
                countryData.closeShop = true;
                setToggleUI(closeShop_UIMaps, true, customMark);
            }
            if (countryData.certificateRequired != territory.certificateRequired)
            {
                countryData.certificateRequired = true;
                setToggleUI(certificateRequired_UIMaps, true, customMark);
            }
            if (countryData.ageDependent != territory.ageDependent)
            {
                countryData.ageDependent = true;
                ageMark = customMark;
            }
            if (countryData.ageDependentMin != territory.ageDependentMin)
            {
                countryData.ageDependentMin = "--";
                ageDependentMin_UIMaps.text = "--";
            }
            if (countryData.ageDependentMax != territory.ageDependentMax)
            {
                countryData.ageDependentMax = "--";
                ageDependentMax_UIMaps.text = "--";
            }
            if (countryBeds.boostBeds != territoryBeds.boostBeds)
            {
                countryBeds.boostBeds = true;
                setToggleUI(beds_UIMaps, true, customMark);
            }
        }
        if (countryData.ageDependentMin != "" && countryData.ageDependentMin != "--" && countryData.ageDependentMax != "" && countryData.ageDependentMax != "--")
        {
            ageDependent_UIMaps.interactable = true;
            setToggleUI(ageDependent_UIMaps, countryData.ageDependent, ageMark);
        }
        else
        {
            countryData.ageDependent = false;
            setToggleUI(ageDependent_UIMaps, false, defaultMark);
            ageDependent_UIMaps.interactable = false;
        }
        SyncUISystem.needUpdate = true;
    }

    private void setToggleUI(Toggle toggle, bool state, Sprite mark)
    {
        toggle.isOn = state;
        toggle.GetComponentsInChildren<Image>()[1].sprite = mark;
    }

    public void updateUI (TerritoryData territoryData, Beds territoryBeds)
    {
        setToggleUI(closePrimarySchool_UIMaps, territoryData.closePrimarySchool, defaultMark);
        setToggleUI(closeSecondarySchool_UIMaps, territoryData.closeSecondarySchool, defaultMark);
        setToggleUI(closeHighSchool_UIMaps, territoryData.closeHighSchool, defaultMark);
        setToggleUI(closeUniversity_UIMaps, territoryData.closeUniversity, defaultMark);
        setToggleUI(callCivicism_UIMaps, territoryData.callCivicism, defaultMark);
        setToggleUI(closeShop_UIMaps, territoryData.closeShop, defaultMark);
        setToggleUI(certificateRequired_UIMaps, territoryData.certificateRequired, defaultMark);
        setToggleUI(ageDependent_UIMaps, territoryData.ageDependent, defaultMark);
        ageDependentMin_UIMaps.text = territoryData.ageDependentMin;
        ageDependentMax_UIMaps.text = territoryData.ageDependentMax;
        if (territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "")
            ageDependent_UIMaps.interactable = true;
        else
            ageDependent_UIMaps.interactable = false;
        setToggleUI(beds_UIMaps, territoryBeds.boostBeds, defaultMark);
        SyncUISystem.needUpdate = true;
    }

    private void checkVision()
    {
        if (noVision > 10)
        {
            string messageChosen = "";
            switch (Random.Range(0, 4))
            {
                case 0: messageChosen = localization.advisorInteriorTexts[0]; break;
                case 1: messageChosen = localization.advisorInteriorTexts[1]; break;
                case 2: messageChosen = localization.advisorInteriorTexts[2]; break;
                case 3: messageChosen = localization.advisorInteriorTexts[3]; break;
            }
            GameObjectManager.addComponent<ChatMessage>(countryData.gameObject, new { sender = localization.advisorTitleInterior, timeStamp = "" + time.daysGone, messageBody = messageChosen });
            noVision = 0;
        }
    }

    public void OnPrimarySchoolChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.closePrimarySchool)
            {
                countryData.closePrimarySchool = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.closePrimarySchoolLastUpdate != 0 && time.daysGone - countryData.closePrimarySchoolLastUpdate < 5)
                    noVision++;
                countryData.closePrimarySchoolLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().closePrimarySchool = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.closePrimarySchool != newState)
            {
                MapSystem.territorySelected.closePrimarySchool = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.closePrimarySchoolLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.closePrimarySchoolLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.closePrimarySchoolLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnSecondarySchoolChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.closeSecondarySchool)
            {
                countryData.closeSecondarySchool = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.closeSecondarySchoolLastUpdate != 0 && time.daysGone - countryData.closeSecondarySchoolLastUpdate < 5)
                    noVision++;
                countryData.closeSecondarySchoolLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().closeSecondarySchool = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.closeSecondarySchool != newState)
            {
                MapSystem.territorySelected.closeSecondarySchool = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.closeSecondarySchoolLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.closeSecondarySchoolLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.closeSecondarySchoolLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnHighSchoolChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.closeHighSchool)
            {
                countryData.closeHighSchool = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.closeHighSchoolLastUpdate != 0 && time.daysGone - countryData.closeHighSchoolLastUpdate < 5)
                    noVision++;
                countryData.closeHighSchoolLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().closeHighSchool = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.closeHighSchool != newState)
            {
                MapSystem.territorySelected.closeHighSchool = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.closeHighSchoolLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.closeHighSchoolLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.closeHighSchoolLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnUniversityChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.closeUniversity)
            {
                countryData.closeUniversity = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.closeUniversityLastUpdate != 0 && time.daysGone - countryData.closeUniversityLastUpdate < 5)
                    noVision++;
                countryData.closeUniversityLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().closeUniversity = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.closeUniversity != newState)
            {
                MapSystem.territorySelected.closeUniversity = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.closeUniversityLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.closeUniversityLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.closeUniversityLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnCivicismChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.callCivicism)
            {
                countryData.callCivicism = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.callCivicismLastUpdate != 0 && time.daysGone - countryData.callCivicismLastUpdate < 5)
                    noVision++;
                countryData.callCivicismLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().callCivicism = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.callCivicism != newState)
            {
                MapSystem.territorySelected.callCivicism = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.callCivicismLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.callCivicismLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.callCivicismLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnShopChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.closeShop)
            {
                countryData.closeShop = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.closeShopLastUpdate != 0 && time.daysGone - countryData.closeShopLastUpdate < 5)
                    noVision++;
                countryData.closeShopLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().closeShop = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.closeShop != newState)
            {
                MapSystem.territorySelected.closeShop = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.closeShopLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.closeShopLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.closeShopLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnCertificateChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.certificateRequired)
            {
                countryData.certificateRequired = newState;
                // modification de l'action en moins de 5 jours
                if (countryData.certificateRequiredLastUpdate != 0 && time.daysGone - countryData.certificateRequiredLastUpdate < 5)
                    noVision++;
                countryData.certificateRequiredLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().certificateRequired = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.certificateRequired != newState)
            {
                MapSystem.territorySelected.certificateRequired = newState;
                // modification de l'action en moins de 5 jours
                if (MapSystem.territorySelected.certificateRequiredLastUpdate != 0 && time.daysGone - MapSystem.territorySelected.certificateRequiredLastUpdate < 5)
                    noVision++;
                MapSystem.territorySelected.certificateRequiredLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
        checkVision();
    }

    public void OnAgeDependentChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryData.ageDependent)
            {
                countryData.ageDependent = newState;
                countryData.ageDependentLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<TerritoryData>().ageDependent = newState;
            }
            updateCountryUI();
        }
        else
        {
            if (MapSystem.territorySelected.ageDependent != newState)
            {
                MapSystem.territorySelected.ageDependent = newState;
                MapSystem.territorySelected.ageDependentLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
    }

    public void OnAgeMinEndEdit(string newAge)
    {
        if (newAge != "--" && newAge != "-")
        {
            TerritoryData workingData;
            if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                workingData = countryData;
            else
                workingData = MapSystem.territorySelected;

            if (newAge != "" && int.Parse(newAge) > 100)
                newAge = "100";
            if (newAge != "" && int.Parse(newAge) < 0)
                newAge = "0";

            if ((workingData.ageDependentMax == "" || workingData.ageDependentMax == "--" || newAge == "" || int.Parse(newAge) <= int.Parse(workingData.ageDependentMax)) && newAge != workingData.ageDependentMin)
            {
                bool localChanged = false;
                if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                {
                    foreach (GameObject territory_go in f_territories)
                    {
                        TerritoryData local = territory_go.GetComponent<TerritoryData>();
                        if (workingData.ageDependentMax == "--")
                            local.ageDependentMax = "";

                        local.ageDependentMin = newAge;

                        if ((local.ageDependentMin == "" || local.ageDependentMax == "") && local.ageDependent)
                        {
                            local.ageDependent = false;
                            localChanged = true;
                        }
                    }
                }
                workingData.ageDependentMin = newAge;
                if ((workingData.ageDependentMin == "" || workingData.ageDependentMax == "" || workingData.ageDependentMin == "--" || workingData.ageDependentMax == "--") && (workingData.ageDependent || localChanged))
                {
                    workingData.ageDependent = false;
                    workingData.ageDependentLastUpdate = time.daysGone;
                }
            }

            if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                updateCountryUI();
            else
                updateUI(workingData, workingData.GetComponent<Beds>());
        }
    }

    public void OnAgeMaxEndEdit(string newAge)
    {
        if (newAge != "--" && newAge != "-")
        {
            TerritoryData workingData;
            if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                workingData = countryData;
            else
                workingData = MapSystem.territorySelected;

            if (newAge != "" && int.Parse(newAge) > 100)
                newAge = "100";
            if (newAge != "" && int.Parse(newAge) < 0)
                newAge = "0";

            if ((workingData.ageDependentMin == "" || workingData.ageDependentMin == "--" || newAge == "" || int.Parse(newAge) >= int.Parse(workingData.ageDependentMin)) && newAge != workingData.ageDependentMax)
            {
                bool localChanged = false;
                if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                    foreach (GameObject territory_go in f_territories)
                    {
                        TerritoryData local = territory_go.GetComponent<TerritoryData>();
                        if (workingData.ageDependentMin == "--")
                            local.ageDependentMin = "";
                        local.ageDependentMax = newAge;

                        if ((local.ageDependentMin == "" || local.ageDependentMax == "") && local.ageDependent)
                        {
                            local.ageDependent = false;
                            localChanged = true;
                        }
                    }
                workingData.ageDependentMax = newAge;
                if ((workingData.ageDependentMin == "" || workingData.ageDependentMax == "" || workingData.ageDependentMin == "--" || workingData.ageDependentMax == "--") && (workingData.ageDependent || localChanged))
                {
                    workingData.ageDependent = false;
                    workingData.ageDependentLastUpdate = time.daysGone;
                }
            }

            if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
                updateCountryUI();
            else
                updateUI(workingData, workingData.GetComponent<Beds>());
        }
    }

    public void OnBedsChange(bool newState)
    {
        if (MapSystem.territorySelected.TerritoryName == countryData.TerritoryName)
        {
            if (newState != countryBeds.boostBeds)
            {
                countryBeds.boostBeds = newState;
                // modification de l'action en moins de 5 jours
                if (countryBeds.boostBedsLastUpdate != 0 && time.daysGone - countryBeds.boostBedsLastUpdate < 5)
                    noVision++;
                countryBeds.boostBedsLastUpdate = time.daysGone;
                foreach (GameObject territory_go in f_territories)
                    territory_go.GetComponent<Beds>().boostBeds = newState;
            }
            updateCountryUI();
        }
        else
        {
            Beds beds = MapSystem.territorySelected.GetComponent<Beds>();
            if (beds.boostBeds != newState)
            {
                beds.boostBeds = newState;
                // modification de l'action en moins de 5 jours
                if (beds.boostBedsLastUpdate != 0 && time.daysGone - beds.boostBedsLastUpdate < 5)
                    noVision++;
                beds.boostBedsLastUpdate = time.daysGone;
            }
            updateUI(MapSystem.territorySelected, beds);
        }
        checkVision();
    }
}
