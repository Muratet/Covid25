using UnityEngine;
using FYFY;

public class AdvisorSystem_wrapper : BaseWrapper
{
	public UnityEngine.GameObject chatContent;
	public UnityEngine.GameObject simulationData;
	public TMPro.TMP_Text newChatNotif;
	public UnityEngine.AudioSource audioEffect;
	public Localization localization;
	private void Start()
	{
		this.hideFlags = HideFlags.NotEditable;
		MainLoop.initAppropriateSystemField (system, "chatContent", chatContent);
		MainLoop.initAppropriateSystemField (system, "simulationData", simulationData);
		MainLoop.initAppropriateSystemField (system, "newChatNotif", newChatNotif);
		MainLoop.initAppropriateSystemField (system, "audioEffect", audioEffect);
		MainLoop.initAppropriateSystemField (system, "localization", localization);
	}

	public void TogglePanel()
	{
		MainLoop.callAppropriateSystemMethod (system, "TogglePanel", null);
	}

	public void ClosePanel()
	{
		MainLoop.callAppropriateSystemMethod (system, "ClosePanel", null);
	}

}
