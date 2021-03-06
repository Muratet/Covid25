﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Globalization;

public class PopUpBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int ageRef;

    private Image sr;
    private Color defaultColor;
    public Tooltip tooltip;

    private bool focused = false;

    private void Start()
    {
        sr = GetComponent<Image>();
        defaultColor = sr.color;
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
    }

    private void UpdateTooltipText()
    {
        string tooltipContent = ageRef + " ans : ";
        if (GetComponent<PopulationBar>())
            tooltipContent += MapSystem.territorySelected.popNumber[ageRef].ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
        if (GetComponent<DeathBar>())
            tooltipContent += MapSystem.territorySelected.popDeath[ageRef].ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
        if (GetComponent<InfectedBar>())
            tooltipContent += MapSystem.territorySelected.popInfected[ageRef].ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
        if (GetComponent<TreatedBar>())
            tooltipContent += MapSystem.territorySelected.popTreated[ageRef].ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
        tooltip.ShowTooltip(tooltipContent);
    } 

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = new Color(1, 1, 1);
        UpdateTooltipText();
        focused = true;
    }

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
