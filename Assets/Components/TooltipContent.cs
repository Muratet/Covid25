using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;
    private Tooltip tooltip;

    public AudioClip audioClip;

    private AudioSource audioSource;

    private bool isOver = false;

    private void Start()
    {
        tooltip = GameObject.Find("TooltipUI").GetComponent<Tooltip>();
        audioSource = GameObject.Find("AudioEffects").GetComponentInParent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(text);
        if (audioClip)
            audioSource.PlayOneShot(audioClip);
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
        isOver = false;
    }

    public void OnDisable()
    {
        if (isOver)
        {
            tooltip.HideTooltip();
            isOver = false;
        }
    }
}
