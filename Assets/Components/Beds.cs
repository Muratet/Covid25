using UnityEngine;
using UnityEngine.UI;

public class Beds : MonoBehaviour {
    // Advice: FYFY component aims to contain only public members (according to Entity-Component-System paradigm).
    [Tooltip("Nombre de lits de réanimation disponibles avant la crise")]
    public int intensiveBeds_default = 5058; // 5058 lits de réanimation avant la crise
    [HideInInspector]
    public int intensiveBeds_current = 5058;
    [Tooltip("Nombre maximum de lits de réanimation disponibles")]
    public int intensiveBeds_high = 14523; // 14523 lits de réanimation suite à réorganisation (annulation des interventions non urgentes, hôpitaux et cliniques privées...)

    [HideInInspector]
    public int intensiveBeds_need = 0; // besoin en lits à un instant "t"

    [Tooltip("UI contrôlant ce composant")]
    public Toggle beds_UIMaps;
    [HideInInspector]
    public bool boostBeds = false;
    [HideInInspector]
    public int boostBedsLastUpdate = -1;
    [HideInInspector]
    public int advisorNotification = -1;
}