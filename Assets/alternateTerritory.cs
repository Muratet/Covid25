using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alternateTerritory : MonoBehaviour
{
    public GameObject firstTerritory;
    public GameObject secondTerritory;
    public float delay;

    private float delta = 0;

    // Update is called once per frame
    void Update()
    {
        delta += Time.deltaTime;
        if (delta > delay)
        {
            delta -= delay;
            firstTerritory.SetActive(!firstTerritory.activeSelf);
            secondTerritory.SetActive(!firstTerritory.activeSelf);
        }
    }
}
