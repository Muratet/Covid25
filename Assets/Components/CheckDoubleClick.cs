using UnityEngine;

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

    public void CheckDoubleClic(int territoryId)
    {
        // Vérifier si un click a déjà eu lieu sur ce territoire dans un délai court
        if (firstClick && lastTerritorySelected == territoryId && Time.time - clickTime < clickDelay)
        {
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
