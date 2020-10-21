using UnityEngine;
using UnityEngine.UI;
using FYFY;
using System.Globalization;

public class BarsSystem : FSystem {
    private Family f_updateBars = FamilyManager.getFamily(new AllOfComponents(typeof(PopUpBar)), new AnyOfComponents(typeof(DeathBar), typeof(InfectedBar), typeof(TreatedBar), typeof(PopulationBar)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));
    private PopUpBar tmpBar;
    private int[] data;
    GameObject barsContainer;
    private TimeScale time;

    public BarsSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération des données de la population
        TerritoryData popData = simu.GetComponent<TerritoryData>();
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();
        // Récupération du graphique
        barsContainer = GameObject.Find("Bars");
        // Récupération du préfab de barres
        BarPrefab barPrefab = barsContainer.GetComponent<BarPrefab>();

        // Create bars
        float yPos = -220f;
        for (int i = 0; i < popData.popNumber.Length; i++)
        {
            buildBar("initPop", i, barPrefab.prefab, barsContainer.transform, popData.popNumber[i], typeof(PopulationBar), new Color(0.34f, 0.47f, 0.81f), popData.maxNumber, yPos);
            buildBar("infected", i, barPrefab.prefab, barsContainer.transform, popData.popInfected[i], typeof(InfectedBar), new Color(0.86f, 0.39f, 0.22f), popData.maxNumber, yPos);
            buildBar("treated", i, barPrefab.prefab, barsContainer.transform, popData.popTreated[i], typeof(TreatedBar), new Color(0.36f, 0.81f, 0.34f), popData.maxNumber, yPos);
            buildBar("death", i, barPrefab.prefab, barsContainer.transform, popData.popDeath[i], typeof(DeathBar), new Color(0f, 0f, 0f), popData.maxNumber, yPos);
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

        // Vérifier s'il faut rafraîchir le diagramme
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
                    // ajout des personnes décédés qui ne sont pas prise en compte dans le décompte des personnes infectées mais qui l'est dans les personnes guéries
                    data = new int[MapSystem.territorySelected.popDeath.Length];
                    for (int age = 0; age < MapSystem.territorySelected.popDeath.Length; age++)
                        data[age] = MapSystem.territorySelected.popTreated[age] + MapSystem.territorySelected.popDeath[age];
                }

                float barWidth = (float)data[tmpBar.ageRef] / MapSystem.territorySelected.maxNumber;
                go.transform.localPosition = new Vector3((barWidth - 1) * 500, go.transform.localPosition.y, go.transform.localPosition.z);
                go.transform.localScale = new Vector3(barWidth * 10, 1, 1);
            }
            // mise à jour de l'abscisse
            Transform xAxis = barsContainer.transform.parent.GetChild(0).GetChild(0);
            int unit = 0;
            for (int child = 0; child < xAxis.childCount; child++)
            {
                xAxis.GetChild(child).gameObject.name = (unit / 1000).ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
                xAxis.GetChild(child).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = xAxis.GetChild(child).gameObject.name;
                unit += MapSystem.territorySelected.maxNumber / 10;
            }
        }
    }
}