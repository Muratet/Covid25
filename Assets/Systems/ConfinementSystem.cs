using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class ConfinementSystem : FSystem {

    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Beds), typeof(Image)));

    private TerritoryData countryData;
    private Beds countryBeds;
    private TimeScale time;
    private CustomCheckMarks customMarks;
    private Sprite ageMark;

    private int noVision = 0;

    public static ConfinementSystem instance;

    public ConfinementSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération des données de la population
        countryData = simu.GetComponent<TerritoryData>();
        countryBeds = simu.GetComponent<Beds>();
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();

        customMarks = GameObject.Find("CountryOptions_UI").GetComponent<CustomCheckMarks>();

        instance = this;
    }

    public void updateCountryUI()
    {
        // On initialise le national avec la première région
        TerritoryData territory = f_territories.First().GetComponent<TerritoryData>();
        Beds territoryBeds = f_territories.First().GetComponent<Beds>();
        countryData.closePrimarySchool = territory.closePrimarySchool;
        countryData.closePrimarySchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.closeSecondarySchool = territory.closeSecondarySchool;
        countryData.closeSecondarySchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.closeHighSchool = territory.closeHighSchool;
        countryData.closeHighSchool_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.closeUniversity = territory.closeUniversity;
        countryData.closeUniversity_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.callCivicism = territory.callCivicism;
        countryData.callCivicism_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.closeShop = territory.closeShop;
        countryData.closeShop_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.certificateRequired = territory.certificateRequired;
        countryData.certificateRequired_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.ageDependent = territory.ageDependent;
        countryData.ageDependent_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        countryData.ageDependent_UIMaps.interactable = true;
        ageMark = customMarks.defaultMark;
        countryData.ageDependentMin = territory.ageDependentMin;
        countryData.ageDependentMin_UIMaps.text = territory.ageDependentMin;
        countryData.ageDependentMax = territory.ageDependentMax;
        countryData.ageDependentMax_UIMaps.text = territory.ageDependentMax;
        countryBeds.boostBeds = territoryBeds.boostBeds;
        countryBeds.beds_UIMaps.GetComponentsInChildren<Image>()[1].sprite = customMarks.defaultMark;
        // On fusionne avec les autres territoires
        for (int i = 1; i < f_territories.Count; i++)
        {
            territory = f_territories.getAt(i).GetComponent<TerritoryData>();
            territoryBeds = f_territories.getAt(i).GetComponent<Beds>();
            if (countryData.closePrimarySchool != territory.closePrimarySchool)
            {
                countryData.closePrimarySchool = true;
                setToggleUI(countryData.closePrimarySchool_UIMaps, true, customMarks.customMark);
            }
            if (countryData.closeSecondarySchool != territory.closeSecondarySchool)
            {
                countryData.closeSecondarySchool = true;
                setToggleUI(countryData.closeSecondarySchool_UIMaps, true, customMarks.customMark);
            }
            if (countryData.closeHighSchool != territory.closeHighSchool)
            {
                countryData.closeHighSchool = true;
                setToggleUI(countryData.closeHighSchool_UIMaps, true, customMarks.customMark);
            }
            if (countryData.closeUniversity != territory.closeUniversity)
            {
                countryData.closeUniversity = true;
                setToggleUI(countryData.closeUniversity_UIMaps, true, customMarks.customMark);
            }
            if (countryData.callCivicism != territory.callCivicism)
            {
                countryData.callCivicism = true;
                setToggleUI(countryData.callCivicism_UIMaps, true, customMarks.customMark);
            }
            if (countryData.closeShop != territory.closeShop)
            {
                countryData.closeShop = true;
                setToggleUI(countryData.closeShop_UIMaps, true, customMarks.customMark);
            }
            if (countryData.certificateRequired != territory.certificateRequired)
            {
                countryData.certificateRequired = true;
                setToggleUI(countryData.certificateRequired_UIMaps, true, customMarks.customMark);
            }
            if (countryData.ageDependent != territory.ageDependent)
            {
                countryData.ageDependent = true;
                ageMark = customMarks.customMark;
            }
            if (countryData.ageDependentMin != territory.ageDependentMin)
            {
                countryData.ageDependentMin = "--";
                countryData.ageDependentMin_UIMaps.text = "--";
            }
            if (countryData.ageDependentMax != territory.ageDependentMax)
            {
                countryData.ageDependentMax = "--";
                countryData.ageDependentMax_UIMaps.text = "--";
            }
            if (countryBeds.boostBeds != territoryBeds.boostBeds)
            {
                countryBeds.boostBeds = true;
                setToggleUI(countryBeds.beds_UIMaps, true, customMarks.customMark);
            }
        }
        if (countryData.ageDependentMin != "" && countryData.ageDependentMin != "--" && countryData.ageDependentMax != "" && countryData.ageDependentMax != "--")
        {
            countryData.ageDependent_UIMaps.interactable = true;
            setToggleUI(countryData.ageDependent_UIMaps, countryData.ageDependent, ageMark);
        }
        else
        {
            countryData.ageDependent = false;
            setToggleUI(countryData.ageDependent_UIMaps, false, customMarks.defaultMark);
            countryData.ageDependent_UIMaps.interactable = false;
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
        setToggleUI(territoryData.closePrimarySchool_UIMaps, territoryData.closePrimarySchool, customMarks.defaultMark);
        setToggleUI(territoryData.closeSecondarySchool_UIMaps, territoryData.closeSecondarySchool, customMarks.defaultMark);
        setToggleUI(territoryData.closeHighSchool_UIMaps, territoryData.closeHighSchool, customMarks.defaultMark);
        setToggleUI(territoryData.closeUniversity_UIMaps, territoryData.closeUniversity, customMarks.defaultMark);
        setToggleUI(territoryData.callCivicism_UIMaps, territoryData.callCivicism, customMarks.defaultMark);
        setToggleUI(territoryData.closeShop_UIMaps, territoryData.closeShop, customMarks.defaultMark);
        setToggleUI(territoryData.certificateRequired_UIMaps, territoryData.certificateRequired, customMarks.defaultMark);
        setToggleUI(territoryData.ageDependent_UIMaps, territoryData.ageDependent, customMarks.defaultMark);
        territoryData.ageDependentMin_UIMaps.text = territoryData.ageDependentMin;
        territoryData.ageDependentMax_UIMaps.text = territoryData.ageDependentMax;
        if (territoryData.ageDependentMin != "" && territoryData.ageDependentMax != "")
            territoryData.ageDependent_UIMaps.interactable = true;
        else
            territoryData.ageDependent_UIMaps.interactable = false;
        setToggleUI(territoryBeds.beds_UIMaps, territoryBeds.boostBeds, customMarks.defaultMark);
        SyncUISystem.needUpdate = true;
    }

    private void checkVision()
    {
        if (noVision > 10)
        {
            string messageChosen = "";
            switch (Random.Range(0, 4))
            {
                case 0: messageChosen = "Vos changements de politique incessants sont perçues négativement par la population."; break;
                case 1: messageChosen = "L'opposition questionne votre politique et ça lisibilité."; break;
                case 2: messageChosen = "Il vous faut faire des choix et vous y tenir, les français ne comprennent pas votre politique."; break;
                case 3: messageChosen = "Les sondages montrent que les français attendent un cap."; break;
            }
            GameObjectManager.addComponent<ChatMessage>(countryData.gameObject, new { sender = "Ministre de l'intérieur", timeStamp = "" + time.daysGone, messageBody = messageChosen });
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