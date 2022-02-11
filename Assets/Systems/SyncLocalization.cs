using UnityEngine;
using FYFY;
using System.Globalization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.Settings;

public class SyncLocalization : FSystem {

    private Family f_syncLocalized = FamilyManager.getFamily(new AllOfComponents(typeof(GameObjectLocalizer)), new AllOfProperties(PropertyMatcher.PROPERTY.ACTIVE_IN_HIERARCHY));

    protected override void onStart()
    {
        f_syncLocalized.addEntryCallback(updateLocale);
    }

    private void updateLocale(GameObject go)
    {
        go.GetComponent<GameObjectLocalizer>().ApplyLocaleVariant(LocalizationSettings.Instance.GetSelectedLocale());
    }
}