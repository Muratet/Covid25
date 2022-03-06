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

    protected override void onStart()
    {
        TerritoryData countryData = time.GetComponent<TerritoryData>();
        //Load game content from the file
        Dictionary<string, int[]> populationData = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(rawContent.text);
        // Réinitialiser les données de la nation pour être sûr de les synchroniser avec les données accumulées des régions
        for (int age = 0; age < countryData.popNumber.Length; age++)
            countryData.popNumber[age] = 0;
        foreach (GameObject territory in f_territories)
        {
            TerritoryData territoryData = territory.GetComponent<TerritoryData>();
            territoryData.popNumber = populationData[territoryData.TerritoryName];
            int total = 0;
            int max = 0;
            for (int age = 0; age < territoryData.popNumber.Length; age++)
            {
                total += territoryData.popNumber[age];
                max = Mathf.Max(max, territoryData.popNumber[age]);
                // accumulation au niveau national
                countryData.popNumber[age] += territoryData.popNumber[age];
            }
            territoryData.nbPopulation = total;
            // Calcul de la puissance de 10 immediatement supérieure au maximum
            int multipleOfThousand = 0;
            int reste = 0;
            while (max - 1000 > 0)
            {
                max -= 1000;
                multipleOfThousand++;
                reste = max % 10;
            }
            multipleOfThousand++;
            territoryData.maxNumber = Mathf.Max(10000, multipleOfThousand * 1000);
        }
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

    public void onScroll(Vector2 pos){

    }
}