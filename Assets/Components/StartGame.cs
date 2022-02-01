using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Game");
    }
}
