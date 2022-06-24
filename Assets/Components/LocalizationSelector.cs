using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// Synchronise Unity localization settings with langage selected in game UI
/// </summary>
public class LocalizationSelector : MonoBehaviour
{
    private bool notInit = true;
    /// <summary>
    /// the langage selector
    /// </summary>
    public ItemSelector itemSelector;

    private void Update()
    {
        if (notInit && Time.frameCount > 10)
        {
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en"));
            notInit = false;
        }
    }

    /// <summary>
    /// Select local accrodingly to the langage selector
    /// </summary>
    public void changeLocale()
    {
        if (itemSelector.currentItem == 0)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en"));
        if (itemSelector.currentItem == 1)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("fr"));
    }
}
