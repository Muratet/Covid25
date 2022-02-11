using UnityEngine;
using FYFY;

public class DaySystem_wrapper : BaseWrapper
{
	public TimeScale time;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "time", time);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void setPause(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod (system, "setPause", newState);
	}

	public void UpdateTimeUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod (system, "UpdateTimeUI", textUI);
	}

}
