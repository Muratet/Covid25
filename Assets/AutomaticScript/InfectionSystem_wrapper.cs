using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class InfectionSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void UpdatePopRatioInfectedUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("InfectionSystem", "UpdatePopRatioInfectedUI", textUI);
	}

	public void UpdateR0UI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("InfectionSystem", "UpdateR0UI", textUI);
	}

}