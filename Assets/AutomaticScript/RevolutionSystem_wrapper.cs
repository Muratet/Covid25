using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class RevolutionSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void UpdateRevolutionUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("RevolutionSystem", "UpdateRevolutionUI", textUI);
	}

}