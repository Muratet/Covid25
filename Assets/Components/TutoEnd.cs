using UnityEngine;
using UnityEngine.UI;
using FYFY;

/// <summary>
/// This component is used configure game when tutorial is closed
/// </summary>
public class TutoEnd : MonoBehaviour
{
    /// <summary></summary>
    public Button button;
    /// <summary></summary>
    public Animator territoryPannel;
    /// <summary></summary>
    public Animator countryPannel;
    /// <summary></summary>
    public VirusStats virusStats;
    /// <summary></summary>
    public Localization localization;

    private void OnDisable()
    {
        // simulate a double click
        territoryPannel.SetTrigger("Close");
        countryPannel.SetTrigger("Close");
        button.onClick.Invoke();

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "0", messageBody = localization.advisorHealthTexts[0] });

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = localization.advisorTitlePrimeMinister, timeStamp = "0", messageBody = localization.getFormatedText(localization.advisorPrimeMinisterTexts[0], (virusStats.populationRatioImmunity*100)) });
    }
}
