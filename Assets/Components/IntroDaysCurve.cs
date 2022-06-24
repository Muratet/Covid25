using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to compute the curve depending on a window size, a peak position and a standard deviation
/// </summary>
public class IntroDaysCurve : MonoBehaviour
{
    /// <summary>
    /// Slider that defines the number of days used to build the curve
    /// </summary>
    public Slider windowSize;
    /// <summary>
    /// Slider that defines the day (inside the window) where the peak of the curve is positionned
    /// </summary>
    public Slider contagiousPeak;
    /// <summary>
    /// Slider that defines the standard deviation of the curve
    /// </summary>
    public Slider contagiousnessDeviation;

    private int size;
    private int peak;
    private float deviation;
    private LineRenderer curve;

    // Start is called before the first frame update
    void Start()
    {
        curve = GetComponent<LineRenderer>();
        size = (int)windowSize.value;
        peak = (int)contagiousPeak.value;
        deviation = contagiousnessDeviation.value;
        RefreshCurve();
    }

    // Update is called once per frame
    private void RefreshCurve()
    {
        Vector3[] newPositions = new Vector3[(int)size];
        float max = (1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(0);
        for (int i = 0; i < (int)size; i++)
            newPositions[i] = new Vector3((((float)i / ((int)size - 1)) * 300), ((1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-((i - peak) * (i - peak)) / (2 * deviation * deviation)) / max * 150), 0);
        curve.positionCount = newPositions.Length;
        curve.SetPositions(newPositions);
    }

    /// <summary>
    /// Used to define the window size
    /// </summary>
    /// <param name="newLength"></param>
    public void UpdateLength(float newLength)
    {
        size = (int)newLength;
        RefreshCurve();
    }

    /// <summary>
    /// Used to define the peak position
    /// </summary>
    /// <param name="newValue"></param>
    public void UpdatePeak(float newValue)
    {
        peak = (int)newValue;
        RefreshCurve();
    }

    /// <summary>
    /// Used to define the standard deviation
    /// </summary>
    /// <param name="newValue"></param>
    public void UpdateDeviation(float newValue)
    {
        deviation = newValue;
        RefreshCurve();
    }

}
