using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class TutoEnd : MonoBehaviour
{
    public Button button;
    public Animator territoryPannel;
    public Animator countryPannel;
    public VirusStats virusStats;
    public Localization localization;

    private void OnDisable()
    {
        // simuler un double click
        territoryPannel.SetTrigger("Close");
        countryPannel.SetTrigger("Close");
        button.onClick.Invoke();

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = localization.advisorTitleHealth, timeStamp = "0", messageBody = localization.advisorHealthTexts[0] });

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = localization.advisorTitlePrimeMinister, timeStamp = "0", messageBody = localization.getFormatedText(localization.advisorPrimeMinisterTexts[0], (virusStats.populationRatioImmunity*100)) });
    }
}
