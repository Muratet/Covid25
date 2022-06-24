using UnityEngine;
using FYFY;
using System.Globalization;

/// <summary>
/// This system used <see cref="SyncUI"/> component to invoke callback when a synchronization is required
/// </summary>
public class SyncUISystem : FSystem {

    private Family f_syncUI = FamilyManager.getFamily(new AllOfComponents(typeof(SyncUI)));

    /// <summary></summary>
    public TimeScale time;
    /// <summary></summary>
    public static bool needUpdate;

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

    /// <summary>
    /// Format String UI depending 
    /// </summary>
    /// <param name="textUI"></param>
    /// <param name="amount"></param>
    public static void formatStringUI(TMPro.TMP_Text textUI, float amount, Localization localisation)
    {
        CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
        if (amount >= 999000000000)
            textUI.text = "+999" + localisation.unitBillionsShort;
        else if ((int)(amount / 1000000000) > 0)
            textUI.text = (amount / 1000000000).ToString("N0", cultureInfo) + localisation.unitBillionsShort;
        else if ((int)(amount / 1000000) > 0)
            textUI.text = (amount / 1000000).ToString("N0", cultureInfo) + localisation.unitMillionsShort;
        else if ((int)amount / 1000 > 0)
            textUI.text = (amount / 1000).ToString("N0", cultureInfo) + localisation.unitThousandsShort;
        else
            textUI.text = amount.ToString("N0", cultureInfo) + "";
    }
}