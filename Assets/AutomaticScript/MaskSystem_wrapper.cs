using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class MaskSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void OnMaskRequisitionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "OnMaskRequisitionChange", newState);
	}

	public void OnBoostProductionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "OnBoostProductionChange", newState);
	}

	public void NewCommand(TMPro.TMP_InputField input)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "NewCommand", input);
	}

	public void OnArtisanalProductionChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "OnArtisanalProductionChange", newState);
	}

	public void UpdateMasksUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "UpdateMasksUI", textUI);
	}

	public void OnFrontierChange(System.Int32 newValue)
	{
		MainLoop.callAppropriateSystemMethod ("MaskSystem", "OnFrontierChange", newValue);
	}

}