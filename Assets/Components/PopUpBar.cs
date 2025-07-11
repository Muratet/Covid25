﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Globalization;

/// <summary>
/// This component is used to display a popup over pyramid bar on mouse over
/// </summary>
public class PopUpBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary></summary>
    public int ageRef;

    private Image sr;
    private Color defaultColor;
    /// <summary></summary>
    public Tooltip tooltip;

    private bool focused = false;
    private Localization localization;

    private void Start()
    {
        sr = GetComponent<Image>();
        defaultColor = sr.color;
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
        localization = GameObject.Find("Localization").GetComponent<Localization>();
    }

    private void UpdateTooltipText()
    {
        CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
        string tooltipContent = ageRef + localization.barYears;
        if (GetComponent<PopulationBar>())
            tooltipContent += MapSystem.territorySelected.popNumber[ageRef].ToString("N0", cultureInfo);
        if (GetComponent<DeathBar>())
            tooltipContent += MapSystem.territorySelected.popDeath[ageRef].ToString("N0", cultureInfo);
        if (GetComponent<InfectedBar>())
            tooltipContent += MapSystem.territorySelected.popInfected[ageRef].ToString("N0", cultureInfo);
        if (GetComponent<TreatedBar>())
            tooltipContent += MapSystem.territorySelected.popTreated[ageRef].ToString("N0", cultureInfo);
        tooltip.ShowTooltip(tooltipContent);
    } 

    /// <summary>
    /// Callback when pointer enter a bar
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = new Color(1, 1, 1);
        UpdateTooltipText();
        focused = true;
    }
    /// <summary>
    /// Callback chen pointer exit a bar
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
        sr.color = defaultColor;
        focused = false;
    }

    private void Update()
    {
        if (tooltip.IsShown() && focused)
            UpdateTooltipText();
    }
}
