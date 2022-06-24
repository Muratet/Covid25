using UnityEngine;

/// <summary>
/// This component is use to transfert customized virus data to the <see cref="VirusStats"/> component
/// </summary>
public class LoadCustomizedVirus : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject customizedVirus = GameObject.Find("CustomizedVirus");
        if (customizedVirus)
        {
            VirusStats localVirus = GetComponent<VirusStats>();
            VirusStats newVirus = customizedVirus.GetComponent<VirusStats>();
            localVirus.contagiosity = newVirus.contagiosity;
            localVirus.populationRatioImmunity = newVirus.populationRatioImmunity;
            localVirus.windowSize = newVirus.windowSize;
            localVirus.contagiousnessPeak = newVirus.contagiousnessPeak;
            localVirus.contagiousnessDeviation = newVirus.contagiousnessDeviation;
            localVirus.deadlinessPeak = newVirus.deadlinessPeak;
            localVirus.deadlinessDeviation = newVirus.deadlinessDeviation;
            localVirus.firstSensitiveAge = newVirus.firstSensitiveAge;
            localVirus.maxDeadlinessRatio = newVirus.maxDeadlinessRatio;
            localVirus.curveStrenght = newVirus.curveStrenght;
            localVirus.seriousRatio = newVirus.seriousRatio;
        }
    }
}
