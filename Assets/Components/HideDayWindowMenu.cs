using UnityEngine;

/// <summary>
/// This component is used to play animation in order to hide day filters when game start
/// </summary>
public class HideDayWindowMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("UpUI_out");
    }
}
