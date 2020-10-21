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

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = "Ministre de la santé", timeStamp = "0", messageBody = "Pour l'instant aucun cas n'a été détecté en Europe." });

        GameObjectManager.addComponent<ChatMessage>(MainLoop.instance.gameObject, new { sender = "Premier ministre", timeStamp = "0", messageBody = "L'objectif est d'<b>atteindre l'immunité collective</b>. D'après les premières études internationales elle sera atteinte lorsque <b>" + (virusStats.populationRatioImmunity*100)+"% de la population</b> aura développé des anticorps." });
    }
}
