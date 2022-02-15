using UnityEngine;
using FYFY;

public class BedsSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void UpdateBedsUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateBedsUI", textUI);
	}

}
