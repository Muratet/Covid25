using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncWithR0 : MonoBehaviour
{
    public Slider Contagiosity;
    private Slider mySlider;

    private void Start()
    {
        mySlider = GetComponent<Slider>();
    }

    void Update()
    {
        mySlider.value = 1 - (1 / Contagiosity.value);
    }
}
