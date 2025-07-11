﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This component is used to display a tooltip near from the mouse cursor
/// </summary>
public class Tooltip : MonoBehaviour
{
    private TMP_Text tooltipText;
    private RectTransform backgroundRectTransform;
    private Image backgroundImg;

    private void Awake()
    {
        backgroundRectTransform = GetComponent<RectTransform>();
        backgroundImg = GetComponent<Image>();
        tooltipText = transform.GetChild(0).GetComponent<TMP_Text>();
        backgroundImg.enabled = false;
        tooltipText.enabled = false;
    }

    /// <summary>
    /// </summary>
    /// <returns>True if the tootltip is shown and false else</returns>
    public bool IsShown()
    {
        return backgroundImg.enabled && tooltipText.enabled;
    }

    /// <summary>
    /// Show the tooltip
    /// </summary>
    /// <param name="tooltipString">The new content of the tooltip</param>
    public void ShowTooltip(string tooltipString)
    {
        backgroundImg.enabled = true;
        tooltipText.enabled = true;

        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth+textPaddingSize*2, tooltipText.preferredHeight+textPaddingSize*2);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    /// <summary>
    /// Hide the tooltip
    /// </summary>
    public void HideTooltip()
    {
        backgroundImg.enabled = false;
        tooltipText.enabled = false;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        // reposition the tooltip so that it points to the center of the screen
        if (mousePos.x > Screen.width / 2)
            mousePos.x -= (10 + backgroundRectTransform.sizeDelta.x / 2) * ((float)Screen.width / 1920);
        else if (mousePos.x < Screen.width / 2)
            mousePos.x += (10 + backgroundRectTransform.sizeDelta.x / 2) * ((float)Screen.width / 1920);

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            mousePos.y += (40 - backgroundRectTransform.sizeDelta.y / 2) * ((float)Screen.height / 1080);
            transform.position = mousePos;
        }
        else
        {
            mousePos.y += (10 + backgroundRectTransform.sizeDelta.y / 2) * ((float)Screen.height / 1080);
            mousePos.z = parentCanvas.planeDistance;

            transform.position = parentCanvas.worldCamera.ScreenToWorldPoint(mousePos);
        }
    }
}
