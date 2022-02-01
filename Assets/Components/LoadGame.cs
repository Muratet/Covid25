using UnityEngine;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    public AudioSource music;
    public GameObject transitionScreen;

    public VirusStats customizedStats;

    public Slider Contagiosity;
    public Slider PopImmunity;
    public Slider WindowSize;
    public Slider ContagiousnessPeak;
    public Slider ContagiousnessDeviation;
    public Slider DeadlinessPeak;
    public Slider DeadlinessDeviation;
    public Slider SeriousRatio;
    public Slider SeriousCritic;
    public Slider FirstSensitiveAge;
    public Slider CurveStrength;
    public Slider MaxDeadlinessRatio;
    public Slider VaccineMounth;

    // Start is called before the first frame update
    public void LoadGameScene()
    {
        DontDestroyOnLoad(music);
        transitionScreen.SetActive(true);
    }

    public void LoadCustomizedGame()
    {
        DontDestroyOnLoad(customizedStats);

        customizedStats.contagiosity = Contagiosity.value;
        customizedStats.populationRatioImmunity = Mathf.Round((1 - (1 / Contagiosity.value)) * 100f) / 100f; // arrondi à deux chiffres après la virgule
        customizedStats.windowSize = (int)WindowSize.value;
        customizedStats.contagiousnessPeak = ContagiousnessPeak.value;
        customizedStats.contagiousnessDeviation = ContagiousnessDeviation.value;
        customizedStats.deadlinessPeak = DeadlinessPeak.value;
        customizedStats.deadlinessDeviation = DeadlinessDeviation.value;
        customizedStats.seriousRatio = SeriousRatio.value;
        customizedStats.firstSensitiveAge = (int)FirstSensitiveAge.value;
        customizedStats.curveStrenght = (int)CurveStrength.value;
        customizedStats.maxDeadlinessRatio = MaxDeadlinessRatio.value;
        customizedStats.vaccineMounthDelay = (int)VaccineMounth.value;

    LoadGameScene();
    }
}
