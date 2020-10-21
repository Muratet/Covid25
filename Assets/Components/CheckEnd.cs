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

    public TMP_InputField shareInputField;
    public Button shareButton;

    private string baseNbDeadText;
    private string nbDeadText;
    private string baseDebtText;
    private string debtText;
    private string baseDaysText;
    private string daysText;

    private bool disableRevolutionEnd = false;

    private AudioSource music = null;

    public AudioClip successEnd;
    public AudioClip failEnd;
    private AudioClip defaultTheme;

    private bool isRevolution = false;

    private void Start()
    {
        baseNbDeadText = nbDead.text;
        baseDebtText = debt.text;
        baseDaysText = days.text;
        GameObject go_music = GameObject.Find("Music");
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
                if (!GameObject.Find("CustomizedVirus")) // ne pas partager le score pour un virus personnalisé
                {
                    shareInputField.interactable = true;
                    shareButton.interactable = true;
                }
                ContinueButton.interactable = false;
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
        nbDeadText = countryPopData.nbDeath.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR"));
        nbDead.text = baseNbDeadText + nbDeadText;
        float debtValue = finances.historySpent[finances.historySpent.Count - 1];
        int nbMilliards = (int)(debtValue / 1000000000);
        if (nbMilliards > 0)
            debtText = nbMilliards.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " Milliard" + (nbMilliards > 1 ? "s" : "") + " d'euros";
        else
        {
            int nbMillions = (int)(debtValue / 1000000);
            if (nbMillions > 0)
                debtText = nbMillions.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " Million" + (nbMillions > 1 ? "s" : "") + " d'euros";
            else
                debtText = debtValue.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " euros";
        }
        debt.text = baseDebtText + debtText;

        daysText = "";
        int nbYears = time.daysGone / 365;
        int nbMonths = (time.daysGone % 365) / 30;
        int nbDays = (time.daysGone % 365) % 30;
        if (nbYears > 0)
            daysText += nbYears.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + " an" + (nbYears > 1 ? "s " : " ");
        if (nbMonths > 0)
            daysText += nbMonths + " mois ";
        if (nbDays > 0)
            daysText += nbDays + " jour" + (nbDays > 1 ? "s " : " ");
        days.text = baseDaysText + daysText;

        // stopper tous les systèmes
        foreach (FSystem sys in FSystemManager.updateSystems())
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
        ContinueButton.interactable = false;
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
        UnityWebRequest www = UnityWebRequest.Get("http://www-ia.lip6.fr/~muratetm/covid25/addScore.php?pseudo=" + shareInputField.text + "&death=" + countryPopData.nbDeath.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "&debt=" + finances.historySpent[finances.historySpent.Count - 1].ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "&days=" + time.daysGone.ToString("N0", CultureInfo.CreateSpecificCulture("fr-FR")) + "&revolution=" + isRevolution);

        yield return www.SendWebRequest();
    }
}
