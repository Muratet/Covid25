using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class AdvisorSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void TogglePanel()
	{
		MainLoop.callAppropriateSystemMethod ("AdvisorSystem", "TogglePanel", null);
	}

	public void ClosePanel()
	{
		MainLoop.callAppropriateSystemMethod ("AdvisorSystem", "ClosePanel", null);
	}

}