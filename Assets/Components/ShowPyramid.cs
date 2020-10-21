using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPyramid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("DiagramEnterUI");
    }
}
