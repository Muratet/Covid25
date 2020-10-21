using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayWindowView : MonoBehaviour
{
    public int windowSize;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void OnValueChange (bool newState)
    {
        if (newState)
        {
            CurvesSystem.instance.SetWindowView(windowSize);
            audioSource.PlayOneShot(audioClip);
        }
    }

}
