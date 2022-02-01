using UnityEngine;
using FYFY;

public class InfectionSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
	}

	public void UpdatePopRatioInfectedUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdatePopRatioInfectedUI", textUI);
	}

	public void UpdateR0UI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateR0UI", textUI);
	}

}
