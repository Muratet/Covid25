using UnityEngine;
using FYFY;
using System.Globalization;

public class SyncUISystem : FSystem {

    private Family f_syncUI = FamilyManager.getFamily(new AllOfComponents(typeof(SyncUI)));

    public TimeScale time;
    public static bool needUpdate;

    protected override void onStart()
    {
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount) {
        if (time.newDay || needUpdate)
        {
            needUpdate = false;
            foreach (GameObject uiGO in f_syncUI)
            {
                SyncUI s_ui = uiGO.GetComponent<SyncUI>();
                s_ui.callback.Invoke();
            }
        }
    }

    public static void formatStringUI(TMPro.TMP_Text textUI, float amount)
    {
        CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
        if (amount >= 999000000000)
            textUI.text = "+999Md";
        else if ((int)(amount / 1000000000) > 0)
            textUI.text = (amount / 1000000000).ToString("N0", cultureInfo) + "Md";
        else if ((int)(amount / 1000000) > 0)
            textUI.text = (amount / 1000000).ToString("N0", cultureInfo) + "M";
        else if ((int)amount / 1000 > 0)
            textUI.text = (amount / 1000).ToString("N0", cultureInfo) + "k";
        else
            textUI.text = amount.ToString("N0", cultureInfo) + "";
    }
}