using UnityEngine;
using FYFY;

public class SyncUISystem_wrapper : BaseWrapper
{
	public TimeScale time;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "time", time);
	}

	public void formatStringUI()
	{
		MainLoop.callAppropriateSystemMethod (system, "formatStringUI", null);
	}

}
