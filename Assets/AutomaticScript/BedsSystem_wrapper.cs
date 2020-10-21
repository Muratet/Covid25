using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class BedsSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void UpdateBedsUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("BedsSystem", "UpdateBedsUI", textUI);
	}

}