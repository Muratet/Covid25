using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TooltipContent))]
[RequireComponent(typeof(Toggle))]
public class DisableButton : MonoBehaviour
{
    private TooltipContent tooltipContent;
    private Toggle toggle;

    public Toggle toggleSubstitution;

    private void Start()
    {
        tooltipContent = GetComponent<TooltipContent>();
        toggle = GetComponent<Toggle>();
    }

    public void DisableButtonAndSetTooltip(string newTooltip)
    {
        if (toggle.isOn)
            toggleSubstitution.isOn = true;
        tooltipContent.text = newTooltip;
        toggle.interactable = false;
    }

    public void EnableButtonAndSetTooltip(string newTooltip)
    {
        tooltipContent.text = newTooltip;
        toggle.interactable = true;
    }
}
