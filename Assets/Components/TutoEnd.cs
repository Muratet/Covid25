using UnityEngine;
using UnityEngine.UI;
using FYFY;

public class TutoEnd : MonoBehaviour
{
    public Button button;
    public Animator territoryPannel;
    public Animator countryPannel;
    public VirusStats virusStats;

    private void OnDisable()
    {
        // simuler un double click
        territoryPannel.SetTrigger("Close");
        countryPannel.SetTrigger("Close");
        button.onClick.Invoke();

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = "Health Minister", timeStamp = "0", messageBody = "So far no case has been detected in Europe." });

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = "Prime Minister", timeStamp = "0", messageBody = "Your goal is for <b>" + (virusStats.populationRatioImmunity*100)+ "% of the population</b> to have developed antibodies." });
    }
}
