using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class MapSystem : FSystem {
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    public static TerritoryData territorySelected;

    public TMP_Text territoryName;
    public TimeScale time;
    public TextAsset rawContent;

    public GameObject territoryPrefab;
    public Transform territoriesParent;

    private TerritoryData countryData;

    public static MapSystem instance;

    private struct RawTerriroryData
    {
        public int id;
        public int bedsDefault;
        public int bedsHigh;
        public int [] agePyramid;
    }

    public MapSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        f_territories.addEntryCallback(onNewTerritory);

        GameObject countryToLoad = GameObject.Find("CountryToLoad");
        if (countryToLoad != null)
            rawContent = countryToLoad.GetComponent<CountryToLoad>().countryToLoad;

        //Load game content from the file
        Dictionary<string, RawTerriroryData> populationData = JsonConvert.DeserializeObject<Dictionary<string, RawTerriroryData>>(rawContent.text);
        // Réinitialiser les données de la nation pour être sûr de les synchroniser avec les données accumulées des régions
        countryData = time.GetComponent<TerritoryData>();
        for (int age = 0; age < countryData.popNumber.Length; age++)
            countryData.popNumber[age] = 0;
        countryData.id = -1;
        countryData.populationRatio = 1;
        countryData.nbPopulation = 0;
        Beds countryBeds = time.GetComponent<Beds>();
        countryBeds.intensiveBeds_default = 0;
        countryBeds.intensiveBeds_high = 0;

        int max = 0;
        // récupération des données de chaque territoire
        foreach (KeyValuePair<string, RawTerriroryData> entry in populationData)
        {
            if (entry.Value.id == -1)
                countryData.TerritoryName = entry.Key;
            else
            {
                GameObject newTerritory = GameObject.Instantiate(territoryPrefab, territoriesParent);
                GameObjectManager.bind(newTerritory);
                newTerritory.name = entry.Key;
                TerritoryData territoryData = newTerritory.GetComponent<TerritoryData>();
                territoryData.TerritoryName = entry.Key;
                territoryData.id = entry.Value.id;
                territoryData.popNumber = entry.Value.agePyramid;
                int total = 0;
                max = 0;
                for (int age = 0; age < territoryData.popNumber.Length; age++)
                {
                    total += territoryData.popNumber[age];
                    max = Mathf.Max(max, territoryData.popNumber[age]);
                    // accumulation au niveau national
                    countryData.popNumber[age] += territoryData.popNumber[age];
                }
                territoryData.nbPopulation = total;
                countryData.nbPopulation += total;
                // Calcul de la puissance de 10 immediatement supérieure au maximum
                territoryData.maxNumber = max - (max % 1000) + 1000;
                // chargement des données des lits
                Beds territoryBeds = newTerritory.GetComponent<Beds>();
                territoryBeds.intensiveBeds_default = entry.Value.bedsDefault;
                territoryBeds.intensiveBeds_high = entry.Value.bedsHigh;
                countryBeds.intensiveBeds_default += entry.Value.bedsDefault;
                countryBeds.intensiveBeds_high += entry.Value.bedsHigh;
                // Chargement des images
                newTerritory.GetComponent<Image>().sprite = Resources.Load<Sprite>(countryData.TerritoryName + "/Regions/" + territoryData.TerritoryName);
                newTerritory.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(countryData.TerritoryName + "/Regions/" + territoryData.TerritoryName + "_focused");
            }
        }

        max = 0;
        for (int age = 0; age < countryData.popNumber.Length; age++)
            max = Mathf.Max(max, countryData.popNumber[age]);
        countryData.maxNumber = max - (max % 1000) + 1000;

        territorySelected = countryData;
        territoryName.text = territorySelected.TerritoryName;
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Vérifier s'il faut générer une nouvelle journée
        if (time.newDay)
        {
            foreach (GameObject territory in f_territories)
            {
                TerritoryData territoryData = territory.GetComponent<TerritoryData>();
                Image img = territory.GetComponent<Image>();
                float ratio = 1f - 6*(float)(territoryData.nbInfected - territoryData.nbTreated - territoryData.nbDeath) / (territoryData.nbPopulation - territoryData.nbDeath);
                img.color = new Color(1f, ratio, ratio);
            }
        }
    }

    public void onNewTerritory(GameObject newTerritory)
    {
        TerritoryData territoryData = newTerritory.GetComponent<TerritoryData>();
        territoryData.populationRatio = (float)territoryData.nbPopulation / countryData.nbPopulation;
    }

    public void selectTerritory(TerritoryData newTerritory)
    {
        if (territorySelected.GetComponent<Animator>() && newTerritory != territorySelected)
            territorySelected.GetComponent<Animator>().Play("TerritoryIdle");
        if (newTerritory.GetComponent<Animator>())
            newTerritory.GetComponent<Animator>().Play("TerritorySelected");
        territorySelected = newTerritory;
        territoryName.text = territorySelected.TerritoryName;
        SyncUISystem.needUpdate = true;
    }

    public void onScroll(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData)data;
        if (ped != null)
        {
            territoriesParent.transform.localScale = new Vector3(territoriesParent.transform.localScale.x + ped.scrollDelta[1] / (20 / territoriesParent.transform.localScale.x), territoriesParent.transform.localScale.y + ped.scrollDelta[1] / (20 / territoriesParent.transform.localScale.y), territoriesParent.transform.localScale.z);
            if (territoriesParent.transform.localScale.x < 1)
                territoriesParent.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void onDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData)data;
        if (ped != null)
        {
            RectTransform rectTr = (RectTransform)territoriesParent.transform;
            // On touche le pivot que l'on maintient entre 0 et 1 en x et en y
            // pivot à [0,0] => carte poussée en haut à droite => effet de zoom en bas à gauche
            // pivot à [0,1] => carte poussée en bas à droite => effet de zoom en haut à gauche
            // ...
            // 300 est une constante magique !!!
            rectTr.pivot = new Vector2(rectTr.pivot.x - ped.delta[0] / (300 * rectTr.localScale.x), rectTr.pivot.y - ped.delta[1] / (300 * rectTr.localScale.y));
            if (rectTr.pivot.x < 0)
                rectTr.pivot = new Vector2(0, rectTr.pivot.y);
            if (rectTr.pivot.x > 1)
                rectTr.pivot = new Vector2(1, rectTr.pivot.y);
            if (rectTr.pivot.y < 0)
                rectTr.pivot = new Vector2(rectTr.pivot.x, 0);
            if (rectTr.pivot.y > 1)
                rectTr.pivot = new Vector2(rectTr.pivot.x, 1);
            // L'image se déplace à l'aide du pivot donc toujours la maintenir en [0, 0, 0]
            rectTr.localPosition = new Vector3(0, 0, 0);
        }
    }
}