using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This component enables to customize tooltip on button depending on activation (used on button only usable in country selection)
/// </summary>
[RequireComponent(typeof(TooltipContent))]
[RequireComponent(typeof(Toggle))]
public class DisableButton : MonoBehaviour
{
    private TooltipContent tooltipContent;
    private Toggle toggle;

    /// <summary></summary>
    public Toggle toggleSubstitution;

    private void Start()
    {
        tooltipContent = GetComponent<TooltipContent>();
        toggle = GetComponent<Toggle>();
    }

    /// <summary>
    /// Disable the button and define the content of the tooltip
    /// </summary>
    /// <param name="newTooltip">The new content</param>
    public void DisableButtonAndSetTooltip(string newTooltip)
    {
        if (toggle.isOn)
            toggleSubstitution.isOn = true;
        tooltipContent.text = newTooltip;
        toggle.interactable = false;
    }

    /// <summary>
    /// Enable the button and define the content of the tooltip
    /// </summary>
    /// <param name="newTooltip"></param>
    public void EnableButtonAndSetTooltip(string newTooltip)
    {
        tooltipContent.text = newTooltip;
        toggle.interactable = true;
    }
}
