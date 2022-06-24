using UnityEngine;

/// <summary>
/// This component store prefab references to instantiate messages
/// </summary>
public class ChatPrefabs : MonoBehaviour
{
    /// <summary>
    /// The prefab of a player message (trigger by player actions)
    /// </summary>
    public GameObject PlayerMessage;

    /// <summary>
    /// The prefab of an advisor message (trigger by the simulation)
    /// </summary>
    public GameObject AdvisorMessage;
}
