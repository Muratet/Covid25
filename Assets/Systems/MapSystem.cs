using UnityEngine;
using UnityEngine.UI;
using FYFY;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// This system manage the map (zoom, moving, territory color)
/// Must be define before MaskSystem, VaccineSystem and FinanceSystem
/// </summary>
public class MapSystem : FSystem {
    private Family f_territories = FamilyManager.getFamily(new AllOfComponents(typeof(TerritoryData), typeof(Image)));

    /// <summary></summary>
    public static TerritoryData territorySelected;

    /// <summary></summary>
    public TMP_Text territoryName;
    /// <summary></summary>
    public GameObject simulationData;
    private TimeScale time;
    /// <summary></summary>
    public TextAsset rawPopulationData;
    /// <summary></summary>
    public TextAsset rawCountryData;

    /// <summary></summary>
    public GameObject territoryPrefab;
    /// <summary></summary>
    public Transform territoriesParent;

    /// <summary></summary>
    public bool isDragging;

    private TerritoryData territoriesData;
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

    private struct RawCountryData
    {
        public string money;
        public float icuBed_CostDay;
        public float borders_TravellerFreightRatio;
        public float borders_CostLostTourismPerDay;
        public float borders_CostLostFreightPerDay;
        public float mask_NationalStock;
        public float mask_DailyMedicalIntake;
        public float mask_DailyNationalProduction;
        public float mask_MaxNationalProduction;
        public float mask_MaxDeliveryPack;
        public float vaccine_MeanDeliveryPack;
        public float vaccine_trust;
        public float tax_CompensateTaxesCanceledPerDay;
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
        // Recovery of the time scale
        time = simulationData.GetComponent<TimeScale>();

        f_territories.addEntryCallback(onNewTerritory);

        GameObject countryToLoad = GameObject.Find("CountryToLoad");
        if (countryToLoad != null)
        {
            rawPopulationData = countryToLoad.GetComponent<CountryToLoad>().territoriesData;
            rawCountryData = countryToLoad.GetComponent<CountryToLoad>().countryData;
        }

        // Load game content from the file
        Dictionary<string, RawTerriroryData> populationData = JsonConvert.DeserializeObject<Dictionary<string, RawTerriroryData>>(rawPopulationData.text);
        RawCountryData countryData = JsonConvert.DeserializeObject<RawCountryData>(rawCountryData.text);

        // Reset the nation's data to make sure it is synchronized with the regions' accumulated data
        territoriesData = time.GetComponent<TerritoryData>();
        for (int age = 0; age < territoriesData.popNumber.Length; age++)
            territoriesData.popNumber[age] = 0;
        territoriesData.id = -1;
        territoriesData.populationRatio = 1;
        territoriesData.nbPopulation = 0;
        Beds countryBeds = time.GetComponent<Beds>();
        countryBeds.intensiveBeds_default = 0;
        countryBeds.intensiveBeds_high = 0;

        int max = 0;
        // recovery of data from each territory
        foreach (KeyValuePair<string, RawTerriroryData> entry in populationData)
        {
            if (entry.Value.id == -1)
                territoriesData.TerritoryName = entry.Key;
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
                    territoriesData.popNumber[age] += territoryData.popNumber[age];
                }
                territoryData.nbPopulation = total;
                territoriesData.nbPopulation += total;
                // Calculation of the power of 10 immediately above the maximum
                territoryData.maxNumber = max - (max % 1000) + 1000;
                // loading of bed data
                Beds territoryBeds = newTerritory.GetComponent<Beds>();
                territoryBeds.intensiveBeds_default = entry.Value.bedsDefault;
                territoryBeds.intensiveBeds_high = entry.Value.bedsHigh;
                countryBeds.intensiveBeds_default += entry.Value.bedsDefault;
                countryBeds.intensiveBeds_high += entry.Value.bedsHigh;
                // Loading images
                newTerritory.GetComponent<Image>().sprite = Resources.Load<Sprite>(territoriesData.TerritoryName + "/Regions/" + territoryData.TerritoryName);
                newTerritory.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(territoriesData.TerritoryName + "/Regions/" + territoryData.TerritoryName + "_focused");
            }
        }

        max = 0;
        for (int age = 0; age < territoriesData.popNumber.Length; age++)
            max = Mathf.Max(max, territoriesData.popNumber[age]);
        territoriesData.maxNumber = max - (max % 1000) + 1000;

        territorySelected = territoriesData;
        territoryName.text = territorySelected.TerritoryName;
        targetScale = territoriesParent.transform.localScale;

        Finances finances = simulationData.GetComponent<Finances>();
        finances.money = countryData.money;
        finances.oneDayReanimationCost = countryData.icuBed_CostDay;
        simulationData.GetComponent<FrontierPermeability>().travellerRatio = countryData.borders_TravellerFreightRatio;
        finances.costLostTourismPerDay = countryData.borders_CostLostTourismPerDay;
        finances.costLostFreightPerDay = countryData.borders_CostLostFreightPerDay;
        Masks masks = simulationData.GetComponent<Masks>();
        masks.nationalStock = countryData.mask_NationalStock;
        masks.medicalRequirementPerDay_low = countryData.mask_DailyMedicalIntake;
        masks.nationalProductionPerDay_low = countryData.mask_DailyNationalProduction;
        masks.nationalProductionPerDay_high = countryData.mask_MaxNationalProduction;
        masks.maxDeliveryPack = countryData.mask_MaxDeliveryPack;
        Vaccine vaccine = simulationData.GetComponent<Vaccine>();
        vaccine.meanDeliveryPack = countryData.vaccine_MeanDeliveryPack;
        vaccine.vaccineTrust = countryData.vaccine_trust;
        simulationData.GetComponent<Tax>().compensateTaxesCanceled = countryData.tax_CompensateTaxesCanceledPerDay;
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
        territoryData.populationRatio = (float)territoryData.nbPopulation / territoriesData.nbPopulation;
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
    /// Zoom in/out with scroll
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
    public void onBeginDrag()
    {
        isDragging = true;
    }

    public void onEndDrag()
    {
        Debug.Log("onEndDrag");
        MainLoop.instance.StartCoroutine(delayEndDrag());
    }

    private IEnumerator delayEndDrag()
    {
        yield return new WaitForSeconds(.1f);
        isDragging = false;
    }

    /// <summary>
    /// Zoom in/out with UI buttons
    /// </summary>
    /// <param name="amount"></param>
    public void zoom(float amount)
    {
        targetScale = new Vector3(territoriesParent.transform.localScale.x + amount / (3 / territoriesParent.transform.localScale.x), territoriesParent.transform.localScale.y + amount / (3 / territoriesParent.transform.localScale.y), territoriesParent.transform.localScale.z);
    }
}