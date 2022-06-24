using UnityEngine;

/// <summary>
/// This component enables to manage double click on the map
/// </summary>
public class CheckDoubleClick : MonoBehaviour
{
    private bool firstClick = false;
    private float clickTime = 0;
    private float clickDelay = 0.5f;
    private int lastTerritorySelected = -1;

    private Animator animatorTerritoryPopUp;
    private Animator animatorCountryPopUp;

    private void Start()
    {
        animatorCountryPopUp = transform.GetChild(0).GetComponent<Animator>();
        animatorTerritoryPopUp = transform.GetChild(1).GetComponent<Animator>();
    }

    /// <summary>
    /// Check if a double click occurs on a territory and open player panels accordingly
    /// </summary>
    /// <param name="territoryId">The territory id on which checking double click</param>
    public void CheckDoubleClic(int territoryId)
    {
        // Check if another click occurs on this territory in a short delay
        if (firstClick && lastTerritorySelected == territoryId && Time.time - clickTime < clickDelay)
        {
            // here we have a double click
            animatorTerritoryPopUp.SetTrigger("Toggle");
            if (territoryId == -1) // National
            {
                animatorCountryPopUp.SetTrigger("Toggle");
                ConfinementSystem.instance.updateCountryUI();
            }
            else
                ConfinementSystem.instance.updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());

            firstClick = false;
        }
        else 
        {
            // here we have a first click 
            firstClick = true;
            clickTime = Time.time;
            lastTerritorySelected = territoryId;
            
            if (territoryId == -1 && animatorTerritoryPopUp.GetCurrentAnimatorClipInfo(0).Length > 0 && animatorTerritoryPopUp.GetCurrentAnimatorClipInfo(0)[0].clip.name == "TerritoryPopUp_in" && !animatorCountryPopUp.GetBool("Close"))
                animatorCountryPopUp.SetTrigger("Open");
            else
                animatorCountryPopUp.SetTrigger("Close");

            if (territoryId == -1) // National
                ConfinementSystem.instance.updateCountryUI();
            else
                ConfinementSystem.instance.updateUI(MapSystem.territorySelected, MapSystem.territorySelected.GetComponent<Beds>());
        }
    }
}
