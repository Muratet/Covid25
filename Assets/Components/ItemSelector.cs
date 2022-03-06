using TMPro;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    public int currentItem = 0;
    public string[] items;
    public TextMeshProUGUI itemUI;

    // Start is called before the first frame update
    void Start()
    {
        itemUI.text = items[currentItem];
    }

    public void nextLanguage()
    {
        currentItem++;
        if (currentItem >= items.Length)
            currentItem = 0;
        itemUI.text = items[currentItem];
    }

    public void prevLanguage()
    {
        currentItem--;
        if (currentItem < 0)
            currentItem = items.Length-1;
        itemUI.text = items[currentItem];
    }
}
