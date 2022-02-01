using UnityEngine;
using UnityEngine.UI;

public class IntroAgeCurve : MonoBehaviour
{
    public Slider firstSensitiveAge;
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
        // Calcul de la valeur de l'exponentielle pour le premier age à partir duquel des morts peuvent arriver
        float minAgeExpo = Mathf.Exp(strength * ((float)firstAge / 100 - 1));
        // Calcul de la valeur maximale de l'exponentielle pour l'age le plus avancé
        float maxExpo = 1 - minAgeExpo;
        // lissage de la mortalité pour quelle soit à 0 au premier age sensible et à sa valeur maximale pour l'age le plus avancé
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
