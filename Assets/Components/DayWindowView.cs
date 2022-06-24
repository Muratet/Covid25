using UnityEngine;

/// <summary>
/// This component enables to set the amount of day to display inside curves panels
/// </summary>
public class DayWindowView : MonoBehaviour
{
    /// <summary></summary>
    public int windowSize;
    /// <summary></summary>
    public AudioSource audioSource;
    /// <summary></summary>
    public AudioClip audioClip;

    /// <summary>
    /// Apply this windowSize on curve panels
    /// </summary>
    /// <param name="newState">if True apply data</param>
    public void OnValueChange (bool newState)
    {
        if (newState)
        {
            CurvesSystem.instance.SetWindowView(windowSize);
            audioSource.PlayOneShot(audioClip);
        }
    }

}
