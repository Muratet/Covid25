using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectTerritory : MonoBehaviour
{
    public GameObject territories;
    public int territoryNum;
    public Image targetHighlight;
    private EventTrigger trigger;
    public Button button;


    private void OnEnable()
    {
        if (territories != null && territories.transform.childCount > 0)
        {
            int targetTerritory = territoryNum;
            if (targetTerritory < 0)
                targetTerritory = territories.transform.childCount + targetTerritory;
            targetHighlight.sprite = territories.transform.GetChild(targetTerritory).gameObject.GetComponent<Image>().sprite;
            trigger = territories.transform.GetChild(targetTerritory).gameObject.GetComponent<EventTrigger>();
        }
        if (button)
            button.onClick.Invoke();
        if (trigger)
            foreach (EventTrigger.Entry entry in trigger.triggers)
                entry.callback.Invoke(null);
    }
}
