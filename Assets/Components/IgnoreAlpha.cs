using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This component is used on map to enable territory selection even if territory is under another but visible due to alpha
/// </summary>
public class IgnoreAlpha : MonoBehaviour
{
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    private Tooltip tooltip;
    private TerritoryData territory;
    private Animator animator;

    private AudioSource audioSource;

    private Localization localization;

    /// <summary>
    /// Sound to play when mouse enter this
    /// </summary>
    public AudioClip onEnter;

    /// <summary>
    /// Sound to play when mouse click this
    /// </summary>
    public AudioClip onClick;

    private DisableButton maskButton;

    private DisableButton financeButton;

    private DisableButton revolutionButton;

    private DisableButton vaccineButton;

    private CheckDoubleClick checkDoubleClick;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        territory = GetComponent<TerritoryData>();
        animator = GetComponent<Animator>();
        tooltip = GameObject.Find("TooltipUI").GetComponent<Tooltip>();
        audioSource = GameObject.Find("AudioEffects").GetComponentInParent<AudioSource>();
        localization = GameObject.Find("Localization").GetComponent<Localization>();
        maskButton = GameObject.Find("MaskButton").GetComponent<DisableButton>();
        financeButton = GameObject.Find("FinanceButton").GetComponent<DisableButton>();
        revolutionButton = GameObject.Find("RevolutionButton").GetComponent<DisableButton>();
        vaccineButton = GameObject.Find("VaccineButton").GetComponent<DisableButton>();
        checkDoubleClick = GameObject.Find("MapMenu").GetComponent<CheckDoubleClick>();
    }

    /// <summary>
    /// Used to manages available monitoring data and action panels depending on selected territory
    /// </summary>
    public void onSelectTerritory()
    {
        MapSystem.instance.selectTerritory(GetComponent<TerritoryData>());
        audioSource.PlayOneShot(onClick);
        maskButton.DisableButtonAndSetTooltip(localization.maskTooltip);
        financeButton.DisableButtonAndSetTooltip(localization.financeTooltip);
        revolutionButton.DisableButtonAndSetTooltip(localization.revolutionTooltip);
        vaccineButton.DisableButtonAndSetTooltip(localization.vaccineTooltip);
        AdvisorSystem.instance.ClosePanel();
        checkDoubleClick.CheckDoubleClic(territory.id);
    }

    /// <summary>
    /// Used to zoom in/out on the map
    /// </summary>
    /// <param name="data"></param>
    public void onScroll(BaseEventData data)
    {
        MapSystem.instance.onScroll(data);
    }

    /// <summary>
    /// Used to move the map
    /// </summary>
    /// <param name="data"></param>
    public void onDrag(BaseEventData data)
    {
        MapSystem.instance.onDrag(data);
    }

    /// <summary>
    /// Display tooltip of this territory that resumes the choices done on this territory
    /// </summary>
    public void onPointerEnter()
    {
        string tooltipContent = "<b>"+territory.TerritoryName+"</b>";
        if (territory.closePrimarySchool)
            tooltipContent += "<br>  * "+localization.territoriesTooltip[0];
        if (territory.closeSecondarySchool)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[1];
        if (territory.closeHighSchool)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[2];
        if (territory.closeUniversity)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[3];
        if (territory.callCivicism)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[4];
        if (territory.closeShop)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[5];
        if (territory.certificateRequired)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[6];
        if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "")
            tooltipContent += "<br>  * " + localization.getFormatedText(localization.territoriesTooltip[7], territory.ageDependentMin, territory.ageDependentMax);
        if (territory.GetComponent<Beds>().boostBeds)
            tooltipContent += "<br>  * " + localization.territoriesTooltip[8];

        tooltipContent += "<br>" + localization.territoriesTooltip[9];
        tooltipContent += "<br>" + localization.territoriesTooltip[10];
        tooltip.ShowTooltip(tooltipContent);
        transform.SetAsLastSibling();
        if (MapSystem.territorySelected != territory)
            animator.Play("TerritoryFocused");
        audioSource.PlayOneShot(onEnter);
    }

    /// <summary>
    /// Hide information tooltip
    /// </summary>
    public void onPointerExit()
    {
        tooltip.HideTooltip();
        if (MapSystem.territorySelected != territory)
            animator.Play("TerritoryIdle");
    }
}