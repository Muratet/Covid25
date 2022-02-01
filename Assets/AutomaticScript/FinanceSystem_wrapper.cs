using UnityEngine;
using FYFY;

public class FinanceSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
	}

	public void UpdateFinanceUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateFinanceUI", textUI);
	}

}
