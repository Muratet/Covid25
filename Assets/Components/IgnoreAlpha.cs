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
            tooltipContent += "<br>  * Fermeture des écoles";
        if (territory.closeSecondarySchool)
            tooltipContent += "<br>  * Fermeture des collèges";
        if (territory.closeHighSchool)
            tooltipContent += "<br>  * Fermeture des lycées";
        if (territory.closeUniversity)
            tooltipContent += "<br>  * Fermeture des universités";
        if (territory.callCivicism)
            tooltipContent += "<br>  * Appel civique";
        if (territory.closeShop)
            tooltipContent += "<br>  * Fermeture commerces";
        if (territory.certificateRequired)
            tooltipContent += "<br>  * Certificat requis";
        if (territory.ageDependent && territory.ageDependentMin != "" && territory.ageDependentMax != "")
            tooltipContent += "<br>  * Interdiction de sortie entre " + territory.ageDependentMin + " et "+ territory.ageDependentMax + " ans";
        if (territory.GetComponent<Beds>().boostBeds)
            tooltipContent += "<br>  * lits boostés";
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