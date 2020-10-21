using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualKeyboard : MonoBehaviour, IPointerClickHandler
{
    private TMPro.TMP_InputField inputField;

    public GameObject virtualKeyboard;
    
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMPro.TMP_InputField>();
    }

    public void insertDigit (string digit)
    {
        int start = Mathf.Min(inputField.selectionStringAnchorPosition, inputField.selectionStringFocusPosition);
        int length = Mathf.Abs(inputField.selectionStringAnchorPosition - inputField.selectionStringFocusPosition);
        if (length > 0)
            inputField.text = inputField.text.Remove(start, length);
        inputField.text = inputField.text.Insert(start, digit);
        inputField.stringPosition = start + 1;
    }

    public void del()
    {
        int start = Mathf.Min(inputField.selectionStringAnchorPosition, inputField.selectionStringFocusPosition);
        int length = Mathf.Abs(inputField.selectionStringAnchorPosition - inputField.selectionStringFocusPosition);
        if (length > 0)
        {
            inputField.text = inputField.text.Remove(start, length);
            inputField.stringPosition = start;
        }
        else if (start > 0)
        {
            inputField.text = inputField.text.Remove(start - 1, 1);
            inputField.stringPosition = start - 1;
        }
    }
    
    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (inputField.interactable && Application.platform == RuntimePlatform.WebGLPlayer && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            virtualKeyboard.SetActive(true);
        
    }

}
