using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

/// <summary>
/// This system manage the map (zoom, moving, territory color)
/// </summary>
public class MapSystem : FSystem {
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    /// <summary></summary>
    public static TerritoryData territorySelected;

    /// <summary></summary>
    public TMP_Text territoryName;
    /// <summary></summary>
    public TimeScale time;
    /// <summary></summary>
    public TextAsset rawContent;

    /// <summary></summary>
    public GameObject territoryPrefab;
    /// <summary></summary>
    public Transform territoriesParent;

    private TerritoryData countryData;
    private Vector3 targetScale;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static MapSystem instance;

    private struct RawTerriroryData
    {
        public int id;
        public int bedsDefault;
        public int bedsHigh;
        public int [] agePyramid;
    }

    /// <summary>
    /// Construct this system
    /// </summary>
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

        // Load game content from the file
        Dictionary<string, RawTerriroryData> populationData = JsonConvert.DeserializeObject<Dictionary<string, RawTerriroryData>>(rawContent.text);
        // Reset the nation's data to make sure it is synchronized with the regions' accumulated data
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
        // recovery of data from each territory
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
                    // accumulation at the national level
                    countryData.popNumber[age] += territoryData.popNumber[age];
                }
                territoryData.nbPopulation = total;
                countryData.nbPopulation += total;
                // Calculation of the power of 10 immediately above the maximum
                territoryData.maxNumber = max - (max % 1000) + 1000;
                // loading of bed data
                Beds territoryBeds = newTerritory.GetComponent<Beds>();
                territoryBeds.intensiveBeds_default = entry.Value.bedsDefault;
                territoryBeds.intensiveBeds_high = entry.Value.bedsHigh;
                countryBeds.intensiveBeds_default += entry.Value.bedsDefault;
                countryBeds.intensiveBeds_high += entry.Value.bedsHigh;
                // Loading images
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
        targetScale = territoriesParent.transform.localScale;
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        // Check if a new day should be generated
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
        // smooth zoom
        if (Vector3.Distance(targetScale, territoriesParent.transform.localScale) >= 0.001f)
        {
            territoriesParent.transform.localScale = Vector3.MoveTowards(territoriesParent.transform.localScale, targetScale, Vector3.Distance(targetScale, territoriesParent.transform.localScale)/10);
            if (territoriesParent.transform.localScale.x < 1)
            {
                territoriesParent.transform.localScale = new Vector3(1, 1, 1);
                targetScale = territoriesParent.transform.localScale;
            }
        }
    }

    /// <summary>
    /// Compute population ratio for each new territory
    /// </summary>
    /// <param name="newTerritory"></param>
    public void onNewTerritory(GameObject newTerritory)
    {
        TerritoryData territoryData = newTerritory.GetComponent<TerritoryData>();
        territoryData.populationRatio = (float)territoryData.nbPopulation / countryData.nbPopulation;
    }

    /// <summary>
    /// Select a new territory
    /// </summary>
    /// <param name="newTerritory">The territory selected</param>
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

    /// <summary>
    /// Zoom in/out
    /// </summary>
    /// <param name="data"></param>
    public void onScroll(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData)data;
        if (ped != null)
            targetScale = new Vector3(territoriesParent.transform.localScale.x + ped.scrollDelta[1] / (3 / territoriesParent.transform.localScale.x), territoriesParent.transform.localScale.y + ped.scrollDelta[1] / (3 / territoriesParent.transform.localScale.y), territoriesParent.transform.localScale.z);
    }

    /// <summary>
    /// Move the map
    /// </summary>
    /// <param name="data"></param>
    public void onDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData)data;
        if (ped != null)
        {
            RectTransform rectTr = (RectTransform)territoriesParent.transform;
            // We touch the pivot that we maintain between 0 and 1 in x and y
            // pivot at [0,0] => map pushed to top right => zoom effect at bottom left
            // pivot at [0,1] => map pushed to bottom right => zoom effect at top left
            // ...
            // 300 is a magical constant!!!
            rectTr.pivot = new Vector2(rectTr.pivot.x - ped.delta[0] / (300 * rectTr.localScale.x), rectTr.pivot.y - ped.delta[1] / (300 * rectTr.localScale.y));
            if (rectTr.pivot.x < 0)
                rectTr.pivot = new Vector2(0, rectTr.pivot.y);
            if (rectTr.pivot.x > 1)
                rectTr.pivot = new Vector2(1, rectTr.pivot.y);
            if (rectTr.pivot.y < 0)
                rectTr.pivot = new Vector2(rectTr.pivot.x, 0);
            if (rectTr.pivot.y > 1)
                rectTr.pivot = new Vector2(rectTr.pivot.x, 1);
            // The image moves with the pivot so always keep it in [0, 0, 0]
            rectTr.localPosition = new Vector3(0, 0, 0);
        }
    }
}