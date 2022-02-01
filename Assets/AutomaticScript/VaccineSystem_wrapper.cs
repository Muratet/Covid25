using UnityEngine;
using FYFY;

public class VaccineSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
	}

	public void NewCommand(TMPro.TMP_InputField input)
	{
		MainLoop.callAppropriateSystemMethod (system, "NewCommand", input);
	}

	public void OnFrontierChange(System.Int32 newValue)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnFrontierChange", newValue);
	}

	public void UpdateMasksUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateMasksUI", textUI);
	}

}
