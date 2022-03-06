using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using FYFY;
using System.Runtime.InteropServices;

public class CheckEnd : MonoBehaviour
{
    public TimeScale time;
    public TerritoryData countryPopData;
    public VirusStats virusStats;
    public Revolution revolution;
    public Finances finances;

    public TMP_Text immunityEnd;
    public TMP_Text revolutionEnd;

    public TMP_Text nbDead;
    public TMP_Text debt;
    public TMP_Text days;

    public Toggle PauseButton;
    public Button ContinueButton;
    public Button reviewButton;
    public Button returnToEndScreen;

    public TMP_InputField shareInputField;
    public Button shareButton;

    public string shareForbidden;
    public string tryToCarryOn;

    public string baseNbDeadText;
    private string nbDeadText;
    public string baseDebtText;
    private string debtText;
    public string baseDaysText;
    private string daysText;

    public string yearsTxt;
    public string monthsTxt;
    public string daysTxt;

    public string billionEuros;
    public string millionEuros;
    public string euros;

    private bool disableRevolutionEnd = false;

    private AudioSource music = null;

    public AudioClip successEnd;
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
            // Vérifier la fin en cas d'immunité collective : seuil d'immunité collective rapproché + 95% des personnes infectées soignées
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
                else  // ne pas partager le score pour un virus personnalisé
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
            else if (!disableRevolutionEnd) // Vérifier la fin en cas de révolution : Stress supérieur à 80% sur les 15 derniers jours
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
                    if (!GameObject.Find("CustomizedVirus")) // ne pas partager le score pour un virus personnalisé
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
        Debug.Log(time.daysGone);
        int nbYears = time.daysGone / 365;
        int nbMonths = (time.daysGone % 365) / 30;
        int nbDays = (time.daysGone % 365) % 30;
        if (nbYears > 0)
            daysText += nbYears.ToString("N0", cultureInfo) + " " + yearsTxt + " ";
        Debug.Log(daysText);
        if (nbMonths > 0)
            daysText += nbMonths.ToString("N0", cultureInfo) + " " + monthsTxt + " ";
        Debug.Log(daysText);
        if (nbDays > 0)
            daysText += nbDays.ToString("N0", cultureInfo) + " " + daysTxt;
        Debug.Log(daysText);
        days.text = baseDaysText + daysText;
        Debug.Log(daysText);

        // stopper tous les systèmes
        foreach (FSystem sys in FSystemManager.updateSystems())
            if (sys != BarsSystem.instance && sys != CurvesSystem.instance)
                sys.Pause = true;
        this.enabled = false;
    }
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

    public void ContinueToPlay()
    {
        disableRevolutionEnd = true;
        ContinueButton.gameObject.SetActive(false);
        reviewButton.gameObject.SetActive(true);
        // restart all systems
        foreach (FSystem sys in FSystemManager.updateSystems())
            sys.Pause = false;

        // Forcer le bouton en pause
        PauseButton.isOn = false;
        PauseButton.isOn = true;

        this.enabled = true;
        transform.GetChild(0).GetComponent<Animator>().Play("ContinueToPlay");
        Invoke("DisableChild", 0.25f); // Après que l'animation soit terminée, il faut désactiver le fils pour qu'il soit proprement réactivé lors de la fin du jeu

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
    private static extern void OpenURL(string url); // voir fichier Assets/Plugins/OpenURL.jslib

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
