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

    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        territory = GetComponent<TerritoryData>();
        animator = GetComponent<Animator>();
        tooltip = GameObject.Find("TooltipUI").GetComponent<Tooltip>();
        audioSource = GameObject.Find("AudioEffects").GetComponentInParent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string tooltipContent = "<b>"+territory.TerritoryName+"</b>";
        if (territory.closePrimarySchool)
            tooltipContent += "<br>  * Closure of schools";
        if (territory.closeSecondarySchool)
            tooltipContent += "<br>  * Closure of middle schools";
        if (territory.closeHighSchool)
            tooltipContent += "<br>  * Closure of high schools";
        if (territory.closeUniversity)
            tooltipContent += "<br>  * Closure of universities";
        if (territory.callCivicism)
            tooltipContent += "<br>  * Civic appeal";
        if (territory.closeShop)
            tooltipContent += "<br>  * Closure of shops";
        if (territory.certificateRequired)
            tooltipContent += "<br>  * Certificate required";
        if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "")
            tooltipContent += "<br>  * Exit ban between " + territory.ageDependentMin + " and "+ territory.ageDependentMax + " years old";
        if (territory.GetComponent<Beds>().boostBeds)
            tooltipContent += "<br>  * boosted beds";

        tooltipContent += "<br>(click: filter data)";
        tooltipContent += "<br>(double click: open panel)";
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