using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectTerritory : MonoBehaviour
{
    public EventTrigger trigger;
    public Button button;

    private void OnEnable()
    {
        if (button)
            button.onClick.Invoke();
        if (trigger)
            foreach (EventTrigger.Entry entry in trigger.triggers)
                entry.callback.Invoke(null);
    }
}
