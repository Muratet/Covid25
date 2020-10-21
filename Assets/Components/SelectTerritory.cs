using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectTerritory : MonoBehaviour
{
    public EventTrigger trigger;
    public Button button;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if (button)
            button.onClick.Invoke();
        if (trigger)
            foreach (EventTrigger.Entry entry in trigger.triggers)
                entry.callback.Invoke(null);
    }
}
