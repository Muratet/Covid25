using UnityEngine;
using TMPro;

/// <summary>
/// This component is used in main menu to sync a slider with a TextMeshPro
/// </summary>
public class SynchronizeSlider : MonoBehaviour
{
    private TMP_Text text;

    /// <summary>
    /// Number of digits after the decimal point
    /// </summary>
    public int decimalPlaces = 0;
    /// <summary>
    /// multiplier to be applied to the slider value
    /// </summary>
    public int UI_multiplier = 1;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Transform the newValue and write it in the TextMeshPro
    /// </summary>
    /// <param name="newValue"></param>
    public void synchronizeText(float newValue)
    {
        text.text = (newValue*UI_multiplier).ToString("N"+decimalPlaces, UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo);
    }
}
