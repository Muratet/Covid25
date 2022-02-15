using UnityEngine;
using FYFY;

public class DeadSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void UpdateDailyDeadUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateDailyDeadUI", textUI);
	}

	public void UpdateCumulDeadUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateCumulDeadUI", textUI);
	}

}
