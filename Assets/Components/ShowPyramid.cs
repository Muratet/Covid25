using UnityEngine;

/// <summary>
/// This component is used to launch animation on pyramid panel on start
/// </summary>
public class ShowPyramid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("DiagramEnterUI");
    }
}
