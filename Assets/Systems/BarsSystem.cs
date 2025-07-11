﻿using UnityEngine;
using UnityEngine.UI;
using FYFY;

/// <summary>
/// Manage all (death, infected and trated) pyramid bars
/// </summary>
public class BarsSystem : FSystem {
    private Family f_updateBars = FamilyManager.getFamily(new AllOfComponents(typeof(PopUpBar)), new AnyOfComponents(typeof(DeathBar), typeof(InfectedBar), typeof(TreatedBar), typeof(PopulationBar)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
    private PopUpBar tmpBar;
    private int[] data;
    /// <summary></summary>
    public GameObject barsContainer;
    /// <summary></summary>
    public GameObject barPrefab;
    /// <summary></summary>
    public Transform xAxis;

    /// <summary></summary>
    public GameObject countrySimData;
    private TimeScale time;
    private TerritoryData countryPopData;

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static BarsSystem instance;

    /// <summary>
    /// Construct this system
    /// </summary>
    public BarsSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery population data
        countryPopData = countrySimData.GetComponent<TerritoryData>();

        // Create bars
        float yPos = -220f;
        for (int i = 0; i < countryPopData.popNumber.Length; i++)
        {
            buildBar("initPop", i, barPrefab, barsContainer.transform, countryPopData.popNumber[i], typeof(PopulationBar), new Color(0.34f, 0.47f, 0.81f), countryPopData.maxNumber, yPos);
            buildBar("infected", i, barPrefab, barsContainer.transform, countryPopData.popInfected[i], typeof(InfectedBar), new Color(0.86f, 0.39f, 0.22f), countryPopData.maxNumber, yPos);
            buildBar("treated", i, barPrefab, barsContainer.transform, countryPopData.popTreated[i], typeof(TreatedBar), new Color(0.36f, 0.81f, 0.34f), countryPopData.maxNumber, yPos);
            buildBar("death", i, barPrefab, barsContainer.transform, countryPopData.popDeath[i], typeof(DeathBar), new Color(0f, 0f, 0f), countryPopData.maxNumber, yPos);
            yPos += 5f;
        }
    }

    private void buildBar(string name, int age, GameObject prefab, Transform container, int initialBarWidth, System.Type type, Color color, int maxValue, float yPos)
    {
        GameObject newbar = GameObject.Instantiate(prefab, container);
        newbar.name = name + "_Bar_" + age;
        PopUpBar popUp = newbar.GetComponent<PopUpBar>();
        popUp.ageRef = age;
        newbar.GetComponent<Image>().color = color;
        newbar.AddComponent(type);
        GameObjectManager.bind(newbar);
        float barWidth = (float)initialBarWidth / maxValue;
        newbar.transform.localPosition = new Vector3((barWidth - 1) * 500, yPos, 0);
        newbar.transform.localScale = new Vector3(barWidth * 10, 1, 1);
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

        // Check if the diagram needs to be refreshed
        if (time.newDay || SyncUISystem.needUpdate)
        {
            foreach (GameObject go in f_updateBars)
            {
                tmpBar = go.GetComponent<PopUpBar>();
                if (go.GetComponent<PopulationBar>())
                    data = MapSystem.territorySelected.popNumber;
                else if (go.GetComponent<DeathBar>())
                    data = MapSystem.territorySelected.popDeath;
                else if (go.GetComponent<InfectedBar>())
                    data = MapSystem.territorySelected.popInfected;
                else if (go.GetComponent<TreatedBar>())
                {
                    // addition of deceased persons who are not included in the count of infected persons but are included in the count of cured persons
                    data = new int[MapSystem.territorySelected.popDeath.Length];
                    for (int age = 0; age < MapSystem.territorySelected.popDeath.Length; age++)
                        data[age] = MapSystem.territorySelected.popTreated[age] + MapSystem.territorySelected.popDeath[age];
                }

                float barWidth = (float)data[tmpBar.ageRef] / MapSystem.territorySelected.maxNumber;
                go.transform.localPosition = new Vector3((barWidth - 1) * 500, go.transform.localPosition.y, go.transform.localPosition.z);
                go.transform.localScale = new Vector3(barWidth * 10, 1, 1);
            }
            // update of the x-axis
            int unit = 0;
            for (int child = 0; child < xAxis.childCount; child++)
            {
                xAxis.GetChild(child).gameObject.name = (unit / 1000).ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
                xAxis.GetChild(child).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = xAxis.GetChild(child).gameObject.name;
                unit += MapSystem.territorySelected.maxNumber / 10;
            }
        }
    }
}