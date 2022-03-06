
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationSelector : MonoBehaviour
{
    private bool notInit = true;
    public ItemSelector itemSelector;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (notInit && Time.frameCount > 10)
        {
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en"));
            notInit = false;
        }
    }

    public void changeLocale()
    {
        if (itemSelector.currentItem == 0)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en"));
        if (itemSelector.currentItem == 1)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("fr"));
    }
}
