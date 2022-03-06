using UnityEngine;
using FYFY;

public class VaccineSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void NewCommand(TMPro.TMP_InputField input)
	{
		MainLoop.callAppropriateSystemMethod (system, "NewCommand", input);
	}

	public void OnFrontierChange(ItemSelector newValue)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnFrontierChange", newValue);
	}

	public void _OnFrontierChange(System.Int32 newValue)
	{
		MainLoop.callAppropriateSystemMethod (system, "_OnFrontierChange", newValue);
	}

	public void UpdateMasksUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateMasksUI", textUI);
	}

}
