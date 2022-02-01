using UnityEngine;
using FYFY;

public class CurvesSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
	}

	public void SetWindowView(System.Int32 newWindowSize)
	{
		MainLoop.callAppropriateSystemMethod (system, "SetWindowView", newWindowSize);
	}

}
