using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This component is used to load the game scene
/// </summary>
public class LoadGame : MonoBehaviour
{
    /// <summary></summary>
    public AudioSource music;
    /// <summary></summary>
    public GameObject transitionScreen;
    /// <summary></summary>

    /// <summary></summary>
    public VirusStats customizedStats;

    /// <summary></summary>
    public Slider Contagiosity;
    /// <summary></summary>
    public Slider PopImmunity;
    /// <summary></summary>
    public Slider WindowSize;
    /// <summary></summary>
    public Slider ContagiousnessPeak;
    /// <summary></summary>
    public Slider ContagiousnessDeviation;
    /// <summary></summary>
    public Slider DeadlinessPeak;
    /// <summary></summary>
    public Slider DeadlinessDeviation;
    /// <summary></summary>
    public Slider SeriousRatio;
    /// <summary></summary>
    public Slider SeriousCritic;
    /// <summary></summary>
    public Slider FirstSensitiveAge;
    /// <summary></summary>
    public Slider CurveStrength;
    /// <summary></summary>
    public Slider MaxDeadlinessRatio;
    /// <summary></summary>
    public Slider VaccineMounth;

    /// <summary>
    /// Play animation that will start automatically the game
    /// </summary>
    public void LoadGameScene()
    {
        DontDestroyOnLoad(music);
        transitionScreen.SetActive(true);
    }

    /// <summary>
    /// Load customized data and call <see cref="LoadGameScene"/>
    /// </summary>
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
