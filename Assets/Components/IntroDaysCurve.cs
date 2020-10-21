using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroDaysCurve : MonoBehaviour
{
    public Slider windowSize;
    public Slider contagiousPeak;
    public Slider contagiousnessDeviation;

    private int length;
    private int peak;
    private float deviation;
    private LineRenderer curve;

    // Start is called before the first frame update
    void Start()
    {
        curve = GetComponent<LineRenderer>();
        length = (int)windowSize.value;
        peak = (int)contagiousPeak.value;
        deviation = contagiousnessDeviation.value;
        RefreshCurve();
    }

    // Update is called once per frame
    private void RefreshCurve()
    {
        Vector3[] newPositions = new Vector3[(int)length];
        float max = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(0);
        for (int i = 0; i < (int)length; i++)
            newPositions[i] = new Vector3((((float)i / ((int)length - 1)) * 300), ((1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation)) / max * 150), 0);
        curve.positionCount = newPositions.Length;
        curve.SetPositions(newPositions);
    }

    public void UpdateLength(float newLength)
    {
        length = (int)newLength;
        RefreshCurve();
    }

    public void UpdatePeak(float newValue)
    {
        peak = (int)newValue;
        RefreshCurve();
    }

    public void UpdateDeviation(float newValue)
    {
        deviation = newValue;
        RefreshCurve();
    }

}
