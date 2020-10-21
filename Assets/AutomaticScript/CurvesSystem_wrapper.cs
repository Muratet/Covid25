using UnityEngine;
using FYFY;

[ExecuteInEditMode]
public class CurvesSystem_wrapper : MonoBehaviour
{
	private void Start()
	{
		this.hideFlags = HideFlags.HideInInspector; // Hide this component in Inspector
	}

	public void SetWindowView(System.Int32 newWindowSize)
	{
		MainLoop.callAppropriateSystemMethod ("CurvesSystem", "SetWindowView", newWindowSize);
	}

}