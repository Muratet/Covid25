using UnityEngine;
using FYFY;
using System.Collections.Generic;
using TMPro;
using System.Globalization;

public class CurvesSystem : FSystem {
    private Family f_displayedCurves = FamilyManager.getFamily(new AllOfComponents(typeof(LineRenderer)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY), new AnyOfTags("CumulativeDeathCurve", "HistoryDeathCurve", "MaskCurve", "VaccineCurve", "FinanceCurve", "StressCurve"));

    private TimeScale time;
    private Masks masks;
    private Vaccine vaccine;
    private Finances finances;
    private Revolution revolution;

    private int windowView = 180; // 6 derniers mois

    public static CurvesSystem instance;

    public CurvesSystem()
    {
        GameObject simu = GameObject.Find("SimulationData");
        // Récupération de l'échelle de temps
        time = simu.GetComponent<TimeScale>();
        // Récupération des données sur les masques
        masks = simu.GetComponent<Masks>();
        // Récupération des données du vaccin
        vaccine = simu.GetComponent<Vaccine>();
        // Récupération des finances
        finances = simu.GetComponent<Finances>();
        // Récupération des données de révolution
        revolution = simu.GetComponent<Revolution>();
        instance = this;
    }

    public void SetWindowView(int newWindowSize)
    {
        windowView = newWindowSize;
        SyncUISystem.needUpdate = true;
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay || SyncUISystem.needUpdate)
        {
            // mise à jour des courbes affichées
            foreach (GameObject curve in f_displayedCurves)
            {
                List<float> workingList = new List<float>();
                LineRenderer workingLineRenderer = curve.GetComponent<LineRenderer>();
                if (curve.CompareTag("CumulativeDeathCurve"))
                {
                    workingList = new List<float>();
                    foreach (int val in MapSystem.territorySelected.cumulativeDeath)
                        workingList.Add(val);
                }
                else if (curve.CompareTag("HistoryDeathCurve"))
                {
                    workingList = new List<float>();
                    foreach (int val in MapSystem.territorySelected.historyDeath)
                        workingList.Add(val);
                }
                else if (curve.CompareTag("MaskCurve"))
                    workingList = new List<float>(masks.historyStock);
                else if (curve.CompareTag("VaccineCurve"))
                    workingList = new List<float>(vaccine.historyStock);
                else if (curve.CompareTag("FinanceCurve"))
                    workingList = new List<float>(finances.historySpent);
                else if (curve.CompareTag("StressCurve"))
                    workingList = new List<float>(revolution.historyStress);

                // extraction des "windowView" derniers éléments
                if (windowView != -1 && workingList.Count > windowView)
                    workingList.RemoveRange(0, workingList.Count - windowView);
                if (workingList.Count >= 2)
                {
                    // Récupération de la valeur maximale de la liste de travail
                    float max = 100;
                    foreach (float val in workingList)
                        max = val > max ? val : max;
                    // cacul des nouvelles positions comprises entre [1, 11] pour X et [0, 5] pour Y
                    Vector3[] newPositions = new Vector3[workingList.Count];
                    for (int i = 0; i < workingList.Count; i++)
                        newPositions[i] = new Vector3((((float)i / (workingList.Count - 1)) * 200), (((workingList[i]) / max) * 100), 0);

                    // Application des nouvelles positions
                    workingLineRenderer.positionCount = newPositions.Length;
                    workingLineRenderer.SetPositions(newPositions);
                    workingLineRenderer.Simplify(0.5f);

                    // Mise à jour de l'ordonnée
                    Transform yAxis = curve.transform.parent.GetChild(1);
                    float step = max / (yAxis.childCount - 1);
                    float unit = 1;
                    TMP_Text UI_Unit = curve.transform.parent.Find("Unit").GetComponent<TMP_Text>();
                    UI_Unit.text = "";
                    if ((int)(step/1000000000) > 0)
                    {
                        unit = 1000000000;
                        UI_Unit.text = "(en Milliards)";
                    } else if ((int)(step / 1000000) > 0)
                    {
                        unit = 1000000;
                        UI_Unit.text = "(en Millions)";
                    }
                    else if((int)(step / 1000) > 0)
                    {
                        unit = 1000;
                        UI_Unit.text = "(en milliers)";
                    }

                    for (int child = 0; child < yAxis.childCount; child++)
                    {
                        yAxis.GetChild(child).gameObject.name = ((int)((max / (yAxis.childCount - 1)) * (yAxis.childCount - 1 - child) / unit)).ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
                        yAxis.GetChild(child).gameObject.GetComponent<TextMeshProUGUI>().text = yAxis.GetChild(child).gameObject.name;
                    }
                }

                // Mise à jour de l'abscisse
                Transform xAxis = curve.transform.parent.GetChild(0);
                for (int child = 0; child < xAxis.childCount; child++)
                {
                    int xValue;
                    if (windowView != -1)
                        xValue = (int)(Mathf.Max(0, time.daysGone - windowView) + (((float)Mathf.Min(time.daysGone, windowView) / (xAxis.childCount - 1)) * child));
                    else
                        xValue = (int)(((float)time.daysGone / (xAxis.childCount - 1)) * child);

                    xAxis.GetChild(child).gameObject.name = xValue.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")); // pour avoir un affichage du type 100 000 au lieu de 100000
                    xAxis.GetChild(child).gameObject.GetComponent<TextMeshProUGUI>().text = xAxis.GetChild(child).gameObject.name;
                }
            }
        }
	}
}