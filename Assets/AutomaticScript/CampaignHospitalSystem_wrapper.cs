using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class CampaignHospitalSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void BuyCampaignHospital()
	{
		MainLoop.callAppropriateSystemMethod (null, "BuyCampaignHospital", null);
	}

}