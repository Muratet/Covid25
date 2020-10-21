using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class ShortTimeWorkingSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void OnShortTimeWorkingChange(System.Boolean newState)
	{
		MainLoop.callAppropriateSystemMethod ("ShortTimeWorkingSystem", "OnShortTimeWorkingChange", newState);
	}

}