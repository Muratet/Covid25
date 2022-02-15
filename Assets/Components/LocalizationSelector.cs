using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationSelector : MonoBehaviour
{
    private bool notInit = true;

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

    public void changeLocale(int select)
    {
        if (select == 0)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en"));
        if (select == 1)
            LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().GetLocale("fr"));
    }
}
