using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartGame : MonoBehaviour
{
    public TMP_Text countrySelected;
    public GameObject countryToLoad;

    private void OnEnable()
    {
        countryToLoad.GetComponent<CountryToLoad>().countryToLoad = Resources.Load<TextAsset>(countrySelected.text + "/Population");
        DontDestroyOnLoad(countryToLoad);
        SceneManager.LoadScene("Game");
    }
}
