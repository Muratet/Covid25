using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class VaccineSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void NewCommand(TMPro.TMP_InputField input)
	{
		MainLoop.callAppropriateSystemMethod ("VaccineSystem", "NewCommand", input);
	}

	public void OnFrontierChange(System.Int32 newValue)
	{
		MainLoop.callAppropriateSystemMethod ("VaccineSystem", "OnFrontierChange", newValue);
	}

	public void UpdateMasksUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("VaccineSystem", "UpdateMasksUI", textUI);
	}

}