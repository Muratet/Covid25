using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using FYFY;
using System.Runtime.InteropServices;

/// <summary>
/// This component enables to trigger the end of the game
/// </summary>
public class CheckEnd : MonoBehaviour
{

    /// <summary></summary>
    public TimeScale time;
    /// <summary></summary>
    public TerritoryData countryPopData;
    /// <summary></summary>
    public VirusStats virusStats;
    /// <summary></summary>
    public Revolution revolution;
    /// <summary></summary>
    public Finances finances;
    /// <summary></summary>

    /// <summary></summary>
    public TMP_Text immunityEnd;
    /// <summary></summary>
    public TMP_Text revolutionEnd;

    /// <summary></summary>
    public TMP_Text nbDead;
    /// <summary></summary>
    public TMP_Text debt;
    /// <summary></summary>
    public TMP_Text days;

    /// <summary></summary>
    public Toggle PauseButton;
    /// <summary></summary>
    public Button ContinueButton;
    /// <summary></summary>
    public Button reviewButton;
    /// <summary></summary>
    public Button returnToEndScreen;
    /// <summary></summary>

    /// <summary></summary>
    public TMP_InputField shareInputField;
    /// <summary></summary>
    public Button shareButton;

    /// <summary></summary>
    public string shareForbidden;
    /// <summary></summary>
    public string tryToCarryOn;

    /// <summary></summary>
    public string baseNbDeadText;
    private string nbDeadText;
    /// <summary></summary>
    public string baseDebtText;
    private string debtText;
    /// <summary></summary>
    public string baseDaysText;
    private string daysText;

    /// <summary></summary>
    public string yearsTxt;
    /// <summary></summary>
    public string monthsTxt;
    /// <summary></summary>
    public string daysTxt;

    /// <summary></summary>
    public string billionEuros;
    /// <summary></summary>
    public string millionEuros;
    /// <summary></summary>
    public string euros;

    private bool disableRevolutionEnd = false;

    private AudioSource music = null;

    /// <summary></summary>
    public AudioClip successEnd;
    /// <summary></summary>
    public AudioClip failEnd;
    private AudioClip defaultTheme;

    private bool isRevolution = false;

    private void Start()
    {
        GameObject go_music = GameObject.Find("Music"); // Not available in scene (come from Intro scene - DontDestroyOnLoad)
        if (go_music)
        {
            music = go_music.GetComponent<AudioSource>();
            defaultTheme = music.clip;
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (time.newDay)
        {
            // Check the end in case of herd immunity: achieve herd immunity threshold + 95% of infected people treated
            if ((float)countryPopData.nbInfected / countryPopData.nbPopulation > (virusStats.populationRatioImmunity - 0.05f) && (float)countryPopData.nbTreated / (countryPopData.nbInfected - countryPopData.nbDeath) > 0.95f)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                revolutionEnd.gameObject.SetActive(false);
                immunityEnd.gameObject.SetActive(true);
                if (!GameObject.Find("CustomizedVirus"))
                {
                    shareInputField.interactable = true;
                    shareInputField.GetComponent<TooltipContent>().enabled = false;
                    shareButton.interactable = true;
                    shareButton.GetComponent<TooltipContent>().enabled = false;
                }
                else  // do not share score for a customized virus
                {
                    shareInputField.GetComponent<TooltipContent>().text = shareForbidden;
                    shareButton.GetComponent<TooltipContent>().text = shareForbidden;
                }
                ContinueButton.gameObject.SetActive(false);
                reviewButton.gameObject.SetActive(true);
                displayEnd();
                if (music)
                {
                    music.clip = successEnd;
                    music.Play();
                }
            }
            else if (!disableRevolutionEnd) // Check the end in case of dissatisfaction: upper than 80% during 15 consécutive days
            {
                bool lessThanThreshold = false;
                for (int i = revolution.historyStress.Count - 1; i >= 0 && i >= revolution.historyStress.Count - 15 && !lessThanThreshold; i--)
                    lessThanThreshold = revolution.historyStress[i] < 80;
                if (!lessThanThreshold)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    immunityEnd.gameObject.SetActive(false);
                    revolutionEnd.gameObject.SetActive(true);
                    isRevolution = true;
                    shareInputField.interactable = false;
                    shareButton.interactable = false;
                    if (!GameObject.Find("CustomizedVirus")) // do not share score for a customized virus
                    {
                        shareInputField.GetComponent<TooltipContent>().text = tryToCarryOn;
                        shareButton.GetComponent<TooltipContent>().text = tryToCarryOn;
                    }
                    else
                    {
                        shareInputField.GetComponent<TooltipContent>().text = shareForbidden;
                        shareButton.GetComponent<TooltipContent>().text = shareForbidden;
                    }
                    displayEnd();
                    if (music)
                    {
                        music.clip = failEnd;
                        music.Play();
                    }
                }
            }
        }
    }

    private void displayEnd()
    {
        CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
        nbDeadText = countryPopData.nbDeath.ToString("N0", cultureInfo);
        nbDead.text = baseNbDeadText + nbDeadText;
        float debtValue = finances.historySpent[finances.historySpent.Count - 1];
        int nbMilliards = (int)(debtValue / 1000000000);
        if (nbMilliards > 0)
            debtText = nbMilliards.ToString("N0", cultureInfo) + " " + billionEuros;
        else
        {
            int nbMillions = (int)(debtValue / 1000000);
            if (nbMillions > 0)
                debtText = nbMillions.ToString("N0", cultureInfo) + " " + millionEuros;
            else
                debtText = debtValue.ToString("N0", cultureInfo) + " " + euros;
        }
        debt.text = baseDebtText + debtText;

        daysText = "";
        int nbYears = time.daysGone / 365;
        int nbMonths = (time.daysGone % 365) / 30;
        int nbDays = (time.daysGone % 365) % 30;
        if (nbYears > 0)
            daysText += nbYears.ToString("N0", cultureInfo) + " " + yearsTxt + " ";
        if (nbMonths > 0)
            daysText += nbMonths.ToString("N0", cultureInfo) + " " + monthsTxt + " ";
        if (nbDays > 0)
            daysText += nbDays.ToString("N0", cultureInfo) + " " + daysTxt;
        days.text = baseDaysText + daysText;

        // stop all systems
        foreach (FSystem sys in FSystemManager.updateSystems())
            if (sys != BarsSystem.instance && sys != CurvesSystem.instance)
                sys.Pause = true;
        this.enabled = false;
    }

    /// <summary>
    /// Reload main menu of the game (the first screen)
    /// </summary>
    public void BackToMainMenu()
    {
        GameObject music = GameObject.Find("Music");
        if (music)
            Destroy(music);
        GameObject customizedVirus = GameObject.Find("CustomizedVirus");
        if (customizedVirus)
            Destroy(customizedVirus);
        SceneManager.LoadScene("Intro");
    }

    /// <summary>
    /// Leave end screen and come back in game to continue to play
    /// </summary>
    public void ContinueToPlay()
    {
        disableRevolutionEnd = true;
        ContinueButton.gameObject.SetActive(false);
        reviewButton.gameObject.SetActive(true);
        // restart all systems
        foreach (FSystem sys in FSystemManager.updateSystems())
            sys.Pause = false;

        // Force pause button
        PauseButton.isOn = false;
        PauseButton.isOn = true;

        this.enabled = true;
        transform.GetChild(0).GetComponent<Animator>().Play("ContinueToPlay");
        Invoke("DisableChild", 0.25f); // After animation is over, we have to disable the child that it will be properly enabled again ate the end of the game

        if (music)
        {
            music.clip = defaultTheme;
            music.Play();
        }
    }

    private void DisableChild()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    [DllImport("__Internal")]
    private static extern void OpenURL(string url); // see file Assets/Plugins/OpenURL.jslib

    /// <summary>
    /// Open web page to see high scores
    /// </summary>
    public void ViewHighScores()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            OpenURL("http://www-ia.lip6.fr/~muratetm/covid25/viewHighScore.php");
        }
        else
        {
            Application.OpenURL("http://www-ia.lip6.fr/~muratetm/covid25/viewHighScore.php");
        }
    }

    /// <summary>
    /// Send player score to the web server
    /// </summary>
    public void SendScores()
    {
        if (shareInputField != null && shareInputField.text != "")
        {
            StartCoroutine(Upload());
            shareInputField.interactable = false;
            shareButton.interactable = false;
        }
    }

    private IEnumerator Upload()
    {
        CultureInfo cultureInfo = UnityEngine.Localization.Settings.LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo;
        UnityWebRequest www = UnityWebRequest.Get("http://www-ia.lip6.fr/~muratetm/covid25/addScore.php?pseudo=" + shareInputField.text + "&death=" + countryPopData.nbDeath.ToString("N0", cultureInfo) + "&debt=" + finances.historySpent[finances.historySpent.Count - 1].ToString("N0", cultureInfo) + "&days=" + time.daysGone.ToString("N0", cultureInfo) + "&revolution=" + isRevolution);

        yield return www.SendWebRequest();
    }
}
