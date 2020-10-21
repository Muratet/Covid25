using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class SynchronizeSlider : MonoBehaviour
{
    private TMP_Text text;

    public int decimalPlaces = 0;
    public int UI_multiplier = 1;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void synchronizeText(float newValue)
    {
        text.text = (newValue*UI_multiplier).ToString("N"+decimalPlaces, CultureInfo.CreateSpecificCulture("fr-FR"));
    }
}
