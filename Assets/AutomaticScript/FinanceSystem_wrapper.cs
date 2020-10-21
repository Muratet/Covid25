using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class FinanceSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void UpdateFinanceUI(TMPro.TMP_Text textUI)
	{
		MainLoop.callAppropriateSystemMethod ("FinanceSystem", "UpdateFinanceUI", textUI);
	}

}