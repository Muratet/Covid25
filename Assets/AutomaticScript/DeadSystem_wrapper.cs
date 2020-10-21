using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class DeadSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void UpdateDailyDeadUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("DeadSystem", "UpdateDailyDeadUI", textUI);
	}

	public void UpdateCumulDeadUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("DeadSystem", "UpdateCumulDeadUI", textUI);
	}

}