using UnityEngine;

/// <summary>
/// This component is used to toggle the right monitoring panel
/// </summary>
public class UpButtonToggled : MonoBehaviour
{
    /// <summary></summary>
    public Animator panelAnimator;
    /// <summary></summary>
    public Animator daysWindowsAnimator;
    /// <summary></summary>
    public AudioSource audiosource;
    /// <summary></summary>
    public AudioClip audioClip;

    /// <summary>
    /// Close and open appropriate monitoring panel
    /// </summary>
    /// <param name="newState"></param>
    public void OnValueChanged(bool newState)
    {
        if (newState)
        {
            panelAnimator.Play("DiagramEnterUI");
            audiosource.PlayOneShot(audioClip);
            if (daysWindowsAnimator)
                daysWindowsAnimator.Play("UpUI_out");
        }
        else
        {
            panelAnimator.Play("DiagramExitUI");
            if (daysWindowsAnimator)
                daysWindowsAnimator.Play("UpUI_in");
        }
    }
}
