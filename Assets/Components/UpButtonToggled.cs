using UnityEngine;

public class UpButtonToggled : MonoBehaviour
{
    public Animator panelAnimator;
    public Animator daysWindowsAnimator;
    public AudioSource audiosource;
    public AudioClip audioClip;

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
