using UnityEngine;

/// <summary>
/// This component is used to describe a message of the advisors. It is consumed by the <see cref="AdvisorSystem"/>.
/// </summary>
public class ChatMessage : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).

    /// <summary>
    /// The advisor name that produce this message
    /// </summary>
    public string sender;

    /// <summary>
    /// The timestamp when this message was produced
    /// </summary>
    public string timeStamp;

    /// <summary>
    /// The content of the message
    /// </summary>
    public string messageBody;
}