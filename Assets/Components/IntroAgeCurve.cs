using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is used to integrate the first sensitive age to compute mortality by age
/// </summary>
public class IntroAgeCurve : MonoBehaviour
{
    /// <summary>
    /// The slider that define the first age sensitive to the virus
    /// </summary>
    public Slider firstSensitiveAge;
    /// <summary>
    /// The slider that define the strength of the curve
    /// </summary>
    public Slider curveStrength;

    private int firstAge;
    private float strength;
    private LineRenderer curve;

    // Start is called before the first frame update
    void Start()
    {
        curve = GetComponent<LineRenderer>();
        firstAge = (int)firstSensitiveAge.value;
        strength = curveStrength.value;
        RefrechCurve();
    }

    // Update is called once per frame
    private void RefrechCurve()
    {
        Vector3[] newPositions = new Vector3[101];
        // Compute the value of the exponential for the first age from which deaths can occur
        float minAgeExpo = Mathf.Exp(strength * ((float)firstAge / 100 - 1));
        // Compute the maximum value of the exponential for the oldest age
        float maxExpo = 1 - minAgeExpo;
        // Smoothing of mortality so that it is at 0 at the first sensitive age and at its maximum value for the oldest age
        for (int age = 0; age < 101; age++)
            newPositions[age] = new Vector3((float)age / 100 * 300, Mathf.Max(0f, (Mathf.Exp(strength*((float)age/100-1))-minAgeExpo)/maxExpo) * 150, 0);
        curve.positionCount = newPositions.Length;
        curve.SetPositions(newPositions);
    }

    public void UpdateFirstAge(float newAge)
    {
        firstAge = (int)newAge;
        RefrechCurve();
    }

    public void UpdateCurveStrength(float newValue)
    {
        strength = newValue;
        RefrechCurve();
    }
}
