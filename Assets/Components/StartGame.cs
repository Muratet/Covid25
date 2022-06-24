using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// This component is used in menu screen to load the game when playing animation is over
/// </summary>
public class StartGame : MonoBehaviour
{
    /// <summary></summary>
    public TMP_Text countrySelected;
    /// <summary></summary>
    public GameObject countryToLoad;

    private void OnEnable()
    {
        countryToLoad.GetComponent<CountryToLoad>().countryToLoad = Resources.Load<TextAsset>(countrySelected.text + "/Population");
        DontDestroyOnLoad(countryToLoad);
        SceneManager.LoadScene("Game");
    }
}
