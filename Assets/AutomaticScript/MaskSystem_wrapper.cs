using UnityEngine;
using FYFY;

public class MaskSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public TMPro.TMP_InputField UI_InputFieldCommand;
	public UnityEngine.UI.Button UI_ButtonCommand;
	public TMPro.TMP_Text UI_UnitPriceValue;
	public TMPro.TMP_Text UI_PendingCommand;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "UI_InputFieldCommand", UI_InputFieldCommand);
		MainLoop.initAppropriateSystemField (system, "UI_ButtonCommand", UI_ButtonCommand);
		MainLoop.initAppropriateSystemField (system, "UI_UnitPriceValue", UI_UnitPriceValue);
		MainLoop.initAppropriateSystemField (system, "UI_PendingCommand", UI_PendingCommand);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void OnMaskRequisitionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnMaskRequisitionChange", newState);
	}

	public void OnBoostProductionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnBoostProductionChange", newState);
	}

	public void NewCommand(TMPro.TMP_InputField input)
	{
		MainLoop.callAppropriateSystemMethod (system, "NewCommand", input);
	}

	public void OnArtisanalProductionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnArtisanalProductionChange", newState);
	}

	public void UpdateMasksUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateMasksUI", textUI);
	}

	public void OnFrontierChange(System.Int32 newValue)
	{
		MainLoop.callAppropriateSystemMethod (system, "OnFrontierChange", newValue);
	}

}
