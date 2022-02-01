using UnityEngine;
using FYFY;

public class RevolutionSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject countrySimData;
	public System.Int32 firstNotifStep;
	public System.Int32 secondNotifStep;
	public System.Int32 thirdNotifStep;
	public System.Int32 fourthNotifStep;
	public System.Int32 fifthNotifStep;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "countrySimData", countrySimData);
		MainLoop.initAppropriateSystemField (system, "firstNotifStep", firstNotifStep);
		MainLoop.initAppropriateSystemField (system, "secondNotifStep", secondNotifStep);
		MainLoop.initAppropriateSystemField (system, "thirdNotifStep", thirdNotifStep);
		MainLoop.initAppropriateSystemField (system, "fourthNotifStep", fourthNotifStep);
		MainLoop.initAppropriateSystemField (system, "fifthNotifStep", fifthNotifStep);
	}

	public void UpdateRevolutionUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateRevolutionUI", textUI);
	}

}
