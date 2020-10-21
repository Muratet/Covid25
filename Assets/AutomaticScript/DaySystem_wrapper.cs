using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class DaySystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void setPause(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("DaySystem", "setPause", newState);
	}

	public void UpdateTimeUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("DaySystem", "UpdateTimeUI", textUI);
	}

}