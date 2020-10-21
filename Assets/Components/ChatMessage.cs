using UnityEngine;

public class ChatMessage : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    public string sender;
    public string timeStamp;
    public string messageBody;
}