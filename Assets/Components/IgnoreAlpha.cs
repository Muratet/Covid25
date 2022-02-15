using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class IgnoreAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    private Tooltip tooltip;
    private TerritoryData territory;
    private Animator animator;

    private AudioSource audioSource;
    public AudioClip audioClip;
    private Localization localization;

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        territory = GetComponent<TerritoryData>();
        animator = GetComponent<Animator>();
        tooltip = GameObject.Find("TooltipUI").GetComponent<Tooltip>();
        audioSource = GameObject.Find("AudioEffects").GetComponentInParent<AudioSource>();
        localization = GameObject.Find("Localization").GetComponent<Localization>();
    }

    public void OnPointerEnter(PointerEventData eventData)
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
        audioSource.PlayOneShot(audioClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
        if (MapSystem.territorySelected != territory)
            animator.Play("TerritoryIdle");
    }
}