using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// This component is used to define the content of a tooltip when the mouse point the game object
/// </summary>
public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The text to display inside the tooltip
    /// </summary>
    public string text;
    private Tooltip tooltip;

    /// <summary></summary>
    public AudioClip audioClip;

    private AudioSource audioSource;

    private bool isOver = false;

    private void Start()
    {
        tooltip = GameObject.Find("TooltipUI").GetComponent<Tooltip>();
        audioSource = GameObject.Find("AudioEffects").GetComponentInParent<AudioSource>();
    }

    /// <summary></summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(text);
        if (audioClip)
            audioSource.PlayOneShot(audioClip);
        isOver = true;
    }

    /// <summary></summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
        isOver = false;
    }

    /// <summary></summary>
    public void OnDisable()
    {
        if (isOver)
        {
            tooltip.HideTooltip();
            isOver = false;
        }
    }
}
