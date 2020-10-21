using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Game");
    }
}
