using UnityEngine;
using FYFY;
using System.Collections.Generic;
using TMPro;
using System.Globalization;

/// <summary>
/// This system update curves
/// </summary>
public class CurvesSystem : FSystem {
    private Family f_displayedCurves = FamilyManager.getFamily(new AllOfComponents(typeof(LineRenderer)), new AnyOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY), new AnyOfTags("CumulativeDeathCurve", "HistoryDeathCurve", "MaskCurve", "VaccineCurve", "FinanceCurve", "StressCurve"));

    /// <summary></summary>
    public GameObject countrySimData;
    /// <summary></summary>
    public Localization localization;
    private TimeScale time;
    private Masks masks;
    private Vaccine vaccine;
    private Finances finances;
    private Revolution revolution;

    private int windowView = 180; // Last 6 months

    /// <summary>
    /// Singleton reference of this system
    /// </summary>
    public static CurvesSystem instance;

    /// <summary>
    /// Construct this system
    /// </summary>
    public CurvesSystem()
    {
        instance = this;
    }

    protected override void onStart()
    {
        // Recovery of the time scale
        time = countrySimData.GetComponent<TimeScale>();
        // Recovery masks data
        masks = countrySimData.GetComponent<Masks>();
        // Recovery vaccine data
        vaccine = countrySimData.GetComponent<Vaccine>();
        // Recovery of financial data
        finances = countrySimData.GetComponent<Finances>();
        // Recovery of dissatisfaction data
        revolution = countrySimData.GetComponent<Revolution>();

        foreach (GameObject curve in f_displayedCurves)
            if (curve.CompareTag("FinanceCurve"))
                curve.transform.parent.Find("Money").GetComponent<TMP_Text>().text = "(" + finances.money + ")";
    }

    /// <summary>
    /// Define the number of days to display on the x axis of the curve
    /// </summary>
    /// <param name="newWindowSize">Number of days</param>
    public void SetWindowView(int newWindowSize)
    {
        windowView = newWindowSize;
        SyncUISystem.needUpdate = true;
    }

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay || SyncUISystem.needUpdate)
        {
            // update of the displayed curves
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

                // extraction of the last "windowView" elements
                if (windowView != -1 && workingList.Count > windowView)
                    workingList.RemoveRange(0, workingList.Count - windowView);
                if (workingList.Count >= 2)
                {
                    // Recovery of the maximum value of the work list
                    float max = 100;
                    foreach (float val in workingList)
                        max = val > max ? val : max;
                    // calculation of new positions between [1, 11] for X and [0, 5] for Y
                    Vector3[] newPositions = new Vector3[workingList.Count];
                    for (int i = 0; i < workingList.Count; i++)
                        newPositions[i] = new Vector3((((float)i / (workingList.Count - 1)) * 200), (((workingList[i]) / max) * 100), 0);

                    // Application of the new positions
                    workingLineRenderer.positionCount = newPositions.Length;
                    workingLineRenderer.SetPositions(newPositions);
                    workingLineRenderer.Simplify(0.5f);

                    // Update of the y axis
                    Transform yAxis = curve.transform.parent.GetChild(1);
                    float step = max / (yAxis.childCount - 1);
                    float unit = 1;
                    TMP_Text UI_Unit = curve.transform.parent.Find("Unit").GetComponent<TMP_Text>();
                    UI_Unit.text = "";
                    if ((int)(step/1000000000) > 0)
                    {
                        unit = 1000000000;
                        UI_Unit.text = localization.unitBillions;
                    } else if ((int)(step / 1000000) > 0)
                    {
                        unit = 1000000;
                        UI_Unit.text = localization.unitMillions;
                    }
                    else if((int)(step / 1000) > 0)
                    {
                        unit = 1000;
                        UI_Unit.text = localization.unitThousands;
                    }

                    for (int child = 0; child < yAxis.childCount; child++)
                    {
                        yAxis.GetChild(child).gameObject.name = ((int)((max / (yAxis.childCount - 1)) * (yAxis.childCount - 1 - child) / unit)).ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
                        yAxis.GetChild(child).gameObject.GetComponent<TextMeshProUGUI>().text = yAxis.GetChild(child).gameObject.name;
                    }
                }

                // Update of x-axis
                Transform xAxis = curve.transform.parent.GetChild(0);
                for (int child = 0; child < xAxis.childCount; child++)
                {
                    int xValue;
                    if (windowView != -1)
                        xValue = (int)(Mathf.Max(0, time.daysGone - windowView) + (((float)Mathf.Min(time.daysGone, windowView) / (xAxis.childCount - 1)) * child));
                    else
                        xValue = (int)(((float)time.daysGone / (xAxis.childCount - 1)) * child);

                    xAxis.GetChild(child).gameObject.name = xValue.ToString("N0", UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
                    xAxis.GetChild(child).gameObject.GetComponent<TextMeshProUGUI>().text = xAxis.GetChild(child).gameObject.name;
                }
            }
        }
	}
}